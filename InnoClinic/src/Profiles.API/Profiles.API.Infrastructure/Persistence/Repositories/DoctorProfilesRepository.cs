using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Profiles.API.Domain.Enums;
using InnoClinic.Profiles.API.Infrastructure.Persistence;
using InnoClinic.Shared.Helpers;
using InnoClinic.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Profiles.API.Infrastructure.Persistence.Repositories
{
    public class DoctorProfilesRepository(ProfilesDbContext context)
        : BaseRepository<DoctorProfile, Guid>(context), IDoctorProfilesRepository
    {
        public async Task<DoctorProfile?> GetByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(profile => profile.AccountId == id, ct);
        }

        public async Task<IEnumerable<DoctorProfile>> GetFilteredAsync(
            Guid? specializationId,
            Guid? officeId,
            string? search,
            DoctorStatus? status,
            CancellationToken ct = default)
        {
            var query = context.Set<DoctorProfile>().AsQueryable();

            if (specializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == specializationId.Value);
            }

            if (officeId.HasValue)
            {
                query = query.Where(d => d.OfficeId == officeId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var terms = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in terms)
                {
                    var escaped = LikeTermHelper.EscapeLikeTerm(term);
                    query = query.Where(d =>
                        EF.Functions.Like(d.FirstName, $"%{escaped}%") ||
                        EF.Functions.Like(d.LastName, $"%{escaped}%") ||
                        d.MiddleName != null && EF.Functions.Like(d.MiddleName, $"%{escaped}%"));
                }
            }

            return await query.ToListAsync(ct);
        }

        public async Task<IEnumerable<DoctorProfile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var idSet = ids.ToHashSet();
            return await context.DoctorProfiles.Where(p => idSet.Contains(p.Id)).ToListAsync();
        }
    }
}
