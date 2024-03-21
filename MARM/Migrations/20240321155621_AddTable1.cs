using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MARM.Migrations
{
    /// <inheritdoc />
    public partial class AddTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Baudrate = table.Column<int>(type: "INT", nullable: false),
                    Light1Mode = table.Column<int>(type: "INT", nullable: false),
                    Light2Mode = table.Column<int>(type: "INT", nullable: false),
                    Light3Mode = table.Column<int>(type: "INT", nullable: false),
                    Light4Mode = table.Column<int>(type: "INT", nullable: false),
                    Port = table.Column<string>(type: "TEXT", nullable: false),
                    TimerInterval = table.Column<int>(type: "INT", nullable: false),
                    Transmit1 = table.Column<bool>(type: "BOOL", nullable: false),
                    Transmit2 = table.Column<bool>(type: "BOOL", nullable: false),
                    Transmit3 = table.Column<bool>(type: "BOOL", nullable: false),
                    Transmit4 = table.Column<bool>(type: "BOOL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigs", x => x.Id);
                });
        }
    }
}
