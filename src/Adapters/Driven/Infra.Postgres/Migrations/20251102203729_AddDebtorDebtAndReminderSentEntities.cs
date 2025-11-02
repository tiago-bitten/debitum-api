using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtorDebtAndReminderSentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "debtors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    public_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debtors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "debts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    debtor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    public_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debts", x => x.id);
                    table.ForeignKey(
                        name: "FK_debts_debtors_debtor_id",
                        column: x => x.debtor_id,
                        principalTable: "debtors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reminders_sent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    debt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    public_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminders_sent", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminders_sent_debts_debt_id",
                        column: x => x.debt_id,
                        principalTable: "debts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_debtors_customer_id",
                table: "debtors",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_debtors_id_created_at",
                table: "debtors",
                columns: new[] { "id", "created_at" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_debtors_is_deleted",
                table: "debtors",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_debtors_public_id",
                table: "debtors",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_debts_debtor_id",
                table: "debts",
                column: "debtor_id");

            migrationBuilder.CreateIndex(
                name: "IX_debts_due_date",
                table: "debts",
                column: "due_date");

            migrationBuilder.CreateIndex(
                name: "IX_debts_id_created_at",
                table: "debts",
                columns: new[] { "id", "created_at" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_debts_is_paid",
                table: "debts",
                column: "is_paid");

            migrationBuilder.CreateIndex(
                name: "IX_debts_public_id",
                table: "debts",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reminders_sent_debt_id",
                table: "reminders_sent",
                column: "debt_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminders_sent_id_created_at",
                table: "reminders_sent",
                columns: new[] { "id", "created_at" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_reminders_sent_public_id",
                table: "reminders_sent",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reminders_sent_sent_at",
                table: "reminders_sent",
                column: "sent_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reminders_sent");

            migrationBuilder.DropTable(
                name: "debts");

            migrationBuilder.DropTable(
                name: "debtors");
        }
    }
}
