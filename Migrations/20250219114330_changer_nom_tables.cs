using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestPayTech.Migrations
{
    /// <inheritdoc />
    public partial class changer_nom_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Paiements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paiements",
                table: "Paiements",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Paiements",
                table: "Paiements");

            migrationBuilder.RenameTable(
                name: "Paiements",
                newName: "Items");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");
        }
    }
}
