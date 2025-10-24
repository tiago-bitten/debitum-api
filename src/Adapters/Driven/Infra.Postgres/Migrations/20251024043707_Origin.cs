using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Origin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "origem",
                table: "customers",
                newName: "origin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "origin",
                table: "customers",
                newName: "origem");
        }
    }
}
