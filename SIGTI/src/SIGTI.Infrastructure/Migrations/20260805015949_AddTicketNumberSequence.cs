using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "ticket_number_sequence", startValue: 1L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "ticket_number_sequence");
        }
    }
}
