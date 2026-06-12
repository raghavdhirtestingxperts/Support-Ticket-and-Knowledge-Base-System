using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Body", "Tags", "Title" },
                values: new object[] { "Click 'Forgot Password' on the login page, type in your email address, and click the link in the reset email. The link is valid for 24 hours.", "password,reset", "Resetting your password" });

            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Body", "Tags", "Title" },
                values: new object[] { "Our standard response times are: Critical - 2 hours, High - 8 hours, Medium - 24 hours, Low - 72 hours.", "sla,priority", "SLA response times" });

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Note",
                value: "Assigned to Bob. Checking payment gateway logs.");

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Note",
                value: "Investigating email logs.");

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Note",
                value: "Manually triggered reset link.");

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Tried logging in multiple times but it just says 401 unauthorized. I'm sure the password is correct.", "Login fails with 401 error" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Checked my card statement and I was billed twice this month. Only one order confirmation received.", "Charged twice for subscription" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Requested a password reset but the email never arrived in my inbox or spam.", "Forgot password reset link not arriving" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Is there any way to turn on dark mode? The white screen is a bit too bright for night use.", "Dark mode support?" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Clicking the export button does nothing. Tried on Chrome and Safari, no file downloads.", "CSV export button not working" });

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
                column: "Name",
                value: "Bob Jones");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Carol Davis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Body", "Tags", "Title" },
                values: new object[] { "To reset your password, navigate to the login page and click 'Forgot Password'. Enter your registered email address and check your inbox for the reset link. The link is valid for 24 hours.", "password,reset,login,account", "How to Reset Your Password" });

            migrationBuilder.UpdateData(
                table: "KnowledgeBaseArticles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Body", "Tags", "Title" },
                values: new object[] { "SLA (Service Level Agreement) defines the time within which a ticket must be resolved. Critical tickets: 2 hours, High: 8 hours, Medium: 24 hours, Low: 72 hours. SLA is calculated from the ticket creation time.", "sla,priority,ticket,response-time", "Understanding SLA and Ticket Priorities" });

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Note",
                value: "Assigned to agent and work started.");

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Note",
                value: "Agent picked up ticket.");

            migrationBuilder.UpdateData(
                table: "TicketHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Note",
                value: "Password reset link sent successfully.");

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "I keep getting 401 errors when I try to login with my credentials.", "Cannot login to portal" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Title" },
                values: new object[] { "My payment was deducted but the order did not get confirmed.", "Payment not processed" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "I forgot my password and the reset email is not arriving.", "How do I reset my password?" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Would love to have a dark mode option in the application.", "Feature request: Dark mode" });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Title" },
                values: new object[] { "The CSV export button does nothing when I click it.", "Data export not working" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Alice Customer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Bob Agent");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Carol Supervisor");
        }
    }
}
