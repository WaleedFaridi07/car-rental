using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarRentalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarCategoryConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    BaseDayRental = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BaseKmPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarCategoryConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    BookingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerSocialSecurityNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CarCategoryId = table.Column<int>(type: "integer", nullable: false),
                    PickupDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PickupMeterReading = table.Column<int>(type: "integer", nullable: false),
                    ReturnDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReturnMeterReading = table.Column<int>(type: "integer", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.BookingNumber);
                    table.ForeignKey(
                        name: "FK_Rentals_CarCategoryConfigs_CarCategoryId",
                        column: x => x.CarCategoryId,
                        principalTable: "CarCategoryConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CarCategoryConfigs",
                columns: new[] { "Id", "BaseDayRental", "BaseKmPrice", "Category" },
                values: new object[,]
                {
                    { 1, 300m, 0m, 1 },
                    { 2, 500m, 2m, 2 },
                    { 3, 800m, 3m, 3 }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "BookingNumber", "CarCategoryId", "CustomerSocialSecurityNumber", "PickupDateTime", "PickupMeterReading", "RegistrationNumber", "ReturnDateTime", "ReturnMeterReading", "TotalPrice" },
                values: new object[,]
                {
                    { "BK0001", 1, "SSN123456", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "REG123", null, null, 350.00m },
                    { "BK0002", 2, "SSN654321", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "REG456", null, null, 520.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CarCategoryId",
                table: "Rentals",
                column: "CarCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "CarCategoryConfigs");
        }
    }
}
