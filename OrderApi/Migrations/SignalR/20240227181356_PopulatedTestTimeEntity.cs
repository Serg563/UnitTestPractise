using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    /// <inheritdoc />
    public partial class PopulatedTestTimeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestTimeEntities",
                columns: new[] { "Id", "Date", "FullTime", "Span", "Time", "TimeOffSet" },
                values: new object[] { 1, new DateTime(2024, 2, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 27, 20, 13, 56, 405, DateTimeKind.Local).AddTicks(7325), new TimeSpan(0, 2, 0, 0, 0), new TimeSpan(0, 10, 30, 0, 0), new DateTimeOffset(new DateTime(2024, 2, 27, 20, 13, 56, 405, DateTimeKind.Unspecified).AddTicks(7392), new TimeSpan(0, 2, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestTimeEntities",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
