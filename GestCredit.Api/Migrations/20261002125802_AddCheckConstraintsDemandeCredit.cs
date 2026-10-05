using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestCredit.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintsDemandeCredit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_DemandeCredit_Duree",
                table: "DemandeCredit",
                sql: "[DureeMois] BETWEEN 1 AND 360");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DemandeCredit_Montant",
                table: "DemandeCredit",
                sql: "[Montant] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DemandeCredit_Statut",
                table: "DemandeCredit",
                sql: "[Statut] IN ('Brouillon', 'Soumise', 'EnAnalyse', 'Approuvee', 'Rejetee')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DemandeCredit_Duree",
                table: "DemandeCredit");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DemandeCredit_Montant",
                table: "DemandeCredit");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DemandeCredit_Statut",
                table: "DemandeCredit");
        }
    }
}
