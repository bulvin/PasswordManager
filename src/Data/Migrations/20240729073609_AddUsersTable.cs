using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Passes",
                schema: "password_manager",
                newName: "passes",
                newSchema: "password_manager");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "password_manager",
                table: "passes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "users",
                schema: "password_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_passes_UserId",
                schema: "password_manager",
                table: "passes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                schema: "password_manager",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_passes_users_UserId",
                schema: "password_manager",
                table: "passes",
                column: "UserId",
                principalSchema: "password_manager",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_passes_users_UserId",
                schema: "password_manager",
                table: "passes");

            migrationBuilder.DropTable(
                name: "users",
                schema: "password_manager");

            migrationBuilder.DropIndex(
                name: "IX_passes_UserId",
                schema: "password_manager",
                table: "passes");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "password_manager",
                table: "passes");

            migrationBuilder.RenameTable(
                name: "passes",
                schema: "password_manager",
                newName: "Passes",
                newSchema: "password_manager");
        }
    }
}
