using Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Shared.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        // Configure property access mode to allow EF Core to set private/protected setters
        builder.Property(e => e.Id)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.PublicId)
            .HasColumnName("public_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.PublicId)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(e => e.PublicId)
            .IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore the Events collection (not mapped to database)
        builder.Ignore(e => e.Events);
    }
}
