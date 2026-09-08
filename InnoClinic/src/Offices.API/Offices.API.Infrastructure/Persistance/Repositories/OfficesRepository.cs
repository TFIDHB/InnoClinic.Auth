using InnoClinic.Offices.API.Application.Interfaces;
using InnoClinic.Offices.API.Domain.Entities;
using InnoClinic.Offices.API.Infrastructure.Persistance;
using MongoDB.Driver;

namespace InnoClinic.Offices.API.Infrastructure.Persistance.Repositories
{
    public class OfficesRepository(OfficesDbContext context) : IOfficesRepository
    {
        private readonly IMongoCollection<Office> _collection = context.Offices;

        public async Task<Office?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken ct = default)
            => await _collection.Find(_ => true).ToListAsync(ct);

        public async Task CreateAsync(Office office, CancellationToken ct = default)
            => await _collection.InsertOneAsync(office, null, ct);

        public async Task UpdateAsync(Office office, CancellationToken ct = default)
            => await _collection.ReplaceOneAsync(e => e.Id == office.Id, office, cancellationToken: ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
            => await _collection.DeleteOneAsync(e => e.Id == id, ct);
    }
}
