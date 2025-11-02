using Domain.Debts.Entities;
using Infra.Postgres.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Debts.Configurations;

internal sealed class DebtConfiguration : BaseEntityConfiguration<Debt>
{
    public override void Configure(EntityTypeBuilder<Debt> builder)
    {
        base.Configure(builder);

        builder.ToTable("debts");

        builder.Property(d => d.DebtorId)
            .HasColumnName("debtor_id")
            .IsRequired();

        builder.Property(d => d.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.Amount)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(d => d.DueDate)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(d => d.Description)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.IsPaid)
            .HasColumnName("is_paid")
            .IsRequired();

        builder.Property(d => d.IsPaid)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(d => d.PaidAt)
            .HasColumnName("paid_at");

        builder.Property(d => d.PaidAt)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(d => d.RemindersSent)
            .WithOne()
            .HasForeignKey(r => r.DebtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(d => d.RemindersSent)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(d => d.LastReminderSentAt);
        builder.Ignore(d => d.RemindersSentCount);

        builder.HasIndex(d => d.DebtorId);
        builder.HasIndex(d => d.IsPaid);
        builder.HasIndex(d => d.DueDate);
    }
}
