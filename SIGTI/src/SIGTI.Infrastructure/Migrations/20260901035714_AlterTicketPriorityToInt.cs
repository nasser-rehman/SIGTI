using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGTI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTicketPriorityToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE ""Tickets"" 
                ALTER COLUMN ""Priority"" TYPE integer 
                USING CASE 
                    WHEN ""Priority"" = 'Low' THEN 1
                    WHEN ""Priority"" = 'Medium' THEN 2
                    WHEN ""Priority"" = 'High' THEN 3
                    WHEN ""Priority"" = 'Critical' THEN 4
                    ELSE 2
                END;
            "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE ""Tickets"" 
                ALTER COLUMN ""Priority"" TYPE character varying(30) 
                USING CASE 
                    WHEN ""Priority"" = 1 THEN 'Low'
                    WHEN ""Priority"" = 2 THEN 'Medium'
                    WHEN ""Priority"" = 3 THEN 'High'
                    WHEN ""Priority"" = 4 THEN 'Critical'
                    ELSE 'Medium'
                END;
            "
            );
        }
    }
}
