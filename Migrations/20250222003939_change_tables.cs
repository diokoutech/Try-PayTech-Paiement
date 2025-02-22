using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestPayTech.Migrations
{
    /// <inheritdoc />
    public partial class change_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paiements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MethodePaiement = table.Column<string>(type: "TEXT", nullable: true),
                    MontantTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    Reference = table.Column<string>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: true),
                    IdDossier = table.Column<string>(type: "TEXT", nullable: false),
                    NomProduitPaiement = table.Column<string>(type: "TEXT", nullable: false),
                    TelephoneClient = table.Column<string>(type: "TEXT", nullable: true),
                    StatutPaiement = table.Column<string>(type: "TEXT", nullable: true),
                    DateModificationWebhook = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paiements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paiements_IdDossier",
                table: "Paiements",
                column: "IdDossier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paiements");
        }
    }
}
