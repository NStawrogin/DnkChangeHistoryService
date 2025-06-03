using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnkChangeHistoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CommentHistoryAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContextId",
                schema: "history",
                table: "CommentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisclosureRequirementId",
                schema: "history",
                table: "CommentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                schema: "history",
                table: "CommentHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextId",
                schema: "history",
                table: "CommentHistories");

            migrationBuilder.DropColumn(
                name: "DisclosureRequirementId",
                schema: "history",
                table: "CommentHistories");

            migrationBuilder.DropColumn(
                name: "ReportId",
                schema: "history",
                table: "CommentHistories");
        }
    }
}
