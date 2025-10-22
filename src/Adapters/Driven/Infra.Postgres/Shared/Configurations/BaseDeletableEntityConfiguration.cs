using Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Shared.Configurations;

public abstract class BaseDeletableEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : EntityDeletable
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(e => e.IsDeleted);
    }
}
