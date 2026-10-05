using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestCredit.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUtilisateurRoleAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomUtilisateur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UtilisateurRole",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    UtilisateursId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilisateurRole", x => new { x.RolesId, x.UtilisateursId });
                    table.ForeignKey(
                        name: "FK_UtilisateurRole_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilisateurRole_Utilisateur_UtilisateursId",
                        column: x => x.UtilisateursId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Agent" },
                    { 2, "Analyste" },
                    { 3, "Administrateur" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Role_Nom",
                table: "Role",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_NomUtilisateur",
                table: "Utilisateur",
                column: "NomUtilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurRole_UtilisateursId",
                table: "UtilisateurRole",
                column: "UtilisateursId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UtilisateurRole");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}
