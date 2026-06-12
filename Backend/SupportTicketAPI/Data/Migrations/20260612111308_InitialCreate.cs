using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportTicketAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<string>(type: "longtext", nullable: false),
                    TenantId = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KnowledgeBaseArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Body = table.Column<string>(type: "longtext", nullable: false),
                    Tags = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBaseArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeBaseArticles_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false),
                    Priority = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SlaDeadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsSlaBreached = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComments_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OldStatus = table.Column<string>(type: "longtext", nullable: true),
                    NewStatus = table.Column<string>(type: "longtext", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Role", "TenantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "alice@demo.com", "Alice Customer", "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK", "Customer", "default" },
                    { 2, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "bob@demo.com", "Bob Agent", "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK", "Agent", "default" },
                    { 3, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "carol@demo.com", "Carol Supervisor", "$2a$11$EjA4/XEdoTEun6S.mVWcCesE7GlBuztGkkPZiLiRWmWKoesKFETtK", "Supervisor", "default" }
                });

            migrationBuilder.InsertData(
                table: "KnowledgeBaseArticles",
                columns: new[] { "Id", "Body", "CreatedAt", "CreatedByUserId", "Tags", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "To reset your password, navigate to the login page and click 'Forgot Password'. Enter your registered email address and check your inbox for the reset link. The link is valid for 24 hours.", new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc), 2, "password,reset,login,account", "How to Reset Your Password", null },
                    { 2, "SLA (Service Level Agreement) defines the time within which a ticket must be resolved. Critical tickets: 2 hours, High: 8 hours, Medium: 24 hours, Low: 72 hours. SLA is calculated from the ticket creation time.", new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), 2, "sla,priority,ticket,response-time", "Understanding SLA and Ticket Priorities", null }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "CreatedByUserId", "Description", "IsSlaBreached", "Priority", "SlaDeadline", "Status", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), 1, "I keep getting 401 errors when I try to login with my credentials.", false, "High", new DateTime(2026, 6, 12, 8, 0, 0, 0, DateTimeKind.Utc), "Open", "Cannot login to portal" },
                    { 2, 2, new DateTime(2026, 6, 11, 21, 0, 0, 0, DateTimeKind.Utc), 1, "My payment was deducted but the order did not get confirmed.", true, "Critical", new DateTime(2026, 6, 11, 23, 0, 0, 0, DateTimeKind.Utc), "InProgress", "Payment not processed" },
                    { 3, 2, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, "I forgot my password and the reset email is not arriving.", false, "Low", new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Resolved", "How do I reset my password?" },
                    { 4, 2, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Would love to have a dark mode option in the application.", false, "Medium", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), "PendingCustomer", "Feature request: Dark mode" },
                    { 5, null, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), 1, "The CSV export button does nothing when I click it.", false, "Medium", new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Open", "Data export not working" }
                });

            migrationBuilder.InsertData(
                table: "TicketHistories",
                columns: new[] { "Id", "ChangedAt", "ChangedByUserId", "NewStatus", "Note", "OldStatus", "TicketId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 11, 22, 0, 0, 0, DateTimeKind.Utc), 2, "InProgress", "Assigned to agent and work started.", "Open", 2 },
                    { 2, new DateTime(2026, 6, 10, 1, 0, 0, 0, DateTimeKind.Utc), 2, "InProgress", "Agent picked up ticket.", "Open", 3 },
                    { 3, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Resolved", "Password reset link sent successfully.", "InProgress", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBaseArticles_CreatedByUserId",
                table: "KnowledgeBaseArticles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketId",
                table: "TicketComments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_UserId",
                table: "TicketComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_ChangedByUserId",
                table: "TicketHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_TicketId",
                table: "TicketHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToUserId",
                table: "Tickets",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedByUserId",
                table: "Tickets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgeBaseArticles");

            migrationBuilder.DropTable(
                name: "TicketComments");

            migrationBuilder.DropTable(
                name: "TicketHistories");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
