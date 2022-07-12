using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ApotheGSF.Models
{
    public class AppDbContext : IdentityDbContext<AppUsuario, AppRol, int,
        IdentityUserClaim<int>, AppUsuarioRol, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //---
            //App User
            //--------------------------------
            modelBuilder.Entity<AppUsuario>(entity =>
            {
                entity.ToTable(name: "tblUsuarios");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.Id).HasColumnName("Codigo");
                entity.Property(p => p.UserName).HasColumnName("NombreUsuario");
                entity.Property(p => p.NormalizedUserName).HasColumnName("NombreUsuarioNormalizado");
                entity.Property(p => p.NormalizedEmail).HasColumnName("EmailNormalizado");
                entity.Property(p => p.EmailConfirmed).HasColumnName("ConfirmarEmail");
                entity.Property(p => p.PhoneNumber).HasColumnName("Telefono");
                entity.Property(p => p.PhoneNumberConfirmed).HasColumnName("ConfirmarTelefono");
                //-
                //Establecion la relacion entre la tabla usuariosRoles y usuario
                entity.HasMany(e => e.UsuariosRoles)
                    .WithOne(e => e.Usuario)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict); //Esto se debe hacer para evitar que sql server diga que hay referencias ondelete cíclicas.
                                                        //.IsRequired();
            });
            //---
            //App Roles
            //--------------------------------
            modelBuilder.Entity<AppRol>(entity =>
            {
                entity.ToTable(name: "tblRoles");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.Id).HasColumnName("RolId");
                entity.Property(p => p.Name).HasColumnName("Nombre");
                entity.Property(p => p.NormalizedName).HasColumnName("NombreNormalizado");
                //-
                //Establecion la relacion entre la tabla usuariosRoles y Rol
                entity.HasMany(e => e.UsuariosRoles)
                    .WithOne(e => e.Rol)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<AppUsuarioRol>(entity =>
            {
                entity.ToTable("tblUsuariosRoles");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.UserId).HasColumnName("UsuarioId");
                entity.Property(p => p.RoleId).HasColumnName("RolId");
                //-
            });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("tblRolClaims");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.Id).HasColumnName("RolClaimId");
                entity.Property(p => p.RoleId).HasColumnName("RolId");
                entity.Property(p => p.ClaimType).HasColumnName("TipoClaim");
                entity.Property(p => p.ClaimValue).HasColumnName("ValorClaim");
                //-
            });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("tblUsuarioClaims");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.Id).HasColumnName("UsuarioClaimId");
                entity.Property(p => p.UserId).HasColumnName("UsuarioId");
                entity.Property(p => p.ClaimType).HasColumnName("TipoClaim");
                entity.Property(p => p.ClaimValue).HasColumnName("ValorClaim");
                //-
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("tblUsuarioLogin");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.UserId).HasColumnName("UsuarioId");
                //-
            });
            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("tblUsuarioToken");
                //Cambindo los nombres de las columnas en la tabla
                entity.Property(p => p.UserId).HasColumnName("UsuarioId");
                entity.Property(p => p.Name).HasColumnName("Nombre");
                entity.Property(p => p.Value).HasColumnName("Valor");
                //-
            });
            modelBuilder.Entity<Proveedores>(entity =>
            {
                entity.ToTable("tblProveedores");
            });
            modelBuilder.Entity<ProveedorMedicamento>(entity =>
            {
                entity.ToTable("tblProveedoresMedicamentos");
            });
            modelBuilder.Entity<Medicamentos>(entity =>
            {
                entity.ToTable("tblMedicamentos");
            });
            modelBuilder.Entity<FacturaMedicamentos>(entity =>
            {
                entity.ToTable("tblFacturasMedicamentos");
            });
            modelBuilder.Entity<Facturas>(entity =>
            {
                entity.ToTable("tblFacturas");
            });

            modelBuilder.Entity<AppRol>().HasData(
                new AppRol
                {
                    Id = 1,
                    Name = "Administrador",
                    NormalizedName = "Administrador".ToUpper()
                },
                new AppRol
                {
                    Id = 2,
                    Name = "Comprador",
                    NormalizedName = "Comprador".ToUpper()
                },
                new AppRol
                {
                    Id = 3,
                    Name = "Vendedor",
                    NormalizedName = "Vendedor".ToUpper()
                });

            var hasher = new PasswordHasher<IdentityUser>();
            modelBuilder.Entity<AppUsuario>().HasData(new AppUsuario
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "",
                UserName = "admin",
                NormalizedUserName = "admin".ToUpper(),
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com".ToUpper(),
                PasswordHash = hasher.HashPassword(null, "1234"),
                Creado = DateTime.Now,
                CreadoPorId = 0,
                Modificado = DateTime.Now,
                ModificadoPorId = 0,
                SecurityStamp = string.Empty,
                LockoutEnabled = true
            });

            modelBuilder.Entity<AppUsuarioRol>().HasData(new AppUsuarioRol
            {
                UserId = 1,
                RoleId = 1
            });
        }

        public DbSet<AppUsuario> AppUsuarios { get; set; }
        public DbSet<AppRol> AppRoles { get; set; }
        public DbSet<AppUsuarioRol> AppUsuariosRoles { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<Facturas> Facturas { get; set; }
        public DbSet<Medicamentos> Medicamentos { get; set; }
    }
}
