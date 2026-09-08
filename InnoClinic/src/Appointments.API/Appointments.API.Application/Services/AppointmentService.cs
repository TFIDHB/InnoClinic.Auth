using AutoMapper;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Exceptions;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Application.Options;
using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;
using Microsoft.Extensions.Options;

namespace InnoClinic.Appointments.API.Application.Services
{
    public class AppointmentService(
        IAppointmentUnitOfWork unitOfWork,
        IMapper mapper,
        IServicesClient servicesClient,
        IProfilesClient profilesClient,
        IOptions<WorkingHoursOptions> workingHoursOptions) : IAppointmentService
    {
        private const int _atWorkStatus = 0;

        public async Task<AppointmentResponseDto> GetByIdAsync(
            Guid appointmentId,
            Guid? patientId,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            if (patientId.HasValue)
            {
                var patientInfo = await profilesClient.GetPatientInfoAsync(appointment.PatientId, ct)
                    ?? throw new NotFoundException("Patient");

                if (patientInfo.AccountId != patientId.Value)
                {
                    throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
                }
            }

            if (doctorId.HasValue)
            {
                var doctorInfo = await profilesClient.GetDoctorInfoAsync(appointment.DoctorId, ct)
                    ?? throw new NotFoundException("Doctor");

                if (doctorInfo.AccountId != doctorId.Value)
                {
                    throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
                }
            }

            return mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<AppointmentResponseDto> CreateAsync(
            CreateAppointmentRequestDto dto,
            bool isPatient,
            CancellationToken ct = default)
        {
            if (isPatient)
            {
                var currentPatientId = await profilesClient.GetMyPatientProfileIdAsync(ct);
                dto.PatientId = currentPatientId;
            }

            var doctorInfo = await profilesClient.GetDoctorInfoAsync(dto.DoctorId, ct)
                ?? throw new NotFoundException("Doctor");

            if (doctorInfo.SpecializationId != dto.SpecializationId || doctorInfo.OfficeId != dto.OfficeId)
            {
                throw new BadRequestException(AppointmentsApiMessages.DoctorDoesNotMatchMessage);
            }

            if (doctorInfo.Status != _atWorkStatus)
            {
                throw new BadRequestException(AppointmentsApiMessages.DoctorNotAvailableMessage);
            }

            var timeSlotSize = await servicesClient.GetTimeSlotSizeAsync(dto.ServiceId, ct);
            var durationMinutes = timeSlotSize * 10;
            var newStart = dto.Time;
            var newEnd = dto.Time.AddMinutes(durationMinutes);

            EnsureWithinWorkingHours(newStart, newEnd);

            var isOverlapping = await IsOverlappingAsync(dto.DoctorId, dto.Date, newStart, newEnd, excludeAppointmentId: null, ct);
            if (isOverlapping)
            {
                throw new OverlappingAppointmentException();
            }

            var appointment = mapper.Map<Appointment>(dto);
            appointment.Duration = TimeSpan.FromMinutes(durationMinutes);
            appointment.OfficeId = doctorInfo.OfficeId;

            await unitOfWork.AppointmentRepository.CreateAsync(appointment, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentSlotDto>> GetSlotsByDateAndDoctorAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            var appointments = await unitOfWork.AppointmentRepository.GetByDateAndDoctorAsync(date, doctorId, ct);
            return mapper.Map<IEnumerable<AppointmentSlotDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentSlotDto>> GetSlotsByDateRangeAndDoctorAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            var appointments = await unitOfWork.AppointmentRepository.GetByDateRangeAndDoctorAsync(startDate, endDate, doctorId, ct);
            return mapper.Map<IEnumerable<AppointmentSlotDto>>(appointments);
        }

        public async Task<IEnumerable<ScheduleDto>> GetDoctorAppointmentScheduleAsync(
            Guid doctorId,
            DateOnly date,
            CancellationToken ct = default)
        {
            var appointments = await unitOfWork.AppointmentRepository.GetByDateAndDoctorAsync(date, doctorId, ct);
            var orderedAppointments = appointments.OrderBy(e => e.Time).ToList();

            var existingResultAppointmentIds = await unitOfWork.ResultRepository
                .GetExistingAppointmentIdsAsync(orderedAppointments.Select(e => e.Id), ct);

            var serviceNames = await servicesClient.GetServiceNamesAsync(
                orderedAppointments.Select(e => e.ServiceId), ct);
            var patients = await profilesClient.GetPatientsInfoAsync(
                orderedAppointments.Select(e => e.PatientId), ct);

            return orderedAppointments.Select(appointment =>
            {
                var entry = mapper.Map<ScheduleDto>(appointment);

                entry.PatientFullName = patients.TryGetValue(appointment.PatientId, out var patientInfo)
                    ? $"{patientInfo.LastName} {patientInfo.FirstName} {patientInfo.MiddleName}".Trim()
                    : "Unknown patient";
                entry.ServiceName = serviceNames.TryGetValue(appointment.ServiceId, out var serviceName)
                    ? serviceName
                    : "Unknown service";
                entry.HasResult = existingResultAppointmentIds.Contains(appointment.Id);

                return entry;
            }).ToList();
        }

        public async Task<IEnumerable<AppointmentListItemDto>> GetFilteredAppointmentsAsync(
            DateOnly? date,
            Guid? officeId,
            bool? isApproved,
            string? doctorFullName,
            string? serviceName,
            CancellationToken ct = default)
        {
            var appointments = await unitOfWork.AppointmentRepository.GetFilteredAsync(date, officeId, isApproved, ct);

            var doctors = await profilesClient.GetDoctorsInfoAsync(
                appointments.Select(e => e.DoctorId), ct);
            var patients = await profilesClient.GetPatientsInfoAsync(
                appointments.Select(e => e.PatientId), ct);
            var serviceNames = await servicesClient.GetServiceNamesAsync(
                appointments.Select(e => e.ServiceId), ct);

            var enrichedAppointments = appointments.Select(appointment =>
            {
                var entry = mapper.Map<AppointmentListItemDto>(appointment);

                entry.DoctorFullName = doctors.TryGetValue(appointment.DoctorId, out var doctorInfo)
                    ? $"{doctorInfo.LastName} {doctorInfo.FirstName} {doctorInfo.MiddleName}".Trim()
                    : "Unknown doctor";
                entry.PatientFullName = patients.TryGetValue(appointment.PatientId, out var patientInfo)
                    ? $"{patientInfo.LastName} {patientInfo.FirstName} {patientInfo.MiddleName}".Trim()
                    : "Unknown patient";
                entry.PatientPhoneNumber = patients.TryGetValue(appointment.PatientId, out var pi) ? pi.PhoneNumber : null;
                entry.ServiceName = serviceNames.TryGetValue(appointment.ServiceId, out var name) ? name : "Unknown service";

                return entry;
            }).ToList();

            if (!string.IsNullOrWhiteSpace(doctorFullName))
            {
                enrichedAppointments = enrichedAppointments
                    .Where(e => e.DoctorFullName.Contains(doctorFullName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                enrichedAppointments = enrichedAppointments
                    .Where(e => e.ServiceName.Contains(serviceName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return enrichedAppointments
                .OrderBy(e => e.StartTime)
                .ThenBy(e => e.DoctorFullName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.ServiceName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task ApproveAsync(Guid id, CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            appointment.IsApproved = true;

            await unitOfWork.AppointmentRepository.UpdateAsync(appointment, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(Guid id, CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            await unitOfWork.AppointmentRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<AppointmentHistoryItemDto>> GetPatientHistoryAsync(Guid patientId, CancellationToken ct = default)
        {
            var appointments = (await unitOfWork.AppointmentRepository.GetByPatientAsync(patientId, ct)).ToList();

            var existingResultAppointmentIds = await unitOfWork.ResultRepository
                .GetExistingAppointmentIdsAsync(appointments.Select(a => a.Id), ct);

            var doctors = await profilesClient.GetDoctorsInfoAsync(
                appointments.Select(a => a.DoctorId), ct);
            var serviceNames = await servicesClient.GetServiceNamesAsync(
                appointments.Select(a => a.ServiceId), ct);

            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            return appointments.Select(appointment =>
            {
                var entry = mapper.Map<AppointmentHistoryItemDto>(appointment);

                entry.DoctorFullName = doctors.TryGetValue(appointment.DoctorId, out var doctorInfo)
                    ? $"{doctorInfo.LastName} {doctorInfo.FirstName} {doctorInfo.MiddleName}".Trim()
                    : "Unknown doctor";
                entry.ServiceName = serviceNames.TryGetValue(appointment.ServiceId, out var serviceName)
                    ? serviceName
                    : "Unknown service";
                entry.HasResult = existingResultAppointmentIds.Contains(appointment.Id);
                entry.CanReschedule = !appointment.IsApproved &&
                    (appointment.Date > today || (appointment.Date == today && appointment.Time > currentTime));

                return entry;
            }).ToList();
        }

        public async Task<AppointmentResponseDto> RescheduleAsync(
            Guid appointmentId,
            RescheduleAppointmentRequestDto dto,
            Guid? patientId,
            CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            if (patientId.HasValue)
            {
                var patientInfo = await profilesClient.GetPatientInfoAsync(appointment.PatientId, ct)
                    ?? throw new NotFoundException("Patient");

                if (patientInfo.AccountId != patientId.Value)
                {
                    throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
                }
            }

            if (appointment.IsApproved)
            {
                throw new BadRequestException(AppointmentsApiMessages.CannotBeRescheduledMessage);
            }

            var newStart = dto.Time;
            var newEnd = dto.Time.Add(appointment.Duration);

            EnsureWithinWorkingHours(newStart, newEnd);

            var isOverlapping = await IsOverlappingAsync(dto.DoctorId, dto.Date, newStart, newEnd, appointmentId, ct);
            if (isOverlapping)
            {
                throw new OverlappingAppointmentException();
            }

            var doctorInfo = await profilesClient.GetDoctorInfoAsync(dto.DoctorId, ct)
                ?? throw new NotFoundException("Doctor");
            appointment.DoctorId = dto.DoctorId;
            appointment.OfficeId = doctorInfo.OfficeId;
            appointment.Date = dto.Date;
            appointment.Time = dto.Time;

            await unitOfWork.AppointmentRepository.UpdateAsync(appointment, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentHistoryItemDto>> GetMyHistoryAsync(CancellationToken ct = default)
        {
            var patientProfileId = await profilesClient.GetMyPatientProfileIdAsync(ct);
            return await GetPatientHistoryAsync(patientProfileId, ct);
        }

        public async Task<IEnumerable<ScheduleDto>> GetMyScheduleAsync(DateOnly date, CancellationToken ct = default)
        {
            var doctorProfileId = await profilesClient.GetMyDoctorProfileIdAsync(ct);
            return await GetDoctorAppointmentScheduleAsync(doctorProfileId, date, ct);
        }

        private async Task<bool> IsOverlappingAsync(
            Guid doctorId,
            DateOnly date,
            TimeOnly newStart,
            TimeOnly newEnd,
            Guid? excludeAppointmentId,
            CancellationToken ct)
        {
            return await unitOfWork.AppointmentRepository.AnyAsync(
                a =>
                a.DoctorId == doctorId &&
                a.Date == date &&
                (excludeAppointmentId == null || a.Id != excludeAppointmentId.Value) &&
                newStart < a.Time.Add(a.Duration) &&
                newEnd > a.Time, ct);
        }

        private void EnsureWithinWorkingHours(TimeOnly start, TimeOnly end)
        {
            var workingHours = workingHoursOptions.Value;
            if (start < workingHours.Start || end > workingHours.End)
            {
                throw new BadRequestException(string.Format(
                    AppointmentsApiMessages.AppointmentBetweenMessage,
                    workingHours.Start,
                    workingHours.End));
            }
        }
    }
}