using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    /// <inheritdoc />
    public partial class UpdEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TestTimeEntities",
            //    table: "TestTimeEntities");

            //migrationBuilder.RenameTable(
            //    name: "TestTimeEntities",
            //    newName: "TestTimeEntity");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TestTimeEntity",
            //    table: "TestTimeEntity",
            //    column: "Id");

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hobbies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetails_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.UpdateData(
            //    table: "TestTimeEntity",
            //    keyColumn: "Id",
            //    keyValue: 1,
            //    columns: new[] { "FullTime", "TimeOffSet" },
            //    values: new object[] { new DateTime(2024, 3, 2, 18, 45, 58, 403, DateTimeKind.Local).AddTicks(872), new DateTimeOffset(new DateTime(2024, 3, 2, 18, 45, 58, 403, DateTimeKind.Unspecified).AddTicks(965), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_ApplicationUserId",
                table: "UserDetails",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestTimeEntity",
                table: "TestTimeEntity");

            migrationBuilder.RenameTable(
                name: "TestTimeEntity",
                newName: "TestTimeEntities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestTimeEntities",
                table: "TestTimeEntities",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 2, 29, 21, 40, 41, 613, DateTimeKind.Local).AddTicks(4555), new DateTimeOffset(new DateTime(2024, 2, 29, 21, 40, 41, 613, DateTimeKind.Unspecified).AddTicks(4646), new TimeSpan(0, 2, 0, 0, 0)) });
        }
    }
}
