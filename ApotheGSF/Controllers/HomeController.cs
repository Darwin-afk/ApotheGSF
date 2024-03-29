﻿using ApotheGSF.Clases;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace ApotheGSF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<AppUsuario> _signInManager;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly ClaimsPrincipal _user;
        private readonly string webRoot;
        private readonly IOptions<AppSettings> appSettings;
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger,
                              SignInManager<AppUsuario> signInManager,
                              UserManager<AppUsuario> userManager,
                              IHttpContextAccessor accessor,
                              IWebHostEnvironment env,
                              IOptions<AppSettings> _appSettings,
                              AppDbContext context,
                              INotyfService notyf)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _user = accessor.HttpContext.User;
            webRoot = env.WebRootPath;
            appSettings = _appSettings;
            _context = context;
            _notyf = notyf;
        }

        public IActionResult Index(string Mensaje = "")
        {
            VerificarInventario();

            if (Mensaje != "")
            {
                _notyf.Custom(Mensaje, 5, "#17D155", "fas fa-check");
            }

            return View();
        }

        private bool VerificarInventario()
        {
            Notificaciones.Mensajes = new List<string>();

            //obtener cada medicamento con su cajas incluidas
            List<Medicamentos> medicamentos = _context.Medicamentos.Where(m => m.Inactivo == false).ToList();

            if (medicamentos == null)
                return true;

            foreach (var medicamento in medicamentos)
            {
                medicamento.MedicamentosCajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo && mc.CantidadUnidad > 0).ToArray();
            }

            //por cada medicamento
            foreach (var medicamento in medicamentos)
            {
                int diasRestantes;
                int cajasVencidas = 0;

                if (medicamento.MedicamentosCajas.Count == 0)
                    continue;

                //por cada caja
                foreach (var caja in medicamento.MedicamentosCajas)
                {
                    //verificar la diferencia de su fecha de vencimiento con la fecha actual
                    diasRestantes = (caja.FechaVencimiento - DateTime.Now).Days;

                    //si es menor que x cantidad de dias
                    if (diasRestantes <= 21 && diasRestantes > 0)
                    {
                        //si la caja esta activa se desactiva
                        if (caja.Inactivo == false)
                        {
                            _context.Update(caja);
                            caja.Inactivo = true;
                            _context.SaveChanges();
                        }

                        cajasVencidas++;
                        //se agregar a notificaciones
                    }
                }

                //agregar mensaje de cajas eliminadas
                if (cajasVencidas > 0)
                    Notificaciones.Mensajes.Add($"&nbsp;&nbsp;&nbsp;{cajasVencidas} cajas de {medicamento.Nombre} estan por vencerse");

            }

            //obtener de nuevo cada medicamento con su cajas incluidas por si hubo cajas que se desactivaron
            medicamentos = _context.Medicamentos.Where(m => m.Inactivo == false).ToList();

            foreach (var medicamento in medicamentos)
            {
                medicamento.MedicamentosCajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo && mc.CantidadUnidad > 0).ToArray();
            }

            //por cada medicamento
            foreach (var medicamento in medicamentos)
            {
                //si su cantidad de cajas es menor x limite y no hay envios en curso
                if (medicamento.MedicamentosCajas.Where(mc => mc.Inactivo == false && mc.CantidadUnidad > 0).ToList().Count <= medicamento.Reorden && medicamento.EnvioPendiente == false)
                {
                    //se agrega una notificacion de reabastecimiento
                    Notificaciones.Mensajes.Add($"&nbsp;&nbsp;&nbsp;{medicamento.Nombre} le queda poca mercancia, desea <a href=\"/Medicamentos/EnviarCorreo?codigoMedicamento={medicamento.Codigo}\">solicitar mas</a>");
                }
            }

            return true;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region LogIn
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = "")
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool usuarioInactivo;
                try
                {
                    //Busca si el usuario que intenta logearse esta activo o inactivo
                    if (_userManager.FindByNameAsync(model.Usuario).Result != null)
                        usuarioInactivo = (bool)_userManager.FindByNameAsync(model.Usuario).Result.Inactivo;
                    else
                        usuarioInactivo = true;
                }
                catch
                {
                    _notyf.Error("Usuario/Contraseña Inválidos");
                    return View(model);
                }

                if (usuarioInactivo)
                {
                    _notyf.Error("Usuario/Contraseña Inválidos");
                    return View(model);
                }

                //logea al usuario
                var result = await _signInManager.PasswordSignInAsync(model.Usuario,
                   model.Password, false, false);

                if (result.Succeeded)
                {
                    var usuario = await _userManager.FindByNameAsync(model.Usuario); //consigue los datos del usurio conectado

                    //Si intento acceder a una pagina y termino en el login lo regresa a esa pagina, sino va al inicio
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            _notyf.Error("Usuario/Contraseña Inválidos");
            return View(model);
        }

        #endregion

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        #region Cambiar Password por Usuario

        [Authorize]
        public async Task<ActionResult> CambiarPassword()
        {
            var user = await _userManager.FindByIdAsync(_user.GetUserID());
            CambiarPasswordViewModel model = new CambiarPasswordViewModel
            {
                Nombre = user.Nombre,
                CodigoUsuario = Convert.ToInt32(_user.GetUserID())
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarPassword([Bind("UserID, PasswordActual, Password, ConfirmarPassword")] CambiarPasswordViewModel modelo)
        {

            var user = await _userManager.FindByIdAsync(_user.GetUserID());
            //verifica si el campo contraseña actual es correcto
            var checkPassResult = await _userManager.CheckPasswordAsync(user, modelo.PasswordActual);
            if (!checkPassResult)
            {
                _notyf.Error("Contraseña actual incorrecta");
                return View(modelo);
            }

            //verifica si el campo confirmar contraseña es igual a la nueva contraseña
            if (!modelo.Password.Equals(modelo.ConfirmarPassword))
            {
                _notyf.Error("Las contraseñas no coinciden");
                return View(modelo);
            }

            ModelState.Remove("Nombre");//no se toma en cuenta el nombre al validar
            ModelState.Remove("Apellido");//no se toma en cuenta el apellido al validar

            if (ModelState.IsValid)
            {
                //Reinicia la contraseña del usuario
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, modelo.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });
                else
                    _notyf.Error(result.Errors.FirstOrDefault().ToString());
            }

            return View(modelo);
        }

        #endregion

        #region PerfilUsuario
        [Authorize]
        public async Task<IActionResult> PerfilUsuario()
        {

            AppUsuario modelo = await _userManager.FindByIdAsync(_user.GetUserID());

            if (modelo == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            //si encuentra al usuario lo manda a su vista con sus campos
            PerfilUsuarioViewModel perfil = new PerfilUsuarioViewModel()
            {
                Codigo = modelo.Id,
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Email = modelo.Email,
                Telefono = modelo.PhoneNumber,
                Foto = modelo.Foto
            };

            return View(perfil);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerfilUsuario([Bind("Codigo, Nombre, Apellido, Email, Telefono, Foto")] PerfilUsuarioViewModel modelo, IFormFile logo, string removeLogo)
        {
            //validar telefono y email
            //obtener lista de usuarios
            List<AppUsuario> usuarios = _context.AppUsuarios.Where(u => u.Inactivo == false && u.Id != modelo.Codigo).ToList();
            //si la lista no es null
            if (usuarios != null)
            {
                //por cada elemento de la lista verificar repeticion de datos
                foreach (var usuario in usuarios)
                {
                    //telefono
                    if (usuario.PhoneNumber == modelo.Telefono)
                    {
                        _notyf.Error("Telefono existente");
                        return View(modelo);
                    }

                    //email
                    if (usuario.Email == modelo.Email)
                    {
                        _notyf.Error("Email existente");
                        return View(modelo);
                    }
                }
            }

            if (!modelo.Email.IsValidEmail())
            {
                _notyf.Error("Email invalido");
                return View(modelo);
            }

            ModelState.Remove("Foto");
            ModelState.Remove("logo");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(modelo.Codigo.ToString());
                if (user != null)
                {
                    if (removeLogo.Equals("1") || logo != null)
                    {
                        if (user.Foto != null)
                        {
                            string archivoActual = Path.Combine(webRoot, appSettings.Value.RutaImagenesUsers, user.Foto);
                            if (System.IO.File.Exists(archivoActual))
                            {
                                try
                                {
                                    GC.Collect(); ///Si no se hace esto dice algunas veces quel archivo esta siendo utilizado por otro proceso
                                    GC.WaitForPendingFinalizers();
                                    System.IO.File.Delete(archivoActual);

                                }
                                catch (Exception e)
                                {
                                    _logger.LogDebug("PerfilUsuario", e);
                                }
                            }
                        }
                        user.Foto = null;
                    }
                    if (logo != null && removeLogo.Equals("0"))
                    {
                        string fileExtension = Path.GetExtension(logo.FileName);
                        string fileName = Guid.NewGuid() + fileExtension;
                        user.Foto = fileName;
                        fileName = Path.Combine(webRoot, appSettings.Value.RutaImagenesUsers, fileName);
                        logo.CopyTo(new FileStream(fileName, FileMode.Create));
                    }
                    user.Nombre = modelo.Nombre;
                    user.Apellido = modelo.Apellido;
                    user.Email = modelo.Email;
                    user.PhoneNumber = modelo.Telefono;
                    user.Modificado = DateTime.Now;
                    user.ModificadoNombreUsuario = _user.GetUserName();
                    modelo.Foto = user.Foto;
                }
                var result = await _userManager.UpdateAsync(user); //actualiza el usuario
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _notyf.Error(error.Description);
                    }
                }
                else
                {
                    ///Si todo esta bien colocamos la foto en el claim
                    var userClaims = await _userManager.GetClaimsAsync(user); //busca todos los claims del usuario modificado

                    ///elimina los claims nombre y posicion, el idOrganizacion no cambia
                    await _userManager.RemoveClaimsAsync(user, userClaims.Where(x => x.Type.Equals("Foto")));
                    await _userManager.RemoveClaimsAsync(user, userClaims.Where(x => x.Type.Equals("Nombre")));

                    ///agrega los claims nuevamente
                    await _userManager.AddClaimAsync(user, new Claim("Nombre", user.Nombre));
                    if (user.Foto != null)
                        await _userManager.AddClaimAsync(user, new Claim("Foto", user.Foto));

                    await _signInManager.RefreshSignInAsync(user);
                    _notyf.Custom("Se ha guardado exitosamente!!!", 5, "#17D155", "fas fa-check");
                    return RedirectToAction("PerfilUsuario", "Home");
                }

            }
            return View(modelo);
        }
        #endregion

        [Authorize]
        public IActionResult VerNotificaciones()
        {
            return View();
        }
    }
}