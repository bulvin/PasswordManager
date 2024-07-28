using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManager.Migrations
{
    /// <inheritdoc />
    public partial class wtf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "password_manager");

            migrationBuilder.CreateTable(
                name: "Passes",
                schema: "password_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    website_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IV = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_passes_uuid_id", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passes",
                schema: "password_manager");
        }
    }
}
