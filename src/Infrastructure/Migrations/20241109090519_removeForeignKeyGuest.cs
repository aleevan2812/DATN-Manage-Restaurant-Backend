using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeForeignKeyGuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Tables_TableNumber",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_TableNumber",
                table: "Guests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Guests_TableNumber",
                table: "Guests",
                column: "TableNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Tables_TableNumber",
                table: "Guests",
                column: "TableNumber",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
