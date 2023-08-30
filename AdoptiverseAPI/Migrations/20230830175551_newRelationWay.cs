using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptiverseAPI.Migrations
{
    /// <inheritdoc />
    public partial class newRelationWay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pets_shelters_shelter_id",
                table: "pets");

            migrationBuilder.DropIndex(
                name: "ix_pets_shelter_id",
                table: "pets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_pets_shelter_id",
                table: "pets",
                column: "shelter_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pets_shelters_shelter_id",
                table: "pets",
                column: "shelter_id",
                principalTable: "shelters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
