using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using p_mvcHacienda.Models;
using Bib_Hacienda.Aplicacion;

namespace p_mvcHacienda.Controllers
{
    public class AccountController : Controller
    {
        private readonly ServicioAutenticacion _servicioAutenticacion;

        public AccountController(ServicioAutenticacion servicioAutenticacion)
        {
            _servicioAutenticacion = servicioAutenticacion;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                bool credencialesValidas = _servicioAutenticacion.ValidarCredenciales(model.Username, model.Password);

                if (credencialesValidas)
                {
                    // La generación del ClaimsPrincipal permanece en AccountController porque
                    // ClaimsIdentity/ClaimTypes son tipos de ASP.NET Core, no del dominio.
                    // ServicioAutenticacion solo valida (bool); el usuario se recupera aparte
                    // con buscar_usuario, igual que hacía UsuarioService.ValidateUserAsync.
                    var usuario = _servicioAutenticacion.buscar_usuario(model.Username);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                    };

                    await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(new ClaimsIdentity(claims, "CookieAuth")));

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
