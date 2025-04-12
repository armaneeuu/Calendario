using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Calendario.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_especifico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomEsp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_especifico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_global",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomGlo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_global", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_respacademico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomAcad = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_respacademico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_respoperador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomOpe = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_respoperador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_principal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GlobalId = table.Column<int>(type: "integer", nullable: false),
                    EspecificoId = table.Column<int>(type: "integer", nullable: false),
                    RespAcademicoId = table.Column<int>(type: "integer", nullable: false),
                    RespOperadorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_principal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_principal_t_especifico_EspecificoId",
                        column: x => x.EspecificoId,
                        principalTable: "t_especifico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_principal_t_global_GlobalId",
                        column: x => x.GlobalId,
                        principalTable: "t_global",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_principal_t_respacademico_RespAcademicoId",
                        column: x => x.RespAcademicoId,
                        principalTable: "t_respacademico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_principal_t_respoperador_RespOperadorId",
                        column: x => x.RespOperadorId,
                        principalTable: "t_respoperador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_ambientea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    PrincipalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_ambientea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_ambientea_t_principal_PrincipalId",
                        column: x => x.PrincipalId,
                        principalTable: "t_principal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_ambientea_PrincipalId",
                table: "t_ambientea",
                column: "PrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_t_principal_EspecificoId",
                table: "t_principal",
                column: "EspecificoId");

            migrationBuilder.CreateIndex(
                name: "IX_t_principal_GlobalId",
                table: "t_principal",
                column: "GlobalId");

            migrationBuilder.CreateIndex(
                name: "IX_t_principal_RespAcademicoId",
                table: "t_principal",
                column: "RespAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_t_principal_RespOperadorId",
                table: "t_principal",
                column: "RespOperadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_ambientea");

            migrationBuilder.DropTable(
                name: "t_principal");

            migrationBuilder.DropTable(
                name: "t_especifico");

            migrationBuilder.DropTable(
                name: "t_global");

            migrationBuilder.DropTable(
                name: "t_respacademico");

            migrationBuilder.DropTable(
                name: "t_respoperador");
        }
    }
}
