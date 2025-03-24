using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Daric.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Account_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "Account.AccountNumber",
                startValue: 1000000001L);

            migrationBuilder.CreateSequence(
                name: "Account.TrackingCode",
                startValue: 1000L);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    AccountNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Id",
                table: "Accounts",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropSequence(
                name: "Account.AccountNumber");

            migrationBuilder.DropSequence(
                name: "Account.TrackingCode");
        }
    }
}
