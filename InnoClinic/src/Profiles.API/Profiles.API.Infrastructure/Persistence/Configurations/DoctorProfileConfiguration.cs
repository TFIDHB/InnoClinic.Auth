using InnoClinic.Profiles.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoClinic.Profiles.API.Infrastructure.Persistence.Configurations
{
    public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasDefaultValueSql("newsequentialid()")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName).IsRequired();
            builder.Property(e => e.LastName).IsRequired();
            builder.Property(e => e.MiddleName);
            builder.Property(e => e.CareerStartYear).IsRequired();
            builder.Property(e => e.Status).HasConversion<string>();
            builder.Property(e => e.SpecializationId).IsRequired();
            builder.HasIndex(e => e.AccountId).IsUnique()
                .IsUnique()
                .HasFilter("[AccountId] IS NOT NULL");
        }
    }
}
