using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelemetryApiRest.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deviceRecords_devices_Id",
                table: "deviceRecords");

            migrationBuilder.DropIndex(
                name: "IX_devices_SerialNumber",
                table: "devices");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "deviceRecords",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "deviceId",
                table: "deviceRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_deviceRecords_deviceId",
                table: "deviceRecords",
                column: "deviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_deviceRecords_devices_deviceId",
                table: "deviceRecords",
                column: "deviceId",
                principalTable: "devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deviceRecords_devices_deviceId",
                table: "deviceRecords");

            migrationBuilder.DropIndex(
                name: "IX_deviceRecords_deviceId",
                table: "deviceRecords");

            migrationBuilder.DropColumn(
                name: "deviceId",
                table: "deviceRecords");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "deviceRecords",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_devices_SerialNumber",
                table: "devices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_deviceRecords_devices_Id",
                table: "deviceRecords",
                column: "Id",
                principalTable: "devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
