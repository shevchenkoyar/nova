using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nova.Modules.HomeAssistant.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "home_assistant");

            migrationBuilder.CreateTable(
                name: "entities",
                schema: "home_assistant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Domain = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    State = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FriendlyName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    DeviceClass = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    UnitOfMeasurement = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Area = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AttributesJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_entities_Domain_IsDeleted",
                schema: "home_assistant",
                table: "entities",
                columns: new[] { "Domain", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_entities_EntityId",
                schema: "home_assistant",
                table: "entities",
                column: "EntityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entities",
                schema: "home_assistant");
        }
    }
}
