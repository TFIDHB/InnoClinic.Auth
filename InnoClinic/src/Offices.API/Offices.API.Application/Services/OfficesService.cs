using AutoMapper;
using InnoClinic.Offices.API.Application.DTOs;
using InnoClinic.Offices.API.Application.Interfaces;
using InnoClinic.Offices.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Offices.API.Application.Services
{
    public class OfficesService(IOfficesRepository officesRepository, IMapper mapper) : IOfficesService
    {
        private readonly IOfficesRepository _officesRepository = officesRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<OfficeDto> CreateAsync(CreateOfficeRequestDto dto, CancellationToken ct = default)
        {
            var office = _mapper.Map<Office>(dto);

            await _officesRepository.CreateAsync(office, ct);
            return _mapper.Map<OfficeDto>(office);
        }

        public async Task<OfficeDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var office = await _officesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Office));

            return _mapper.Map<OfficeDto>(office);
        }

        public async Task<IEnumerable<OfficeDto>> GetAllAsync(CancellationToken ct = default)
        {
            var offices = await _officesRepository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<OfficeDto>>(offices);
        }

        public async Task<OfficeDto> UpdateAsync(Guid id, UpdateOfficeRequestDto dto, CancellationToken ct = default)
        {
            var office = await _officesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Office));

            _mapper.Map(dto, office);
            await _officesRepository.UpdateAsync(office, ct);
            return _mapper.Map<OfficeDto>(office);
        }

        public async Task<OfficeDto> UpdateStatusAsync(Guid id, UpdateOfficeStatusRequestDto dto, CancellationToken ct = default)
        {
            var office = await _officesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Office));

            _mapper.Map(dto, office);
            await _officesRepository.UpdateAsync(office, ct);
            return _mapper.Map<OfficeDto>(office);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var office = await _officesRepository.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Office));

            await _officesRepository.DeleteAsync(id, ct);
        }
    }
}
