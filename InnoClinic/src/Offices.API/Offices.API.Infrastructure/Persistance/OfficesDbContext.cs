using InnoClinic.Offices.API.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace InnoClinic.Offices.API.Infrastructure.Persistance
{
    public class OfficesDbContext
    {
        private readonly IMongoDatabase _database;

        public OfficesDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("OfficesConnection"));
            _database = client.GetDatabase("OfficesDb");
        }

        public IMongoCollection<Office> Offices => _database.GetCollection<Office>("Offices");
    }
}
