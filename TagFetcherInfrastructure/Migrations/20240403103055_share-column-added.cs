using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagFetcherInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sharecolumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Share",
                table: "Tags",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Share",
                table: "Tags");
        }
    }
}
