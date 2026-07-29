using ClinicFlow.Domain.Specialties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicFlow.Infrastructure.Persistence.Configurations;

public sealed class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("SPECIALTIES");
        builder.HasKey(specialty => specialty.Id);

        builder.Property(specialty => specialty.Id)
            .ValueGeneratedOnAdd();

        builder.Property(specialty => specialty.Name)
            .HasMaxLength(Specialty.MaxNameLength)
            .IsRequired();

        builder.Property(specialty => specialty.Description)
            .HasMaxLength(Specialty.MaxDescriptionLength);

        builder.Property(specialty => specialty.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(specialty => specialty.CreatedAt)
            .IsRequired();

        builder.Property(specialty => specialty.CreatedBy)
            .HasMaxLength(128);

        builder.Property(specialty => specialty.UpdatedBy)
            .HasMaxLength(128);

        builder.Property(specialty => specialty.DeletedBy)
            .HasMaxLength(128);

        builder.HasIndex(specialty => specialty.Name)
            .IsUnique()
            .HasDatabaseName("UX_SPECIALTIES_NAME");
    }
}
