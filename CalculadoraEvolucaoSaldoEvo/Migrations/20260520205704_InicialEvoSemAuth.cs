using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculadoraEvolucaoSaldoEvo.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class InicialEvoSemAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsSimulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SimulacaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Acao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsSimulacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ValorInicial = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TaxaJurosMensal = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    PrazoMeses = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotalFinal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalJuros = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Deletado = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletadoEmUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    ModificadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ModificadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Evolucoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SimulacaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    SaldoInicial = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Juro = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SaldoFinal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evolucoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evolucoes_Simulacoes_SimulacaoId",
                        column: x => x.SimulacaoId,
                        principalTable: "Simulacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evolucoes_SimulacaoId",
                table: "Evolucoes",
                column: "SimulacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evolucoes");

            migrationBuilder.DropTable(
                name: "LogsSimulacoes");

            migrationBuilder.DropTable(
                name: "Simulacoes");
        }
    }
}
