using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    /// <inheritdoc />
    public partial class UpdEnt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "MessageGroups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.UpdateData(
            //    table: "TestTimeEntity",
            //    keyColumn: "Id",
            //    keyValue: 1,
            //    columns: new[] { "FullTime", "TimeOffSet" },
            //    values: new object[] { new DateTime(2024, 3, 2, 22, 57, 8, 46, DateTimeKind.Local).AddTicks(6137), new DateTimeOffset(new DateTime(2024, 3, 2, 22, 57, 8, 46, DateTimeKind.Unspecified).AddTicks(6221), new TimeSpan(0, 2, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "MessageGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "TestTimeEntity",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 3, 2, 18, 45, 58, 403, DateTimeKind.Local).AddTicks(872), new DateTimeOffset(new DateTime(2024, 3, 2, 18, 45, 58, 403, DateTimeKind.Unspecified).AddTicks(965), new TimeSpan(0, 2, 0, 0, 0)) });
        }
    }
}
