using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DishId",
                table: "DishSnapshots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DishSnapshots_DishId",
                table: "DishSnapshots",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_DishSnapshots_DishId",
                table: "DishSnapshots");

            migrationBuilder.AlterColumn<int>(
                name: "DishId",
                table: "DishSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
