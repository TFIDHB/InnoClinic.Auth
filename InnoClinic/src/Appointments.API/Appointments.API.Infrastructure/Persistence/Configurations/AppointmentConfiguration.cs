using InnoClinic.Appointments.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();

            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.DoctorId).IsRequired();
            builder.Property(e => e.ServiceId).IsRequired();
            builder.Property(e => e.OfficeId).IsRequired();

            builder.Property(e => e.Date).HasColumnType("date");
            builder.Property(e => e.Time).HasColumnType("time");

            builder.Property(e => e.Duration).HasColumnType("interval").IsRequired();

            builder.Property(e => e.IsApproved);

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("now() AT TIME ZONE 'utc'").ValueGeneratedOnAdd();
        }
    }
}
