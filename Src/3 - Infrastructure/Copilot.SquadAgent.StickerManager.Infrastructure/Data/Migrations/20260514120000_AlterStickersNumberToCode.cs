using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterStickersNumberToCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE stickers RENAME COLUMN number TO code;");

            migrationBuilder.Sql("ALTER TABLE stickers ALTER COLUMN code TYPE VARCHAR(32) USING code::VARCHAR;");

            migrationBuilder.AddUniqueConstraint(
                name: "uq_stickers_code",
                table: "stickers",
                column: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "uq_stickers_code",
                table: "stickers");

            migrationBuilder.Sql("ALTER TABLE stickers ALTER COLUMN code TYPE INTEGER USING code::INTEGER;");

            migrationBuilder.Sql("ALTER TABLE stickers RENAME COLUMN code TO number;");
        }
    }
}
