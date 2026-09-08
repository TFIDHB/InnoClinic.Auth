using InnoClinic.Services.API.Application.Interfaces;
using InnoClinic.Services.API.Domain.Entities;
using InnoClinic.Services.API.Infrastructure.Persistence;
using InnoClinic.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Services.API.Infrastructure.Persistence.Repositories
{
    public class SpecializationsRepository(ServicesDbContext context) : BaseRepository<Specialization, Guid>(context), ISpecializationsRepository
    {
        public async Task<Specialization?> GetByIdWithServicesAsync(Guid id, CancellationToken ct = default)
            => await context.Specializations
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<IEnumerable<Specialization>> GetAllWithServicesAsync(CancellationToken ct = default)
            => await context.Specializations
            .Include(s => s.Services)
            .ToListAsync(ct);
    }
}
