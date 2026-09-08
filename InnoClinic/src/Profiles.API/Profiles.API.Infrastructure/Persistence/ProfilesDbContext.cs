using InnoClinic.Profiles.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Profiles.API.Infrastructure.Persistence
{
    public class ProfilesDbContext : DbContext
    {
        public ProfilesDbContext(DbContextOptions<ProfilesDbContext> context)
            : base(context)
        {
        }

        public DbSet<DoctorProfile> DoctorProfiles { get; set; }

        public DbSet<PatientProfile> PatientProfiles { get; set; }

        public DbSet<ReceptionistProfile> ReceptionistProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfilesDbContext).Assembly);
        }
    }
}
