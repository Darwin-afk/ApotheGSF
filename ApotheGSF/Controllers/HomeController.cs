using ApotheGSF.Models;
using ApotheGSF.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApotheGSF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<AppUsuario> _signInManager;
        private readonly UserManager<AppUsuario> _userManager;

        public HomeController(ILogger<HomeController> logger,
                              SignInManager<AppUsuario> signInManager,
                              UserManager<AppUsuario> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
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
                    usuarioInactivo = _userManager.FindByNameAsync(model.Usuario).Result.Inactivo;
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

                var result = await _signInManager.PasswordSignInAsync(model.Usuario,
                   model.Password, false, false);

                if (result.Succeeded)
                {
                    var usuario = await _userManager.FindByNameAsync(model.Usuario); //consigue los datos del usurio conectado

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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }
    }
}