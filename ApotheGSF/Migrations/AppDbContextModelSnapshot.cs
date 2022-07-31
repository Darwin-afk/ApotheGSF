﻿// <auto-generated />
using System;
using ApotheGSF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApotheGSF.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ApotheGSF.Models.AppRol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RolId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("Nombre");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("NombreNormalizado");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NombreNormalizado] IS NOT NULL");

                    b.ToTable("tblRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "b768122b-0a03-4c23-a672-0d236c1d126f",
                            Name = "Administrador",
                            NormalizedName = "ADMINISTRADOR"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "e7525b6d-1c8b-4a69-8b69-a97fe7105466",
                            Name = "Comprador",
                            NormalizedName = "COMPRADOR"
                        },
                        new
                        {
                            Id = 3,
                            ConcurrencyStamp = "904bc662-dd84-43d1-9af6-0b013467ad58",
                            Name = "Vendedor",
                            NormalizedName = "VENDEDOR"
                        });
                });

            modelBuilder.Entity("ApotheGSF.Models.AppUsuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Codigo");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Apellido");

                    b.Property<string>("Cedula")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Creado")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Direccion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("ConfirmarEmail");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<string>("Foto")
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Foto");

                    b.Property<bool?>("Inactivo")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime?>("Modificado")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModificadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Nombre");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("EmailNormalizado");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("NombreUsuarioNormalizado");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Telefono");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit")
                        .HasColumnName("ConfirmarTelefono");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("NombreUsuario");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NombreUsuarioNormalizado] IS NOT NULL");

                    b.ToTable("tblUsuarios", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccessFailedCount = 0,
                            Apellido = "",
                            ConcurrencyStamp = "71ee5cb4-523c-4365-9e9e-bf96248e2621",
                            Creado = new DateTime(2022, 7, 31, 17, 49, 54, 469, DateTimeKind.Local).AddTicks(6629),
                            Email = "admin@gmail.com",
                            EmailConfirmed = false,
                            FechaNacimiento = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Inactivo = false,
                            LockoutEnabled = true,
                            Modificado = new DateTime(2022, 7, 31, 17, 49, 54, 469, DateTimeKind.Local).AddTicks(6677),
                            Nombre = "Admin",
                            NormalizedEmail = "ADMIN@GMAIL.COM",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAEAACcQAAAAEFgVLHx74s4ivGCht7IpVPx4Oh5Q8B123rMBXGvJTLVcc7WG2zPJMb2Z9RyOVZTrAw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("ApotheGSF.Models.AppUsuarioRol", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioId");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RolId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("tblUsuariosRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            RoleId = 1
                        });
                });

            modelBuilder.Entity("ApotheGSF.Models.FacturaMedicamentosCajas", b =>
                {
                    b.Property<int>("FacturaId")
                        .HasColumnType("int");

                    b.Property<int>("CajaId")
                        .HasColumnType("int");

                    b.Property<int>("CantidadUnidad")
                        .HasColumnType("int");

                    b.Property<float>("Precio")
                        .HasColumnType("real");

                    b.Property<int>("TipoCantidad")
                        .HasColumnType("int");

                    b.HasKey("FacturaId", "CajaId");

                    b.HasIndex("CajaId");

                    b.ToTable("tblFacturasMedicamentosCajas", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.Facturas", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Codigo"), 1L, 1);

                    b.Property<DateTime?>("Creado")
                        .HasColumnType("datetime")
                        .HasColumnName("Creado");

                    b.Property<string>("CreadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Inactivo")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("Modificado")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModificadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("SubTotal")
                        .HasColumnType("real");

                    b.Property<float>("Total")
                        .HasColumnType("real");

                    b.HasKey("Codigo");

                    b.ToTable("tblFacturas", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.Medicamentos", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Codigo"), 1L, 1);

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Concentracion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Costo")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Creado")
                        .HasColumnType("datetime")
                        .HasColumnName("Creado");

                    b.Property<string>("CreadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Inactivo")
                        .HasColumnType("bit");

                    b.Property<string>("Indicaciones")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Modificado")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModificadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("PrecioUnidad")
                        .HasColumnType("int");

                    b.Property<string>("Sustancia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UnidadesCaja")
                        .HasColumnType("int");

                    b.HasKey("Codigo");

                    b.ToTable("tblMedicamentos", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.MedicamentosCajas", b =>
                {
                    b.Property<int>("CajaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CajaId"), 1L, 1);

                    b.Property<int>("CantidadUnidad")
                        .HasColumnType("int");

                    b.Property<bool>("Detallada")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaAdquirido")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaVencimiento")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Inactivo")
                        .HasColumnType("bit");

                    b.Property<int>("MedicamentoId")
                        .HasColumnType("int");

                    b.HasKey("CajaId");

                    b.HasIndex("MedicamentoId");

                    b.ToTable("tblMedicamentosCajas", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.Proveedores", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Codigo"), 1L, 1);

                    b.Property<DateTime?>("Creado")
                        .HasColumnType("datetime")
                        .HasColumnName("Creado");

                    b.Property<string>("CreadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fax")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Inactivo")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("Modificado")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModificadoNombreUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RNC")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TerminosdePago")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Codigo");

                    b.ToTable("tblProveedores", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.ProveedorMedicamentos", b =>
                {
                    b.Property<int>("MedicamentosId")
                        .HasColumnType("int");

                    b.Property<int>("ProveedoresId")
                        .HasColumnType("int");

                    b.HasKey("MedicamentosId", "ProveedoresId");

                    b.HasIndex("ProveedoresId");

                    b.ToTable("tblProveedoresMedicamentos", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RolClaimId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TipoClaim");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ValorClaim");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RolId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("tblRolClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UsuarioClaimId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TipoClaim");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ValorClaim");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("tblUsuarioClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("tblUsuarioLogin", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioId");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Nombre");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Valor");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("tblUsuarioToken", (string)null);
                });

            modelBuilder.Entity("ApotheGSF.Models.AppUsuarioRol", b =>
                {
                    b.HasOne("ApotheGSF.Models.AppRol", "Rol")
                        .WithMany("UsuariosRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApotheGSF.Models.AppUsuario", "Usuario")
                        .WithMany("UsuariosRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Rol");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ApotheGSF.Models.FacturaMedicamentosCajas", b =>
                {
                    b.HasOne("ApotheGSF.Models.MedicamentosCajas", "MedicamentosCajas")
                        .WithMany("FacturaMedicamentos")
                        .HasForeignKey("CajaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApotheGSF.Models.Facturas", "Facturas")
                        .WithMany("FacturasMedicamentosCajas")
                        .HasForeignKey("FacturaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Facturas");

                    b.Navigation("MedicamentosCajas");
                });

            modelBuilder.Entity("ApotheGSF.Models.MedicamentosCajas", b =>
                {
                    b.HasOne("ApotheGSF.Models.Medicamentos", "Medicamentos")
                        .WithMany("MedicamentosCajas")
                        .HasForeignKey("MedicamentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicamentos");
                });

            modelBuilder.Entity("ApotheGSF.Models.ProveedorMedicamentos", b =>
                {
                    b.HasOne("ApotheGSF.Models.Medicamentos", "Medicamentos")
                        .WithMany("ProveedoresMedicamentos")
                        .HasForeignKey("MedicamentosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApotheGSF.Models.Proveedores", "Proveedores")
                        .WithMany("ProveedoresMedicamentos")
                        .HasForeignKey("ProveedoresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicamentos");

                    b.Navigation("Proveedores");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("ApotheGSF.Models.AppRol", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("ApotheGSF.Models.AppUsuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("ApotheGSF.Models.AppUsuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("ApotheGSF.Models.AppUsuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApotheGSF.Models.AppRol", b =>
                {
                    b.Navigation("UsuariosRoles");
                });

            modelBuilder.Entity("ApotheGSF.Models.AppUsuario", b =>
                {
                    b.Navigation("UsuariosRoles");
                });

            modelBuilder.Entity("ApotheGSF.Models.Facturas", b =>
                {
                    b.Navigation("FacturasMedicamentosCajas");
                });

            modelBuilder.Entity("ApotheGSF.Models.Medicamentos", b =>
                {
                    b.Navigation("MedicamentosCajas");

                    b.Navigation("ProveedoresMedicamentos");
                });

            modelBuilder.Entity("ApotheGSF.Models.MedicamentosCajas", b =>
                {
                    b.Navigation("FacturaMedicamentos");
                });

            modelBuilder.Entity("ApotheGSF.Models.Proveedores", b =>
                {
                    b.Navigation("ProveedoresMedicamentos");
                });
#pragma warning restore 612, 618
        }
    }
}
