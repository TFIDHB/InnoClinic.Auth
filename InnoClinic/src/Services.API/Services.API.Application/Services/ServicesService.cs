using AutoMapper;
using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Services.API.Application.Services
{
    public class ServicesService(
        IServicesUnitOfWork unitOfWork,
        IMapper mapper,
        IAppointmentsClient appointmentsClient): IServicesService
    {
        private const int SlotGranularityMinutes = 10;
        private const int StartTime = 8;
        private const int EndTime = 20;

        public async Task<ServiceDto> CreateAsync(CreateServiceRequestDto dto, CancellationToken ct = default)
        {
            if (dto.SpecializationId.HasValue)
            {
                var specialization = await unitOfWork.SpecializationsRepository.GetByIdAsync(dto.SpecializationId.Value, ct)
                    ?? throw new NotFoundException(nameof(Specialization));
            }

            var categoryExists = await unitOfWork.ServicesRepository.CategoryExistsAsync(dto.ServiceCategoryId, ct);
            if (!categoryExists)
            {
                throw new NotFoundException(nameof(ServiceCategory));
            }

            var service = mapper.Map<Service>(dto);
            await unitOfWork.ServicesRepository.CreateAsync(service, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var service = await unitOfWork.ServicesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Service));

            return mapper.Map<ServiceDto>(service);
        }

        public async Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default)
        {
            var services = await unitOfWork.ServicesRepository.GetAllAsync(ct);
            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<ServiceDto> UpdateAsync(Guid id, UpdateServiceRequestDto dto, CancellationToken ct = default)
        {
            var service = await unitOfWork.ServicesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Service));

            if (dto.SpecializationId.HasValue)
            {
                var specialization = await unitOfWork.SpecializationsRepository.GetByIdAsync(dto.SpecializationId.Value, ct)
                    ?? throw new NotFoundException(nameof(Specialization));
            }

            var categoryExists = await unitOfWork.ServicesRepository.CategoryExistsAsync(dto.ServiceCategoryId, ct);
            if (!categoryExists)
            {
                throw new NotFoundException(nameof(ServiceCategory));
            }

            mapper.Map(dto, service);
            await unitOfWork.ServicesRepository.UpdateAsync(service, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> UpdateStatusAsync(Guid id, UpdateServiceStatusRequestDto dto, CancellationToken ct = default)
        {
            var service = await unitOfWork.ServicesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Service));

            mapper.Map(dto, service);
            await unitOfWork.ServicesRepository.UpdateAsync(service, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var service = await unitOfWork.ServicesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Service));

            await unitOfWork.ServicesRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<TimeOnly>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto dto, CancellationToken ct = default)
        {
            var timeSlotSize = await unitOfWork.ServicesRepository.GetTimeSlotSizeAsync(dto.ServiceId, ct);
            var appointments = await appointmentsClient.GetAppointmentsAsync(dto.Date, dto.DoctorId, ct);
            var busySlots = GetBusySlots(appointments);
            return GenerateAllSlots(timeSlotSize)
                .Where(slot => Enumerable
                    .Range(0, timeSlotSize)
                    .Select(i => slot.AddMinutes(i * 10))
                    .All(s => !busySlots.Contains(s)));
        }

        public async Task<IEnumerable<DateOnly>> GetAvailableDatesAsync(GetAvailableDatesRequestDto dto, CancellationToken ct = default)
        {
            var timeSlotSize = await unitOfWork.ServicesRepository.GetTimeSlotSizeAsync(dto.ServiceId, ct);
            var today = DateOnly.FromDateTime(DateTime.Today);
            var startDate = today;
            var endDate = today.AddDays(29);
            var result = new List<DateOnly>();
            var allSlots = GenerateAllSlots(timeSlotSize).ToList();
            var allAppointments = await appointmentsClient.GetAppointmentsRangeAsync(startDate, endDate, dto.DoctorId, ct);

            for (var i = 0; i < 30; i++)
            {
                var date = today.AddDays(i);
                var appointments = allAppointments.Where(e => e.Date == date);
                var busySlots = GetBusySlots(appointments);
                if (HasFreeSlot(allSlots, busySlots, timeSlotSize))
                {
                    result.Add(date);
                }
            }

            return result;
        }

        public async Task<int> GetTimeSlotSizeAsync(Guid serviceId, CancellationToken ct = default)
        {
            var timeSlotSize = await unitOfWork.ServicesRepository.GetTimeSlotSizeAsync(serviceId, ct);
            if (timeSlotSize == 0)
            {
                throw new NotFoundException(nameof(Service));
            }

            return timeSlotSize;
        }

        public async Task<IEnumerable<ServiceDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var services = await unitOfWork.ServicesRepository.GetByIdsAsync(ids, ct);
            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        private bool HasFreeSlot(IEnumerable<TimeOnly> allSlots, HashSet<TimeOnly> busySlots, int requiredSlots)
            => allSlots.Any(slot => Enumerable
            .Range(0, requiredSlots)
            .All(j => !busySlots.Contains(slot.AddMinutes(j * SlotGranularityMinutes))));

        private static HashSet<TimeOnly> GetBusySlots(IEnumerable<AppointmentSlotDto> appointments)
            => appointments
                .SelectMany(a => Enumerable
                    .Range(0, (int)a.Duration.TotalMinutes / SlotGranularityMinutes)
                    .Select(i => a.Time.AddMinutes(i * SlotGranularityMinutes)))
                .ToHashSet();

        private static IEnumerable<TimeOnly> GenerateAllSlots(int requiredSlots)
        {
            var slot = new TimeOnly(StartTime, 0);
            var end = new TimeOnly(EndTime, 0).AddMinutes(-(requiredSlots - 1) * SlotGranularityMinutes);
            while (slot < end)
            {
                yield return slot;
                slot = slot.AddMinutes(SlotGranularityMinutes);
            }
        }
    }
}
