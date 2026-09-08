using InnoClinic.Services.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoClinic.Services.API.Infrastructure.Persistence.Configurations
{
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newsequentialid()").ValueGeneratedOnAdd();
            builder.Property(e => e.Name).IsRequired();
            builder.HasMany(e => e.Services).WithOne(e => e.Specialization).HasForeignKey(e => e.SpecializationId);
        }
    }
}