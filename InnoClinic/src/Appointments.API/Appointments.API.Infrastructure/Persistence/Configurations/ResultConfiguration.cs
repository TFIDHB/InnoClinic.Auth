using InnoClinic.Appointments.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence.Configurations
{
    public class ResultConfiguration : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
            builder.HasIndex(e => e.AppointmentId).IsUnique();
            builder.Property(e => e.Complaints).IsRequired();
            builder.Property(e => e.Conclusion).IsRequired();
            builder.Property(e => e.Recommendations).IsRequired();
        }
    }
}
