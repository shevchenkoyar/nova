using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nova.Modules.Relationships.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "relationships");

            migrationBuilder.CreateTable(
                name: "profiles",
                schema: "relationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Trust = table.Column<int>(type: "integer", nullable: false),
                    Warmth = table.Column<int>(type: "integer", nullable: false),
                    Respect = table.Column<int>(type: "integer", nullable: false),
                    Familiarity = table.Column<int>(type: "integer", nullable: false),
                    Annoyance = table.Column<int>(type: "integer", nullable: false),
                    OffenseScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profiles_PersonId",
                schema: "relationships",
                table: "profiles",
                column: "PersonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profiles",
                schema: "relationships");
        }
    }
}
