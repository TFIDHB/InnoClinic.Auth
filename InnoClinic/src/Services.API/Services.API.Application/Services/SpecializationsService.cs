using AutoMapper;
using InnoClinic.Services.API.Application.DTOs;
using InnoClinic.Services.API.Application.Exceptions;
using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Services.API.Application.Services
{
    public class SpecializationsService(
        IServicesUnitOfWork unitOfWork,
        IMapper mapper): ISpecializationsService
    {
        public async Task<SpecializationDto> CreateAsync(CreateSpecializationRequestDto dto, CancellationToken ct = default)
        {
            var specialization = mapper.Map<Specialization>(dto);
            var services = await unitOfWork.ServicesRepository.GetByIdsAsync(dto.ServiceIds, ct);

            if (services.Count() != dto.ServiceIds.Count())
            {
                var missingIds = dto.ServiceIds.Where(id => !services.Any(e => e.Id == id));
                throw new ServicesNotFoundException(string.Join(", ", missingIds));
            }

            var inactiveService = services.FirstOrDefault(e => !e.IsActive);
            if (inactiveService != null)
            {
                throw new InactiveServiceLinkException(inactiveService.Id);
            }

            specialization.Services = services.ToList();

            await unitOfWork.SpecializationsRepository.CreateAsync(specialization, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return mapper.Map<SpecializationDto>(specialization);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var specialization = await unitOfWork.SpecializationsRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Specialization));

            await unitOfWork.SpecializationsRepository.DeleteAsync(id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<SpecializationDto>> GetAllAsync(CancellationToken ct = default)
        {
            var specializations = await unitOfWork.SpecializationsRepository.GetAllWithServicesAsync(ct);
            return mapper.Map<IEnumerable<SpecializationDto>>(specializations);
        }

        public async Task<SpecializationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var specialization = await unitOfWork.SpecializationsRepository.GetByIdWithServicesAsync(id, ct)
                ?? throw new NotFoundException(nameof(Specialization));
            return mapper.Map<SpecializationDto>(specialization);
        }

        public async Task<SpecializationDto> UpdateAsync(Guid id, UpdateSpecializationRequestDto dto, CancellationToken ct = default)
        {
            var specialization = await unitOfWork.SpecializationsRepository.GetByIdWithServicesAsync(id, ct)
                ?? throw new NotFoundException(nameof(Specialization));

            mapper.Map(dto, specialization);
            var services = await unitOfWork.ServicesRepository.GetByIdsAsync(dto.ServiceIds, ct);

            if (services.Count() != dto.ServiceIds.Count())
            {
                var missingIds = dto.ServiceIds.Where(id => !services.Any(e => e.Id == id));
                throw new ServicesNotFoundException(string.Join(", ", missingIds));
            }

            specialization.Services = services.ToList();

            if (specialization.IsActive == false)
            {
                foreach (var service in specialization.Services)
                {
                    service.IsActive = false;
                }
            }

            await unitOfWork.SpecializationsRepository.UpdateAsync(specialization, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<SpecializationDto>(specialization);
        }

        public async Task<SpecializationDto> UpdateStatusAsync(Guid id, UpdateSpecializationStatusRequestDto dto, CancellationToken ct = default)
        {
            var specialization = await unitOfWork.SpecializationsRepository.GetByIdWithServicesAsync(id, ct)
                ?? throw new NotFoundException(nameof(Specialization));

            mapper.Map(dto, specialization);

            if (specialization.IsActive == false)
            {
                foreach (var service in specialization.Services)
                {
                    service.IsActive = false;
                }
            }

            await unitOfWork.SpecializationsRepository.UpdateAsync(specialization, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return mapper.Map<SpecializationDto>(specialization);
        }
    }
}
