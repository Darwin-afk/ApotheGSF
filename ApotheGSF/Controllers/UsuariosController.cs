using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;
using System.Security.Claims;
using ApotheGSF.Clases;
using Microsoft.AspNetCore.Identity;

namespace ApotheGSF.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<AppRol> _roleManager;
        private readonly ClaimsPrincipal _user;

        public UsuariosController(AppDbContext context,
                                  UserManager<AppUsuario> userManager,
                                  RoleManager<AppRol> roleManager,
                                  IHttpContextAccessor accessor)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _user = accessor.HttpContext.User;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var lista = await (from u in _context.AppUsuarios
                            .AsNoTracking()
                            .AsQueryable()
                               join ur in _context.AppUsuariosRoles on u.Id equals ur.UserId
                               join r in _context.Roles on ur.RoleId equals r.Id
                               select new AppUsuario
                               {
                                   Id = u.Id,
                                   Nombre = u.Nombre,
                                   UserName = u.UserName,
                                   Email = u.Email,
                                   PhoneNumber = u.PhoneNumber,
                                   Inactivo = u.Inactivo,
                                   Rol = r.Name
                               }).Where(x => x.Inactivo == false).ToListAsync();

            return lista != null ?
                    View(lista) :
                    Problem("Entity set 'AppDbContext.AppUsuario'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AppUsuarios == null)
            {
                return NotFound();
            }

            var usuario = await (from u in _context.AppUsuarios
                            .AsNoTracking()
                            .AsQueryable()
                                 join ur in _context.AppUsuariosRoles on u.Id equals ur.UserId
                                 join r in _context.Roles on ur.RoleId equals r.Id
                                 join creado in _context.AppUsuarios on u.CreadoPorId equals creado.Id into lj2
                                 from y in lj2.DefaultIfEmpty()
                                 join modificado in _context.AppUsuarios on u.ModificadoPorId equals modificado.Id into lj
                                 from x in lj.DefaultIfEmpty()
                                 select new UsuarioViewModel
                                 {
                                     Id = u.Id,
                                     Nombre = u.Nombre,
                                     Usuario = u.UserName,
                                     Email = u.Email,
                                     Telefono = u.PhoneNumber,
                                     Rol = r.Name,
                                     Creado = u.Creado,
                                     Modificado = u.Modificado,
                                     ModificadoPor = x == null ? string.Empty : x.Nombre,
                                     CreadPor = x == null ? string.Empty : y.Nombre,
                                 }).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre, Apellido, Usuario, FechaNacimiento, Cedula, Email, Direccion, Telefono, Password, ConfirmarPassword, Rol")] UsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AppUsuario nuevoUsuario = new()
                {
                    Nombre = viewModel.Nombre,
                    Apellido = viewModel.Apellido,
                    UserName = viewModel.Usuario,
                    FechaNacimiento = viewModel.FechaNacimiento,
                    Cedula = viewModel.Cedula,
                    Email = viewModel.Email,
                    Direccion = viewModel.Direccion,
                    PhoneNumber = viewModel.Telefono,
                    Creado = DateTime.Now,
                    CreadoPorId = _user.GetUserID().ToInt(),
                    Modificado = DateTime.Now,
                    ModificadoPorId = _user.GetUserID().ToInt(),
                    Inactivo = false
                };

                //Crea el usuario y lo asigna al rol elegido.
                //Se debe validar que el email y el telefono no se repitan.
                // Verificar si _userManager tiene una opcion, se pueden hacer indeces unique y se puede hacer un query antes de.
                var result = await _userManager.CreateAsync(nuevoUsuario, viewModel.Password);
                if (result.Succeeded)
                {
                    var rr = await _userManager.AddToRoleAsync(nuevoUsuario, viewModel.Rol);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
            return View(viewModel);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUsuario appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AppUsuarios == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AppUsuarios == null)
            {
                return Problem("Entity set 'AppDbContext.AppUser'  is null.");
            }
            var appUser = await _context.AppUsuarios.FindAsync(id);
            if (appUser != null)
            {
                _context.AppUsuarios.Remove(appUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
          return (_context.AppUsuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
