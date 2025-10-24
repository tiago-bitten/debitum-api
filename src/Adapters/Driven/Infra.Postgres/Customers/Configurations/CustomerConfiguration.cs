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

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone.Number,
                number => Phone.Create(number).Value)
            .IsRequired();

        builder.Property(c => c.Phone)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(c => c.Origin)
            .HasColumnName("origin")
            .HasMaxLength(50)
            .HasConversion(
                origin => origin.ToString(),
                origin => Enum.Parse<CustomerOrigin>(origin))
            .IsRequired();

        builder.Property(c => c.Origin)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}