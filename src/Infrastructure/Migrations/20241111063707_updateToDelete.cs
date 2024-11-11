using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateToDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots");

            migrationBuilder.AddForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots");

            migrationBuilder.AddForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id");
        }
    }
}
