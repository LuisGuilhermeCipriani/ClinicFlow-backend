using ClinicFlow.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("USERS");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedOnAdd();

        builder.Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(User.MaxUsernameLength);

        builder.Property(user => user.DisplayName)
            .IsRequired()
            .HasMaxLength(User.MaxDisplayNameLength);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(User.MaxEmailLength);

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(User.MaxPasswordHashLength);

        builder.Property(user => user.Role)
            .IsRequired()
            .HasMaxLength(User.MaxRoleLength);

        builder.Property(user => user.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.CreatedBy)
            .HasMaxLength(128);

        builder.Property(user => user.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(user => user.DeletedBy)
            .HasMaxLength(128);

        builder.HasIndex(user => user.Username)
            .IsUnique()
            .HasDatabaseName("UX_USERS_USERNAME");

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName("UX_USERS_EMAIL");
    }
}
