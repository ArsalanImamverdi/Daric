using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Daric.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Bonus_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bonuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_Id",
                table: "Bonuses",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_Type",
                table: "Bonuses",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bonuses");
        }
    }
}
