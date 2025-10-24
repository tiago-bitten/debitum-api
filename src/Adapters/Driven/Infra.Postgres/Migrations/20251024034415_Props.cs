using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Props : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE customers
                ALTER COLUMN id TYPE uuid USING id::uuid;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_customers_id_created_at",
                table: "customers",
                columns: new[] { "id", "created_at" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customers_id_created_at",
                table: "customers");

            migrationBuilder.Sql(@"
                ALTER TABLE customers
                ALTER COLUMN id TYPE text USING id::text;
            ");
        }
    }
}
