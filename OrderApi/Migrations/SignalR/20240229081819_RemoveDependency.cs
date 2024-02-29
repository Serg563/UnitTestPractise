using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    /// <inheritdoc />
    public partial class RemoveDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.UpdateData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 2, 29, 10, 18, 19, 590, DateTimeKind.Local).AddTicks(3945), new DateTimeOffset(new DateTime(2024, 2, 29, 10, 18, 19, 590, DateTimeKind.Unspecified).AddTicks(4046), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_AspNetUsers_UserId",
                table: "Connections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_AspNetUsers_UserId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 2, 29, 10, 10, 41, 972, DateTimeKind.Local).AddTicks(5403), new DateTimeOffset(new DateTime(2024, 2, 29, 10, 10, 41, 972, DateTimeKind.Unspecified).AddTicks(5505), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
