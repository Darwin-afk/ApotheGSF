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
using System.Text;
using ReflectionIT.Mvc.Paging;
using System.Linq.Dynamic.Core;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace ApotheGSF.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<AppRol> _roleManager;
        private readonly ClaimsPrincipal _user;
        private readonly INotyfService _notyf;

        public UsuariosController(AppDbContext context,
                                  UserManager<AppUsuario> userManager,
                                  RoleManager<AppRol> roleManager,
                                  IHttpContextAccessor accessor,
                                  INotyfService notyf)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _user = accessor.HttpContext.User;
            _notyf = notyf;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sortExpression = "", int search = 0)
        {
            StringBuilder filtro = new StringBuilder($" Inactivo == false && Codigo != {User.GetUserID().ToInt()}");
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<UsuarioViewModel> listado = new List<UsuarioViewModel>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await (from u in _context.AppUsuarios
                               .AsNoTracking()
                               .AsQueryable()
                                 join ur in _context.AppUsuariosRoles on u.Id equals ur.UserId
                                 join r in _context.Roles on ur.RoleId equals r.Id
                                 select new UsuarioViewModel
                                 {
                                     Codigo = u.Id,
                                     Nombre = u.Nombre,
                                     Apellido = u.Apellido,
                                     Telefono = u.PhoneNumber,
                                     Direccion = u.Direccion,
                                     Usuario = u.UserName,
                                     Email = u.Email,
                                     Rol = r.Name,
                                     Inactivo = u.Inactivo

                                 }).Where(filtro.ToString()).ToListAsync();
            }
            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Nombre" : sortExpression;
            var model = PagingList.Create(listado, 3, pageindex, sortExpression, "");
            model.RouteValue = new RouteValueDictionary {
                            { "filter", filter}
            };
            model.Action = "Index";

            return model != null ?
                View(model) :
                Problem("Entity set 'ApplicationDbContext.ApplicationUser'  is null.");
            ;

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
                                 join creado in _context.AppUsuarios on u.CreadoNombreUsuario equals creado.CreadoNombreUsuario into lj2
                                 from y in lj2.DefaultIfEmpty()
                                 join modificado in _context.AppUsuarios on u.ModificadoNombreUsuario equals modificado.ModificadoNombreUsuario into lj
                                 from x in lj.DefaultIfEmpty()
                                 select new UsuarioViewModel
                                 {
                                     Codigo = u.Id,
                                     Nombre = u.Nombre,
                                     Apellido = u.Apellido,
                                     Usuario = u.UserName,
                                     FechaNacimiento = u.FechaNacimiento,
                                     Cedula = u.Cedula,
                                     Email = u.Email,
                                     Telefono = u.PhoneNumber,
                                     Direccion = u.Direccion,
                                     Rol = r.Name,
                                     Creado = u.Creado,
                                     Modificado = u.Modificado,
                                     ModificadoPor = x == null ? string.Empty : x.Nombre,
                                     CreadPor = x == null ? string.Empty : y.Nombre,
                                     Inactivo = u.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();

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
                //obtener lista de usuarios
                List<AppUsuario> usuarios = _context.AppUsuarios.Where(m => m.Inactivo == false).ToList();
                //si la lista no es null
                if (usuarios != null)
                {
                    //por cada elemento de la lista
                    foreach (var usuario in usuarios)
                    {
                        //se verifica si tiene el mismo nombre que el usuario que se quiere crear
                        if (usuario.UserName.ToUpper() == viewModel.Usuario.ToUpper())
                        {
                            //si lo tiene regresa error
                            _notyf.Error("Este usuario ya existe");
                            return View(viewModel);
                        }

                    }
                }

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
                    CreadoNombreUsuario = _user.GetUserName(),
                    Modificado = DateTime.Now,
                    ModificadoNombreUsuario = _user.GetUserName(),
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
                        _notyf.Error($"{error.Code} - {error.Description}");
                    }
                }
            }
            return View(viewModel);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
                                 select new UsuarioViewModel
                                 {
                                     Codigo = u.Id,
                                     Nombre = u.Nombre,
                                     Apellido = u.Apellido,
                                     Usuario = u.UserName,
                                     Email = u.Email,
                                     Telefono = u.PhoneNumber,
                                     Cedula = u.Cedula,
                                     FechaNacimiento = u.FechaNacimiento,
                                     Direccion = u.Direccion,
                                     Rol = r.Name
                                 }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo, Nombre, Apellido, Usuario, FechaNacimiento, Cedula, Email, Direccion, Telefono, Rol")] UsuarioViewModel viewModel)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmarPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var antiguoUsuario = await _context.AppUsuarios.FirstOrDefaultAsync(x => x.Id == viewModel.Codigo);
                    antiguoUsuario.Nombre = viewModel.Nombre;
                    antiguoUsuario.Apellido = viewModel.Apellido;
                    antiguoUsuario.PhoneNumber = viewModel.Telefono;
                    antiguoUsuario.Cedula = viewModel.Cedula;
                    antiguoUsuario.FechaNacimiento = viewModel.FechaNacimiento;
                    antiguoUsuario.Direccion = viewModel.Direccion;
                    antiguoUsuario.Email = viewModel.Email;
                    antiguoUsuario.UserName = viewModel.Usuario;
                    antiguoUsuario.Modificado = DateTime.Now;
                    antiguoUsuario.ModificadoNombreUsuario = _user.GetUserName();

                    //Se debe validar que el email y el telefono no se repitan.
                    // Verificar si _userManager tiene una opcion, se pueden hacer indeces unique y se puede hacer un query antes de.
                    _context.Update(antiguoUsuario);
                    antiguoUsuario.Modificado = DateTime.Now;
                    antiguoUsuario.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Entry(antiguoUsuario).Property(c => c.Creado).IsModified = false;
                    _context.Entry(antiguoUsuario).Property(c => c.CreadoNombreUsuario).IsModified = false;
                    _context.Entry(antiguoUsuario).Property(c => c.Inactivo).IsModified = false;
                    var result = await _context.SaveChangesAsync();

                    // verificar si se grabó bien, para luego asignar el rol
                    if (result > 0)
                    {
                        var rolesViejos = await _context.AppUsuariosRoles.Where(x => x.UserId == viewModel.Codigo).ToListAsync();
                        _context.RemoveRange(rolesViejos);
                        await _context.SaveChangesAsync();
                        await _userManager.AddToRoleAsync(antiguoUsuario, viewModel.Rol);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(ViewBag.Id))
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
            return View(viewModel);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                                 join creado in _context.AppUsuarios on u.CreadoNombreUsuario equals creado.CreadoNombreUsuario into lj2
                                 from y in lj2.DefaultIfEmpty()
                                 join modificado in _context.AppUsuarios on u.ModificadoNombreUsuario equals modificado.ModificadoNombreUsuario into lj
                                 from x in lj.DefaultIfEmpty()
                                 select new UsuarioViewModel
                                 {
                                     Codigo = u.Id,
                                     Nombre = u.Nombre,
                                     Apellido = u.Apellido,
                                     Usuario = u.UserName,
                                     FechaNacimiento = u.FechaNacimiento,
                                     Cedula = u.Cedula,
                                     Email = u.Email,
                                     Telefono = u.PhoneNumber,
                                     Direccion = u.Direccion,
                                     Rol = r.Name,
                                     Creado = u.Creado,
                                     Modificado = u.Modificado,
                                     ModificadoPor = x == null ? string.Empty : x.Nombre,
                                     CreadPor = x == null ? string.Empty : y.Nombre,
                                     Inactivo = u.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AppUsuarios == null)
            {
                return Problem("Entity set 'AppDbContext.AppUsuario'  is null.");
            }
            var appUsuario = await _context.AppUsuarios.FindAsync(id);
            if (appUsuario != null)
            {
                _context.AppUsuarios.Update(appUsuario);
                appUsuario.Modificado = DateTime.Now;
                appUsuario.ModificadoNombreUsuario = _user.GetUserName();
                appUsuario.Inactivo = true;
                _context.Entry(appUsuario).Property(c => c.Creado).IsModified = false;
                _context.Entry(appUsuario).Property(c => c.CreadoNombreUsuario).IsModified = false;
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
