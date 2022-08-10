using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApotheGSF.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblFacturas",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubTotal = table.Column<float>(type: "real", nullable: false),
                    Total = table.Column<float>(type: "real", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFacturas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "tblMedicamentos",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sustancia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concentracion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadesCaja = table.Column<int>(type: "int", nullable: false),
                    Costo = table.Column<float>(type: "real", nullable: false),
                    PrecioUnidad = table.Column<float>(type: "real", nullable: false),
                    Indicaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: true),
                    EnvioPendiente = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMedicamentos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "tblProveedores",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RNC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminosdePago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProveedores", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "tblRoles",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarios",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    Foto = table.Column<string>(type: "varchar(50)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NombreUsuarioNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConfirmarEmail = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmarTelefono = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarios", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "tblMedicamentosCajas",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoMedicamento = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidad = table.Column<int>(type: "int", nullable: false),
                    FechaAdquirido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detallada = table.Column<bool>(type: "bit", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMedicamentosCajas", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_tblMedicamentosCajas_tblMedicamentos_CodigoMedicamento",
                        column: x => x.CodigoMedicamento,
                        principalTable: "tblMedicamentos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblProveedoresMedicamentos",
                columns: table => new
                {
                    CodigoProveedor = table.Column<int>(type: "int", nullable: false),
                    CodigoMedicamento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProveedoresMedicamentos", x => new { x.CodigoMedicamento, x.CodigoProveedor });
                    table.ForeignKey(
                        name: "FK_tblProveedoresMedicamentos_tblMedicamentos_CodigoMedicamento",
                        column: x => x.CodigoMedicamento,
                        principalTable: "tblMedicamentos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblProveedoresMedicamentos_tblProveedores_CodigoProveedor",
                        column: x => x.CodigoProveedor,
                        principalTable: "tblProveedores",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRolClaims",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoRol = table.Column<int>(type: "int", nullable: false),
                    TipoClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRolClaims", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_tblRolClaims_tblRoles_CodigoRol",
                        column: x => x.CodigoRol,
                        principalTable: "tblRoles",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarioClaims",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoUsuario = table.Column<int>(type: "int", nullable: false),
                    TipoClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioClaims", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_tblUsuarioClaims_tblUsuarios_CodigoUsuario",
                        column: x => x.CodigoUsuario,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarioLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tblUsuarioLogin_tblUsuarios_CodigoUsuario",
                        column: x => x.CodigoUsuario,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuariosRoles",
                columns: table => new
                {
                    CodigoUsuario = table.Column<int>(type: "int", nullable: false),
                    CodigoRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuariosRoles", x => new { x.CodigoUsuario, x.CodigoRol });
                    table.ForeignKey(
                        name: "FK_tblUsuariosRoles_tblRoles_CodigoRol",
                        column: x => x.CodigoRol,
                        principalTable: "tblRoles",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUsuariosRoles_tblUsuarios_CodigoUsuario",
                        column: x => x.CodigoUsuario,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarioToken",
                columns: table => new
                {
                    CodigoUsuario = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioToken", x => new { x.CodigoUsuario, x.LoginProvider, x.Nombre });
                    table.ForeignKey(
                        name: "FK_tblUsuarioToken_tblUsuarios_CodigoUsuario",
                        column: x => x.CodigoUsuario,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFacturasMedicamentosCajas",
                columns: table => new
                {
                    CodigoFactura = table.Column<int>(type: "int", nullable: false),
                    CodigoCaja = table.Column<int>(type: "int", nullable: false),
                    TipoCantidad = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidad = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFacturasMedicamentosCajas", x => new { x.CodigoFactura, x.CodigoCaja });
                    table.ForeignKey(
                        name: "FK_tblFacturasMedicamentosCajas_tblFacturas_CodigoFactura",
                        column: x => x.CodigoFactura,
                        principalTable: "tblFacturas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFacturasMedicamentosCajas_tblMedicamentosCajas_CodigoCaja",
                        column: x => x.CodigoCaja,
                        principalTable: "tblMedicamentosCajas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tblRoles",
                columns: new[] { "Codigo", "ConcurrencyStamp", "Nombre", "NombreNormalizado" },
                values: new object[,]
                {
                    { 1, "07c491f0-be48-4705-a860-0f2e196497a8", "Administrador", "ADMINISTRADOR" },
                    { 2, "65fce93a-9d0e-400f-a3c1-43d2b3f09569", "Comprador", "COMPRADOR" },
                    { 3, "1a5e96fc-c165-48b6-b6c8-f58a93dec18a", "Vendedor", "VENDEDOR" }
                });

            migrationBuilder.InsertData(
                table: "tblUsuarios",
                columns: new[] { "Codigo", "AccessFailedCount", "Apellido", "Cedula", "ConcurrencyStamp", "Creado", "CreadoNombreUsuario", "Direccion", "Email", "ConfirmarEmail", "FechaNacimiento", "Foto", "Inactivo", "LockoutEnabled", "LockoutEnd", "Modificado", "ModificadoNombreUsuario", "Nombre", "EmailNormalizado", "NombreUsuarioNormalizado", "PasswordHash", "Telefono", "ConfirmarTelefono", "SecurityStamp", "TwoFactorEnabled", "NombreUsuario" },
                values: new object[] { 1, 0, "", null, "47a14708-85df-4069-86d3-9078a8d6300a", new DateTime(2022, 8, 9, 12, 41, 15, 452, DateTimeKind.Local).AddTicks(2220), null, null, "admin@gmail.com", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, null, new DateTime(2022, 8, 9, 12, 41, 15, 452, DateTimeKind.Local).AddTicks(2272), null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEB6ZSLZ0yaEUn0tARYVJQ5pzHDUYcBNrw7imkPoiMZ74bLwrQO/5PQ4ZnBAk1IbgAw==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "tblUsuariosRoles",
                columns: new[] { "CodigoRol", "CodigoUsuario" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_tblFacturasMedicamentosCajas_CodigoCaja",
                table: "tblFacturasMedicamentosCajas",
                column: "CodigoCaja");

            migrationBuilder.CreateIndex(
                name: "IX_tblMedicamentosCajas_CodigoMedicamento",
                table: "tblMedicamentosCajas",
                column: "CodigoMedicamento");

            migrationBuilder.CreateIndex(
                name: "IX_tblProveedoresMedicamentos_CodigoProveedor",
                table: "tblProveedoresMedicamentos",
                column: "CodigoProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_tblRolClaims_CodigoRol",
                table: "tblRolClaims",
                column: "CodigoRol");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "tblRoles",
                column: "NombreNormalizado",
                unique: true,
                filter: "[NombreNormalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsuarioClaims_CodigoUsuario",
                table: "tblUsuarioClaims",
                column: "CodigoUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsuarioLogin_CodigoUsuario",
                table: "tblUsuarioLogin",
                column: "CodigoUsuario");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "tblUsuarios",
                column: "EmailNormalizado");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "tblUsuarios",
                column: "NombreUsuarioNormalizado",
                unique: true,
                filter: "[NombreUsuarioNormalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsuariosRoles_CodigoRol",
                table: "tblUsuariosRoles",
                column: "CodigoRol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblFacturasMedicamentosCajas");

            migrationBuilder.DropTable(
                name: "tblProveedoresMedicamentos");

            migrationBuilder.DropTable(
                name: "tblRolClaims");

            migrationBuilder.DropTable(
                name: "tblUsuarioClaims");

            migrationBuilder.DropTable(
                name: "tblUsuarioLogin");

            migrationBuilder.DropTable(
                name: "tblUsuariosRoles");

            migrationBuilder.DropTable(
                name: "tblUsuarioToken");

            migrationBuilder.DropTable(
                name: "tblFacturas");

            migrationBuilder.DropTable(
                name: "tblMedicamentosCajas");

            migrationBuilder.DropTable(
                name: "tblProveedores");

            migrationBuilder.DropTable(
                name: "tblRoles");

            migrationBuilder.DropTable(
                name: "tblUsuarios");

            migrationBuilder.DropTable(
                name: "tblMedicamentos");
        }
    }
}
