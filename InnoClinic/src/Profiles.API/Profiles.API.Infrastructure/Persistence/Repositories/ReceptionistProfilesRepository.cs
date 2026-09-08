using InnoClinic.Profiles.API.Application.Interfaces;
using InnoClinic.Profiles.API.Domain.Entities;
using InnoClinic.Profiles.API.Infrastructure.Persistence;
using InnoClinic.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Profiles.API.Infrastructure.Persistence.Repositories
{
    public class ReceptionistProfilesRepository(ProfilesDbContext context)
        : BaseRepository<ReceptionistProfile, Guid>(context), IReceptionistProfilesRepository
    {
        public async Task<ReceptionistProfile?> GetByAccountIdAsync(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.AccountId == id, ct);
        }
    }
}
