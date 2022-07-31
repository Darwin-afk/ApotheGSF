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
                    Costo = table.Column<int>(type: "int", nullable: false),
                    PrecioUnidad = table.Column<int>(type: "int", nullable: false),
                    Indicaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoNombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: true)
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
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRoles", x => x.RolId);
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
                    CajaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicamentoId = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidad = table.Column<int>(type: "int", nullable: false),
                    FechaAdquirido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detallada = table.Column<bool>(type: "bit", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMedicamentosCajas", x => x.CajaId);
                    table.ForeignKey(
                        name: "FK_tblMedicamentosCajas_tblMedicamentos_MedicamentoId",
                        column: x => x.MedicamentoId,
                        principalTable: "tblMedicamentos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblProveedoresMedicamentos",
                columns: table => new
                {
                    ProveedoresId = table.Column<int>(type: "int", nullable: false),
                    MedicamentosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProveedoresMedicamentos", x => new { x.MedicamentosId, x.ProveedoresId });
                    table.ForeignKey(
                        name: "FK_tblProveedoresMedicamentos_tblMedicamentos_MedicamentosId",
                        column: x => x.MedicamentosId,
                        principalTable: "tblMedicamentos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblProveedoresMedicamentos_tblProveedores_ProveedoresId",
                        column: x => x.ProveedoresId,
                        principalTable: "tblProveedores",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRolClaims",
                columns: table => new
                {
                    RolClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    TipoClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRolClaims", x => x.RolClaimId);
                    table.ForeignKey(
                        name: "FK_tblRolClaims_tblRoles_RolId",
                        column: x => x.RolId,
                        principalTable: "tblRoles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarioClaims",
                columns: table => new
                {
                    UsuarioClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoClaim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorClaim = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioClaims", x => x.UsuarioClaimId);
                    table.ForeignKey(
                        name: "FK_tblUsuarioClaims_tblUsuarios_UsuarioId",
                        column: x => x.UsuarioId,
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
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tblUsuarioLogin_tblUsuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuariosRoles",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuariosRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_tblUsuariosRoles_tblRoles_RolId",
                        column: x => x.RolId,
                        principalTable: "tblRoles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblUsuariosRoles_tblUsuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblUsuarioToken",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsuarioToken", x => new { x.UsuarioId, x.LoginProvider, x.Nombre });
                    table.ForeignKey(
                        name: "FK_tblUsuarioToken_tblUsuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "tblUsuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblFacturasMedicamentosCajas",
                columns: table => new
                {
                    FacturaId = table.Column<int>(type: "int", nullable: false),
                    CajaId = table.Column<int>(type: "int", nullable: false),
                    TipoCantidad = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidad = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFacturasMedicamentosCajas", x => new { x.FacturaId, x.CajaId });
                    table.ForeignKey(
                        name: "FK_tblFacturasMedicamentosCajas_tblFacturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "tblFacturas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblFacturasMedicamentosCajas_tblMedicamentosCajas_CajaId",
                        column: x => x.CajaId,
                        principalTable: "tblMedicamentosCajas",
                        principalColumn: "CajaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tblRoles",
                columns: new[] { "RolId", "ConcurrencyStamp", "Nombre", "NombreNormalizado" },
                values: new object[,]
                {
                    { 1, "b768122b-0a03-4c23-a672-0d236c1d126f", "Administrador", "ADMINISTRADOR" },
                    { 2, "e7525b6d-1c8b-4a69-8b69-a97fe7105466", "Comprador", "COMPRADOR" },
                    { 3, "904bc662-dd84-43d1-9af6-0b013467ad58", "Vendedor", "VENDEDOR" }
                });

            migrationBuilder.InsertData(
                table: "tblUsuarios",
                columns: new[] { "Codigo", "AccessFailedCount", "Apellido", "Cedula", "ConcurrencyStamp", "Creado", "CreadoNombreUsuario", "Direccion", "Email", "ConfirmarEmail", "FechaNacimiento", "Foto", "Inactivo", "LockoutEnabled", "LockoutEnd", "Modificado", "ModificadoNombreUsuario", "Nombre", "EmailNormalizado", "NombreUsuarioNormalizado", "PasswordHash", "Telefono", "ConfirmarTelefono", "SecurityStamp", "TwoFactorEnabled", "NombreUsuario" },
                values: new object[] { 1, 0, "", null, "71ee5cb4-523c-4365-9e9e-bf96248e2621", new DateTime(2022, 7, 31, 17, 49, 54, 469, DateTimeKind.Local).AddTicks(6629), null, null, "admin@gmail.com", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, null, new DateTime(2022, 7, 31, 17, 49, 54, 469, DateTimeKind.Local).AddTicks(6677), null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEFgVLHx74s4ivGCht7IpVPx4Oh5Q8B123rMBXGvJTLVcc7WG2zPJMb2Z9RyOVZTrAw==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "tblUsuariosRoles",
                columns: new[] { "RolId", "UsuarioId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_tblFacturasMedicamentosCajas_CajaId",
                table: "tblFacturasMedicamentosCajas",
                column: "CajaId");

            migrationBuilder.CreateIndex(
                name: "IX_tblMedicamentosCajas_MedicamentoId",
                table: "tblMedicamentosCajas",
                column: "MedicamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_tblProveedoresMedicamentos_ProveedoresId",
                table: "tblProveedoresMedicamentos",
                column: "ProveedoresId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRolClaims_RolId",
                table: "tblRolClaims",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "tblRoles",
                column: "NombreNormalizado",
                unique: true,
                filter: "[NombreNormalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsuarioClaims_UsuarioId",
                table: "tblUsuarioClaims",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_tblUsuarioLogin_UsuarioId",
                table: "tblUsuarioLogin",
                column: "UsuarioId");

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
                name: "IX_tblUsuariosRoles_RolId",
                table: "tblUsuariosRoles",
                column: "RolId");
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
