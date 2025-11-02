using Domain.Debts.Entities;
using Domain.Shared.ValueObjects;
using Infra.Postgres.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Debts.Configurations;

internal sealed class DebtorConfiguration : BaseDeletableEntityConfiguration<Debtor>
{
    public override void Configure(EntityTypeBuilder<Debtor> builder)
    {
        base.Configure(builder);

        builder.ToTable("debtors");

        builder.Property(d => d.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Name)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone.Number,
                number => Phone.Create(number).Value)
            .IsRequired();

        builder.Property(d => d.Phone)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .HasConversion(
                email => email!.Address,
                address => Email.Create(address).Value!)
            .IsRequired();

        builder.Property(d => d.Email)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(d => d.Debts)
            .WithOne()
            .HasForeignKey(debt => debt.DebtorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(d => d.Debts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(d => d.CustomerId);
    }
}
