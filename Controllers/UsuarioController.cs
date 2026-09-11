using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace InmobiliariaCC2.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RepositorioUsuario repositorio;
        private readonly PasswordHasher<Usuario> passwordHasher;

        public UsuarioController(RepositorioUsuario repositorio)
        {
            this.repositorio = repositorio;
            passwordHasher = new PasswordHasher<Usuario>();
        }

        // GET: Usuario/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 1. Buscar usuario por email
            Usuario? usuario = repositorio.ObtenerPorEmail(email);

            // 2. Verificar que exista
            if (usuario == null)
            {
                ViewBag.Error = "Email o contraseña incorrectos.";
                return View();
            }

            // 3. Verificar que esté activo
            if (!usuario.Estado)
            {
                ViewBag.Error = "El usuario se encuentra dado de baja.";
                return View();
            }

            // 4. Verificar contraseña
            var resultado = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                password
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Email o contraseña incorrectos.";
                return View();
            }

            // 5. Crear Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.Rol)
    };

            // 6. Crear identidad
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // 7. Generar cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            // 8. Redirigir al Home
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Home");
        }
    }
}