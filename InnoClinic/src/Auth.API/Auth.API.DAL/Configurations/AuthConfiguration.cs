using InnoClinic.Auth.API.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoClinic.Auth.API.DAL.Configurations
{
    public class AuthConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.PasswordHash).IsRequired();
            builder.Property(e => e.RefreshToken);
        }
    }
}
