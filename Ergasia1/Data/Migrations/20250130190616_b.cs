using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergasia1.Data.Migrations
{
    /// <inheritdoc />
    public partial class b : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_ContactId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactId_UserId",
                table: "Contacts",
                columns: new[] { "ContactId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId_ContactId",
                table: "Contacts",
                columns: new[] { "UserId", "ContactId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_ContactId_UserId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId_ContactId",
                table: "Contacts");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactId",
                table: "Contacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");
        }
    }
}
