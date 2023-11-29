using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Asuntos",
                columns: table => new
                {
                    IdAsunto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asuntos", x => x.IdAsunto);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NivelesEducativos",
                columns: table => new
                {
                    IdNivelEducativo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesEducativos", x => x.IdNivelEducativo);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    IdPais = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombrePais = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.IdPais);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoUsuario = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreCompleto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TelefonoUsuario = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreEstado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdPais = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.IdEstado);
                    table.ForeignKey(
                        name: "FK_Estados_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "IdPais",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    IdMunicipio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreMunicipio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.IdMunicipio);
                    table.ForeignKey(
                        name: "FK_Municipios_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Domicilios",
                columns: table => new
                {
                    IdDomicilio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Calle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Colonia = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdMunicipio = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdPais = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domicilios", x => x.IdDomicilio);
                    table.ForeignKey(
                        name: "FK_Domicilios_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Domicilios_Municipios_IdMunicipio",
                        column: x => x.IdMunicipio,
                        principalTable: "Municipios",
                        principalColumn: "IdMunicipio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Domicilios_Paises_IdPais",
                        column: x => x.IdPais,
                        principalTable: "Paises",
                        principalColumn: "IdPais",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    CURP = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Paterno = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Materno = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdNivelEducativo = table.Column<int>(type: "int", nullable: true),
                    IdDomicilio = table.Column<int>(type: "int", nullable: true),
                    TelefonoEstudiante = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.CURP);
                    table.ForeignKey(
                        name: "FK_Estudiantes_Domicilios_IdDomicilio",
                        column: x => x.IdDomicilio,
                        principalTable: "Domicilios",
                        principalColumn: "IdDomicilio");
                    table.ForeignKey(
                        name: "FK_Estudiantes_NivelesEducativos_IdNivelEducativo",
                        column: x => x.IdNivelEducativo,
                        principalTable: "NivelesEducativos",
                        principalColumn: "IdNivelEducativo");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Folio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CURP = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdAsunto = table.Column<int>(type: "int", nullable: true),
                    Estatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroTurno = table.Column<int>(type: "int", nullable: true),
                    FechaRegistroTramite = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FechaDeTramite = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IdMunicipio = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Folio);
                    table.ForeignKey(
                        name: "FK_Tickets_Asuntos_IdAsunto",
                        column: x => x.IdAsunto,
                        principalTable: "Asuntos",
                        principalColumn: "IdAsunto");
                    table.ForeignKey(
                        name: "FK_Tickets_Estudiantes_CURP",
                        column: x => x.CURP,
                        principalTable: "Estudiantes",
                        principalColumn: "CURP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Municipios_IdMunicipio",
                        column: x => x.IdMunicipio,
                        principalTable: "Municipios",
                        principalColumn: "IdMunicipio");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilios_IdEstado",
                table: "Domicilios",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilios_IdMunicipio",
                table: "Domicilios",
                column: "IdMunicipio");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilios_IdPais",
                table: "Domicilios",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Estados_IdPais",
                table: "Estados",
                column: "IdPais");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdDomicilio",
                table: "Estudiantes",
                column: "IdDomicilio");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_IdNivelEducativo",
                table: "Estudiantes",
                column: "IdNivelEducativo");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_IdEstado",
                table: "Municipios",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CURP",
                table: "Tickets",
                column: "CURP");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdAsunto",
                table: "Tickets",
                column: "IdAsunto");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdMunicipio",
                table: "Tickets",
                column: "IdMunicipio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Asuntos");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Domicilios");

            migrationBuilder.DropTable(
                name: "NivelesEducativos");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Paises");
        }
    }
}
