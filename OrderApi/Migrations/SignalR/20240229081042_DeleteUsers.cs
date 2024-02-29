using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    /// <inheritdoc />
    public partial class DeleteUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 2, 29, 10, 10, 41, 972, DateTimeKind.Local).AddTicks(5403), new DateTimeOffset(new DateTime(2024, 2, 29, 10, 10, 41, 972, DateTimeKind.Unspecified).AddTicks(5505), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.Sql("DROP TABLE Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullTime", "TimeOffSet" },
                values: new object[] { new DateTime(2024, 2, 28, 12, 32, 28, 932, DateTimeKind.Local).AddTicks(2143), new DateTimeOffset(new DateTime(2024, 2, 28, 12, 32, 28, 932, DateTimeKind.Unspecified).AddTicks(2227), new TimeSpan(0, 2, 0, 0, 0)) });
        }
    }
}
