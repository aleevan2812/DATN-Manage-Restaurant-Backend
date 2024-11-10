using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changesomething : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Accounts_OrderHandlerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DishSnapshots_DishSnapshotId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Guests_GuestId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tables_TableNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TableNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_DishSnapshots_DishId",
                table: "DishSnapshots");

            migrationBuilder.AlterColumn<int>(
                name: "TableNumber",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderHandlerId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GuestId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DishSnapshotId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Accounts_OrderHandlerId",
                table: "Orders",
                column: "OrderHandlerId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DishSnapshots_DishSnapshotId",
                table: "Orders",
                column: "DishSnapshotId",
                principalTable: "DishSnapshots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Guests_GuestId",
                table: "Orders",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Accounts_OrderHandlerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DishSnapshots_DishSnapshotId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Guests_GuestId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "TableNumber",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderHandlerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GuestId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DishSnapshotId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableNumber",
                table: "Orders",
                column: "TableNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DishSnapshots_DishId",
                table: "DishSnapshots",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishSnapshots_Dishes_DishId",
                table: "DishSnapshots",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Accounts_OrderHandlerId",
                table: "Orders",
                column: "OrderHandlerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DishSnapshots_DishSnapshotId",
                table: "Orders",
                column: "DishSnapshotId",
                principalTable: "DishSnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Guests_GuestId",
                table: "Orders",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tables_TableNumber",
                table: "Orders",
                column: "TableNumber",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
