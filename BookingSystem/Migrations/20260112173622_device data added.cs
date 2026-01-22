using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class devicedataadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Category", "Condition", "CreatedAt", "CreatedBy", "Description", "Name", "SerialNumber", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Laptop", "Neuwertig", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "Max Mustermann", "Laptop für Entwicklung und Design", "MacBook Pro 16\"", "MBP-2024-001", "Verfügbar", null },
                    { 2, "Smartphone", "Gut", new DateTime(2024, 2, 10, 14, 15, 0, 0, DateTimeKind.Unspecified), "Anna Schmidt", "Smartphone für Mobiltests", "iPhone 15 Pro", "IPH-2024-042", "Ausgeliehen", new DateTime(2024, 12, 20, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Peripherie", "Gebraucht", new DateTime(2023, 11, 5, 8, 45, 0, 0, DateTimeKind.Unspecified), "Thomas Weber", "Kabellose ergonomische Maus", "Logitech MX Master 3", "LOG-MX3-789", "Verfügbar", new DateTime(2025, 1, 5, 11, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Monitor", "Sehr gut", new DateTime(2024, 3, 22, 16, 0, 0, 0, DateTimeKind.Unspecified), "Lisa Müller", "4K Monitor für Grafikdesign", "Dell UltraSharp 27\" Monitor", "DELL-US27-456", "In Wartung", new DateTime(2025, 1, 8, 13, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Tablet", "Neuwertig", new DateTime(2024, 6, 18, 11, 10, 0, 0, DateTimeKind.Unspecified), "Michael Schneider", "Tablet für Präsentationen und Notizen", "iPad Air 2024", "IPAD-AIR-223", "Verfügbar", null },
                    { 6, "Audio", "Gut", new DateTime(2023, 9, 30, 9, 25, 0, 0, DateTimeKind.Unspecified), "Sarah Fischer", "Noise-Cancelling Kopfhörer", "Sony WH-1000XM5", "SONY-WH-1000-XM5-112", "Defekt", new DateTime(2025, 1, 10, 10, 45, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
