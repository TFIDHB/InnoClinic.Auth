using InnoClinic.Appointments.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence
{
    public class AppointmentDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Result> Results { get; set; }

        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentDbContext).Assembly);
        }
    }
}
