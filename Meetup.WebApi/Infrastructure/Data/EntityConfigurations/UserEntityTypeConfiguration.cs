using Meetup.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meetup.WebApi.Infrastructure.Data.EntityConfigurations
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasIndex(p => p.Email)
                .HasName("UQ_Users_Email")
                .IsUnique();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasColumnType("varchar(128)");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasColumnType("varchar(128)");

            builder.Property(p => p.Password)
                .IsRequired()
                .HasColumnType("varchar(256)");

            builder.Ignore(p => p.Notifications);
        }
    }
}
