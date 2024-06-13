using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace swps_web.Migrations
{
    /// <inheritdoc />
    public partial class SensorRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    Humidity = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    RawValue0 = table.Column<int>(type: "int", nullable: false),
                    RawValue1 = table.Column<int>(type: "int", nullable: false),
                    RawValue2 = table.Column<int>(type: "int", nullable: false),
                    RawValue3 = table.Column<int>(type: "int", nullable: false),
                    Voltage0 = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Voltage1 = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Voltage2 = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Voltage3 = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    DetectTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PumpStartTime = table.Column<decimal>(type: "decimal(6,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "DetectTimeIndex",
                table: "SensorRecords",
                column: "DetectTime");

            migrationBuilder.CreateIndex(
                name: "IX_SensorRecords_UserId",
                table: "SensorRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorRecords");
        }
    }
}
