using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ApotheGSF.Models
{
    public class AppDbContext : IdentityDbContext<AppUsuario, AppRol, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //---
        //    //Application User
        //    //--------------------------------
        //    modelBuilder.Entity<AppUser>(entity =>
        //    {
        //        entity.ToTable(name: "tblUsuarios");
        //        entity.Property(u => u.UserName).HasColumnName("NombreUsuario");
        //        entity.HasMany(e => e.UserRoles)
        //            .WithOne(e => e.User)
        //            .HasForeignKey(ur => ur.UserId)
        //            .OnDelete(DeleteBehavior.Restrict); //Esto se debe hacer para evitar que sql server diga que hay referencias ondelete cíclicas.
        //                                                //.IsRequired();
        //    });
        //    //---
        //    //Application Roles
        //    //--------------------------------
        //    modelBuilder.Entity<AppRole>(entity =>
        //    {
        //        entity.ToTable(name: "tblRoles");
        //        //entity.HasIndex(x => x.NormalizedName).HasName("RoleNameIndex").IsUnique(false); //permite que existan nombres duplicados
        //        entity.HasMany(e => e.UserRoles)
        //            .WithOne(e => e.Role)
        //            .HasForeignKey(ur => ur.RoleId)
        //            .OnDelete(DeleteBehavior.Cascade)
        //            .IsRequired();
        //    });

        //    modelBuilder.Entity<AppUserRole>(entity =>
        //    {
        //        entity.ToTable("tblRolesUsuario");
        //    });
        //    modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
        //    {
        //        entity.ToTable("tblRolClaims");
        //    });
        //    modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        //    {
        //        entity.ToTable("tblUserClaims");
        //    });
        //    modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        //    {
        //        entity.ToTable("tblUserLogin");
        //    });
        //    modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        //    {
        //        entity.ToTable("tblUserToken");
        //    });

        //    modelBuilder.Entity<AppRole>().HasData(
        //        new AppRole
        //        {
        //            Id = 1,
        //            Name = "Administrador",
        //            NormalizedName = "Administrador".ToUpper()
        //        },
        //        new AppRole
        //        {
        //            Id = 2,
        //            Name = "Comprador",
        //            NormalizedName = "Comprador".ToUpper()
        //        },
        //        new AppRole
        //        {
        //            Id = 3,
        //            Name = "Vendedor",
        //            NormalizedName = "Vendedor".ToUpper()
        //        });

        //    var hasher = new PasswordHasher<IdentityUser>();
        //    modelBuilder.Entity<AppUser>().HasData(new AppUser
        //    {
        //        Id = 1,
        //        Nombre = "Admin",
        //        UserName = "admin",
        //        NormalizedUserName = "admin".ToUpper(),
        //        Email = "admin@gmail.com",
        //        NormalizedEmail = "admin@gmail.com".ToUpper(),
        //        PasswordHash = hasher.HashPassword(null, "1234"),
        //        Rol = "Administrador",
        //        Creado = DateTime.Now,
        //        CreadoPorId = 0,
        //        Modificado = DateTime.Now,
        //        ModificadoPorId = 0,
        //        SecurityStamp = string.Empty,
        //        LockoutEnabled = true
        //    });

        //    modelBuilder.Entity<AppUserRole>().HasData(new AppUserRole
        //    {
        //        UserId = 1,
        //        RoleId = 1
        //    });
        //}

        public DbSet<AppUsuario> AppUser { get; set; }
        public DbSet<AppRol> AppRole { get; set; }
        public DbSet<AppUserRole> AppUserRole { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
    }
}
