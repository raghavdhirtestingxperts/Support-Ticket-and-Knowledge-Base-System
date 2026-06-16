using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportTicketAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedByUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedByUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "ChangedByUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "ChangedByUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "ChangedByUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssignedToUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssignedToUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 4,
                column: "AssignedToUserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Alice");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "Role" },
                values: new object[] { "banni@gmail.com", "Banni", "Customer" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "Role" },
                values: new object[] { "bob@demo.com", "Bob", "Agent" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "TenantId" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "manav@gmail.com", "Manav", "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK", "Customer", "default" },
                    { 5, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "carol@demo.com", "Carol", "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK", "Supervisor", "default" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedByUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedByUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "ChangedByUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "ChangedByUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "ChangedByUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssignedToUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssignedToUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 4,
                column: "AssignedToUserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Alice Smith");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "Role" },
                values: new object[] { "bob@demo.com", "Bob Jones", "Agent" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "Role" },
                values: new object[] { "carol@demo.com", "Carol Davis", "Supervisor" });
        }
    }
}
