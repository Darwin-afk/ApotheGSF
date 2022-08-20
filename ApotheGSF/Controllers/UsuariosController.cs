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
        public async Task<IActionResult> Index(string filter, string Mensaje = "", int pageindex = 1, string sortExpression = "", int search = 0)
        {
            if (Mensaje != "")
            {
                _notyf.Custom(Mensaje, 5, "#17D155", "fas fa-check");
            }

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
                                     Cedula = u.Cedula,
                                     Telefono = u.PhoneNumber,
                                     Direccion = u.Direccion,
                                     Usuario = u.UserName,
                                     Email = u.Email,
                                     Rol = r.Name,
                                     Inactivo = u.Inactivo

                                 }).Where(filtro.ToString()).ToListAsync();
            }

            if (listado.Count == 0 && search == 1)
                _notyf.Information("No hay otros usuarios existentes");

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

            if (id == _user.GetUserID().ToInt())
            {
                _notyf.Error("Opcion no valida");
                return RedirectToAction("Index", "Home");
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
                                     Foto = u.Foto,
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
                string error = ValidarDatos(viewModel);

                if (error != "")
                {
                    _notyf.Error(error);
                    return View(viewModel);
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
                    return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });
                }
                else
                {
                    //si el nombre de usuario existe lo actualiza
                    if (_userManager.FindByNameAsync(nuevoUsuario.UserName).Result != null)
                    {
                        AppUsuario antiguoUsuario = _userManager.FindByNameAsync(nuevoUsuario.UserName).Result;
                        antiguoUsuario.Nombre = viewModel.Nombre;
                        antiguoUsuario.Apellido = viewModel.Apellido;
                        antiguoUsuario.Foto = null;
                        antiguoUsuario.PhoneNumber = viewModel.Telefono;
                        antiguoUsuario.Cedula = viewModel.Cedula;
                        antiguoUsuario.FechaNacimiento = viewModel.FechaNacimiento;
                        antiguoUsuario.Direccion = viewModel.Direccion;
                        antiguoUsuario.Email = viewModel.Email;
                        antiguoUsuario.UserName = viewModel.Usuario;
                        antiguoUsuario.Creado = DateTime.Now;
                        antiguoUsuario.ModificadoNombreUsuario = _user.GetUserName();
                        antiguoUsuario.Modificado = DateTime.Now;
                        antiguoUsuario.ModificadoNombreUsuario = _user.GetUserName();
                        antiguoUsuario.Inactivo = false;

                        _context.Update(antiguoUsuario);
                        var resultado = await _context.SaveChangesAsync();

                        // verificar si se grabó bien, para luego asignar el rol
                        if (resultado > 0)
                        {
                            var rolesViejos = await _context.AppUsuariosRoles.Where(x => x.UserId == antiguoUsuario.Id).ToListAsync();
                            _context.RemoveRange(rolesViejos);
                            await _context.SaveChangesAsync();
                            await _userManager.AddToRoleAsync(antiguoUsuario, viewModel.Rol);
                        }

                        return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });

                    }

                    foreach (IdentityError _error in result.Errors)
                    {
                        if (_error.Code == "InvalidUserName")
                            _notyf.Error("Nombre de usuario invalido,solo puede tener letras o numeros");
                        else
                            _notyf.Error($"{_error.Code}");
                    }
                }

            }
            return View(viewModel);
        }

        private string ValidarDatos(UsuarioViewModel viewModel)
        {
            //obtener lista de usuarios
            List<AppUsuario> usuarios = _context.AppUsuarios.Where(u => u.Inactivo == false && u.Id != viewModel.Codigo).ToList();
            //si la lista no es null
            if (usuarios != null)
            {
                //por cada elemento de la lista verificar repeticion de datos
                foreach (var usuario in usuarios)
                {
                    //nombre de usuario
                    if (usuario.UserName.ToUpper() == viewModel.Usuario.ToUpper())
                    {
                        return "Este usuario ya existe";
                    }

                    //telefono
                    if (usuario.PhoneNumber == viewModel.Telefono)
                    {
                        return "Telefono existente";
                    }

                    //cedula
                    if (usuario.Cedula == viewModel.Cedula)
                    {
                        return "Cedula existente";
                    }

                    //email
                    if (usuario.Email == viewModel.Email)
                    {
                        return "Email existente";
                    }
                }
            }

            //validar fecha de nacimiento
            if (!viewModel.FechaNacimiento.ValidarFechaNacimiento())
            {
                return "Fecha de nacimiento invalida";
            }

            //validar email
            if (!viewModel.Email.IsValidEmail())
            {
                return "Email invalido";
            }

            return "";
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.AppUsuarios == null)
            {
                return NotFound();
            }

            if (id == _user.GetUserID().ToInt())
            {
                _notyf.Error("Opcion no valida");
                return RedirectToAction("Index", "Home");
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
                                     Rol = r.Name,
                                     Inactivo = u.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();

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
                    string error = ValidarDatos(viewModel);

                    if (error != "")
                    {
                        _notyf.Error(error);
                        return View(viewModel);
                    }

                    var antiguoUsuario = await _context.AppUsuarios.FirstOrDefaultAsync(x => x.Id == viewModel.Codigo);
                    antiguoUsuario.Nombre = viewModel.Nombre;
                    antiguoUsuario.Apellido = viewModel.Apellido;
                    antiguoUsuario.PhoneNumber = viewModel.Telefono;
                    antiguoUsuario.Cedula = viewModel.Cedula;
                    antiguoUsuario.FechaNacimiento = viewModel.FechaNacimiento;
                    antiguoUsuario.Direccion = viewModel.Direccion;
                    antiguoUsuario.Email = viewModel.Email;
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
                        var token = await _userManager.GeneratePasswordResetTokenAsync(antiguoUsuario);
                        await _userManager.ResetPasswordAsync(antiguoUsuario, token, viewModel.Password);

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
                _notyf.Custom("Se ha guardado exitosamente!!!", 5, "#17D155", "fas fa-check");
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

            if (id == _user.GetUserID().ToInt())
            {
                _notyf.Error("Opcion no valida");
                return RedirectToAction("Index", "Home");
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
                                     Foto = u.Foto,
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
        public async Task<bool> DeleteConfirmed(int Codigo)
        {
            if (_context.AppUsuarios == null)
            {
                _notyf.Error("No se ha podido eliminar");
                return false;
            }


            var appUsuario = await _context.AppUsuarios.FindAsync(Codigo);
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

            return true;
        }

        private bool AppUserExists(int id)
        {
            return (_context.AppUsuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
