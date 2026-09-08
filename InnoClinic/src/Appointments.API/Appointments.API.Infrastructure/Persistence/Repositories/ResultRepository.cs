using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Appointments.API.Infrastructure.Persistence;
using InnoClinic.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence.Repositories
{
    public class ResultRepository(AppointmentDbContext context) : BaseRepository<Result, Guid>(context), IResultRepository
    {
        public async Task<Result?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(e => e.AppointmentId == appointmentId, ct);
        }

        public async Task<HashSet<Guid>> GetExistingAppointmentIdsAsync(IEnumerable<Guid> appointmentIds, CancellationToken ct = default)
        {
            var ids = appointmentIds.ToList();
            if (ids.Count == 0)
            {
                return [];
            }

            var existing = await DbSet
                .Where(e => ids.Contains(e.AppointmentId))
                .Select(e => e.AppointmentId)
                .ToListAsync(ct);

            return existing.ToHashSet();
        }
    }
}
