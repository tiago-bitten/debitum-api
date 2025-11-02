using Domain.Debts.Entities;
using Infra.Postgres.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Postgres.Debts.Configurations;

internal sealed class ReminderSentConfiguration : BaseEntityConfiguration<ReminderSent>
{
    public override void Configure(EntityTypeBuilder<ReminderSent> builder)
    {
        base.Configure(builder);

        builder.ToTable("reminders_sent");

        builder.Property(r => r.DebtId)
            .HasColumnName("debt_id")
            .IsRequired();

        builder.Property(r => r.Channel)
            .HasColumnName("channel")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Channel)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(r => r.Message)
            .HasColumnName("message")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(r => r.Message)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(r => r.SentAt)
            .HasColumnName("sent_at")
            .IsRequired();

        builder.Property(r => r.SentAt)
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(r => r.DebtId);
        builder.HasIndex(r => r.SentAt);
    }
}
