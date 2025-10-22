using Domain.Customers.Entities;
using Domain.Customers.Enums;
using Domain.Shared.ValueObjects;
using Infra.Postgres.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Customers.Configurations;

internal sealed class CustomerConfiguration : BaseDeletableEntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.ToTable("customers");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Name)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Mapear Email Value Object
        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .HasConversion(
                email => email.Address,
                address => Email.Create(address).Value)
            .IsRequired();

        builder.Property(c => c.Email)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        // Mapear Phone Value Object
        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone.Number,
                number => Phone.Create(number).Value)
            .IsRequired();

        builder.Property(c => c.Phone)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Mapear Enum como string
        builder.Property(c => c.Origem)
            .HasColumnName("origem")
            .HasMaxLength(50)
            .HasConversion(
                origem => origem.ToString(),
                origem => Enum.Parse<CustomerOrigem>(origem))
            .IsRequired();

        builder.Property(c => c.Origem)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
