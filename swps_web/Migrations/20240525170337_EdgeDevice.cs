using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace swps_web.Migrations
{
    /// <inheritdoc />
    public partial class EdgeDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "DeviceSNIndex",
                table: "AspNetUsers",
                newName: "NormalizedDeviceSNIndex");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "SensorRecords",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EdgeDevices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    DeviceSN = table.Column<string>(type: "varchar(255)", nullable: false),
                    DetectInterval = table.Column<int>(type: "int", nullable: false),
                    PumpStartTime = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    SoilMoisture = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdgeDevices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SensorRecords_DeviceId",
                table: "SensorRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "DeviceSNIndex",
                table: "EdgeDevices",
                column: "DeviceSN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EdgeDevices_UserId",
                table: "EdgeDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorRecords_EdgeDevices_DeviceId",
                table: "SensorRecords",
                column: "DeviceId",
                principalTable: "EdgeDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorRecords_EdgeDevices_DeviceId",
                table: "SensorRecords");

            migrationBuilder.DropTable(
                name: "EdgeDevices");

            migrationBuilder.DropIndex(
                name: "IX_SensorRecords_DeviceId",
                table: "SensorRecords");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "SensorRecords");

            migrationBuilder.RenameIndex(
                name: "NormalizedDeviceSNIndex",
                table: "AspNetUsers",
                newName: "DeviceSNIndex");
        }
    }
}
