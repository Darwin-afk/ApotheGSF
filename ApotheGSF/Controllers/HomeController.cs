using ApotheGSF.Clases;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;
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

        public HomeController(ILogger<HomeController> logger,
                              SignInManager<AppUsuario> signInManager,
                              UserManager<AppUsuario> userManager,
                              IHttpContextAccessor accessor,
                              IWebHostEnvironment env,
                              IOptions<AppSettings> _appSettings,
                              AppDbContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _user = accessor.HttpContext.User;
            webRoot = env.WebRootPath;
            appSettings = _appSettings;
            _context = context;
        }

        public IActionResult Index()
        {
            VerificarInventario();

            return View();
        }

        private bool VerificarInventario()
        {
            Notificaciones.Mensajes = new List<string>();

            //obtener cada medicamento con su cajas incluidas
            List<Medicamentos> medicamentos = _context.Medicamentos.Where(m => m.Inactivo == false).Include(m => m.MedicamentosCajas.Where(mc=>mc.CantidadUnidad > 0)).ToList();

            if (medicamentos == null)
                return true;

            //por cada medicamento
            foreach(var medicamento in medicamentos)
            {
                int diasRestantes;
                int cajasVencidas = 0;

                if (medicamento.MedicamentosCajas.Count == 0)
                    continue;

                //por cada caja
                foreach(var caja in medicamento.MedicamentosCajas)
                {
                    //verificar la diferencia de su fecha de vencimiento con la fecha actual
                    diasRestantes = (caja.FechaVencimiento - DateTime.Now).Days;

                    //si es menor que x cantidad de dias
                    if(diasRestantes <= 30 && diasRestantes > 0)
                    {
                        //si la caja esta activa se desactiva
                        if(caja.Inactivo == false)
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
                if(cajasVencidas > 0)
                    Notificaciones.Mensajes.Add($"{cajasVencidas} cajas de {medicamento.Nombre} estan por vencerse");

            }

            //obtener de nuevo cada medicamento con su cajas incluidas por si hubo cajas que se desactivaron
            medicamentos = _context.Medicamentos.Where(m => m.Inactivo == false).Include(m => m.MedicamentosCajas.Where(mc => mc.Inactivo == false && mc.CantidadUnidad > 0)).ToList();
            //por cada medicamento
            foreach (var medicamento in medicamentos)
            {
                //si su cantidad de cajas es menor x limite
                if(medicamento.MedicamentosCajas.Count < 20)
                {
                    //se agrega una notificacion de reabastecimiento
                    Notificaciones.Mensajes.Add($"{medicamento.Nombre} le queda poca mercancia, desea <a href=\"/Medicamentos/EnviarCorreo?codigoMedicamento={medicamento.Codigo}\">solicitar mas</a>");
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
                    usuarioInactivo = (bool)_userManager.FindByNameAsync(model.Usuario).Result.Inactivo;
                }
                catch
                {
                    ModelState.AddModelError("", "Usuario/Contraseña Inválidos");
                    return View(model);
                }

                if (usuarioInactivo)
                {
                    ModelState.AddModelError("", "Usuario/Contraseña Inválidos");
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
            ModelState.AddModelError("", "Usuario/Contraseña Inválidos");
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
                ModelState.AddModelError("", "Contraseña actual incorrecta.");
            }

            //verifica si el campo confirmar contraseña es igual a la nueva contraseña
            if (!modelo.Password.Equals(modelo.ConfirmarPassword))
            {
                ModelState.AddModelError("", "Las contraseñas con coinciden.");
            }

            ModelState.Remove("Nombre");//no se toma en cuenta el nombre al validar
            ModelState.Remove("Apellido");//no se toma en cuenta el apellido al validar

            if (ModelState.IsValid)
            {
                //Reinicia la contraseña del usuario
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, modelo.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("", result.Errors.FirstOrDefault().ToString());
            }

            return View(modelo);
        }

        #endregion

        #region PerfilUsuario
        public async Task<IActionResult> PerfilUsuario(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }

            AppUsuario modelo = await _userManager.FindByIdAsync(id.ToString());

            if (modelo == null)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }
            //si encuentra al usuario lo manda a su vista con sus campos
            PerfilUsuarioViewModel perfil = new PerfilUsuarioViewModel()
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Email = modelo.Email,
                Telefono = modelo.PhoneNumber,
                Foto = modelo.Foto
            };

            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerfilUsuario([Bind("Id, Nombre, Apellido, Email, Telefono, Foto")] PerfilUsuarioViewModel modelo, IFormFile logo, string removeLogo)
        {
            //Valida que si se cambia el correo no exista otro usuario con el mismo asignado.
            var u = await _userManager.FindByEmailAsync(modelo.Email); //No se puede registrar el mismo correo en el sistema dos veces, no importa la Org.
            if (u != null && u.Id != modelo.Codigo)
            {
                ModelState.AddModelError("", string.Format("El correo {0} ya está registrado.", modelo.Email));
            }

            if (!modelo.Email.IsValidEmail())
            {
                ModelState.AddModelError("", string.Format("El Email {0} no es correcto.", modelo.Email));
            }

            ModelState.Remove("Foto");
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
                        ModelState.AddModelError(string.Empty, error.Description);
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
                    return RedirectToAction("PerfilUsuario", "Home");
                }

            }
            return View(modelo);
        }
        #endregion

        public IActionResult VerNotificaciones()
        {
            return View();
        }
    }
}