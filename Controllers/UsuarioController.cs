using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace InmobiliariaCC2.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RepositorioUsuario repositorio;
        private readonly PasswordHasher<Usuario> passwordHasher;
        private readonly IWebHostEnvironment environment;

        public UsuarioController(
            RepositorioUsuario repositorio,
            IWebHostEnvironment environment)
        {
            this.repositorio = repositorio;
            this.environment = environment;

            passwordHasher = new PasswordHasher<Usuario>();
        }


        // =========================================================
        // LOGIN
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(
            string email,
            string password)
        {
            var usuario = repositorio.ObtenerPorEmail(email);

            if (usuario == null)
            {
                ViewBag.Error =
                    "Email o contraseña incorrectos.";

                return View();
            }

            if (!usuario.Estado)
            {
                ViewBag.Error =
                    "El usuario se encuentra dado de baja.";

                return View();
            }

            var resultado =
                passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    password
                );

            if (resultado ==
                PasswordVerificationResult.Failed)
            {
                ViewBag.Error =
                    "Email o contraseña incorrectos.";

                return View();
            }

            // Crear claims del usuario
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    usuario.Nombre
                ),

                new Claim(
                    ClaimTypes.Email,
                    usuario.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol
                ),

                new Claim(
                    "Avatar",
                    usuario.Avatar ?? ""
                )
            };

            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme
                );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,

                new ClaimsPrincipal(claimsIdentity)
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }


        // =========================================================
        // MI PERFIL
        // =========================================================

        [HttpGet]
        [Authorize]
        public IActionResult Perfil()
        {
            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (!int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                return Unauthorized();
            }

            var usuario =
                repositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        // =========================================================
        // EDITAR PERFIL - GET
        // =========================================================

        [HttpGet]
        [Authorize]
        public IActionResult EditarPerfil()
        {
            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (!int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                return Unauthorized();
            }

            var usuario =
                repositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        // =========================================================
        // EDITAR PERFIL - POST
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(
            string nombre,
            string email)
        {
            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (!int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                return Unauthorized();
            }

            var usuarioActual =
                repositorio.ObtenerPorId(idUsuario);

            if (usuarioActual == null)
            {
                return NotFound();
            }


            // Validar nombre
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ModelState.AddModelError(
                    "Nombre",
                    "El nombre es obligatorio."
                );
            }


            // Validar email
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "El email es obligatorio."
                );
            }


            if (!ModelState.IsValid)
            {
                usuarioActual.Nombre = nombre;
                usuarioActual.Email = email;

                return View(usuarioActual);
            }


            var actualizado =
                repositorio.ActualizarPerfil(
                    idUsuario,
                    nombre.Trim(),
                    email.Trim()
                );


            if (!actualizado)
            {
                ModelState.AddModelError(
                    "",
                    "No se pudo actualizar el perfil."
                );

                usuarioActual.Nombre = nombre;
                usuarioActual.Email = email;

                return View(usuarioActual);
            }


            // Volvemos a consultar el usuario
            // para obtener todos los datos actualizados,
            // incluido el avatar.
            var usuario =
                repositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                return NotFound();
            }


            // Renovamos los claims.
            // IMPORTANTE: también conservamos Avatar.
            await RenovarClaims(usuario);


            TempData["Mensaje"] =
                "Los datos del perfil fueron actualizados correctamente.";

            return RedirectToAction(
                nameof(Perfil)
            );
        }


        // =========================================================
        // CAMBIAR CONTRASEÑA - GET
        // =========================================================

        [HttpGet]
        [Authorize]
        public IActionResult CambiarPassword()
        {
            return View();
        }


        // =========================================================
        // CAMBIAR CONTRASEÑA - POST
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarPassword(
            string passwordActual,
            string passwordNueva,
            string confirmarPassword)
        {
            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (!int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                return Unauthorized();
            }


            var usuario =
                repositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                return NotFound();
            }


            // Contraseña actual obligatoria
            if (string.IsNullOrWhiteSpace(
                passwordActual))
            {
                ViewBag.Error =
                    "Debe ingresar la contraseña actual.";

                return View();
            }


            // Nueva contraseña obligatoria
            if (string.IsNullOrWhiteSpace(
                passwordNueva))
            {
                ViewBag.Error =
                    "Debe ingresar una nueva contraseña.";

                return View();
            }


            // Longitud mínima
            if (passwordNueva.Length < 6)
            {
                ViewBag.Error =
                    "La nueva contraseña debe tener al menos 6 caracteres.";

                return View();
            }


            // Confirmación
            if (passwordNueva !=
                confirmarPassword)
            {
                ViewBag.Error =
                    "La nueva contraseña y su confirmación no coinciden.";

                return View();
            }


            // Verificar contraseña actual
            var resultado =
                passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    passwordActual
                );


            if (resultado ==
                PasswordVerificationResult.Failed)
            {
                ViewBag.Error =
                    "La contraseña actual es incorrecta.";

                return View();
            }


            // Evitar reutilizar la contraseña actual
            var mismaPassword =
                passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    passwordNueva
                );


            if (mismaPassword !=
                PasswordVerificationResult.Failed)
            {
                ViewBag.Error =
                    "La nueva contraseña debe ser diferente de la contraseña actual.";

                return View();
            }


            // Generar hash de la nueva contraseña
            string nuevoHash =
                passwordHasher.HashPassword(
                    usuario,
                    passwordNueva
                );


            var actualizado =
                repositorio.ActualizarPassword(
                    idUsuario,
                    nuevoHash
                );


            if (!actualizado)
            {
                ViewBag.Error =
                    "No se pudo actualizar la contraseña.";

                return View();
            }


            TempData["Mensaje"] =
                "La contraseña fue actualizada correctamente.";

            return RedirectToAction(
                nameof(Perfil)
            );
        }


        // =========================================================
        // ACTUALIZAR AVATAR
        // =========================================================

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarAvatar(
            IFormFile? avatar)
        {
            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (!int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                return Unauthorized();
            }


            var usuario =
                repositorio.ObtenerPorId(idUsuario);


            if (usuario == null)
            {
                return NotFound();
            }


            // Debe seleccionar archivo
            if (avatar == null ||
                avatar.Length == 0)
            {
                TempData["Error"] =
                    "Debe seleccionar una imagen.";

                return RedirectToAction(
                    nameof(Perfil)
                );
            }


            // Máximo 5 MB
            if (avatar.Length >
                5 * 1024 * 1024)
            {
                TempData["Error"] =
                    "La imagen no puede superar los 5 MB.";

                return RedirectToAction(
                    nameof(Perfil)
                );
            }


            // Validar extensión
            var extension =
                Path.GetExtension(
                    avatar.FileName
                ).ToLowerInvariant();


            var extensionesPermitidas =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


            if (!extensionesPermitidas.Contains(
                extension))
            {
                TempData["Error"] =
                    "Formato no permitido. Utilice JPG, JPEG, PNG o WEBP.";

                return RedirectToAction(
                    nameof(Perfil)
                );
            }


            // Carpeta:
            // wwwroot/uploads/usuarios
            var carpeta =
                Path.Combine(
                    environment.WebRootPath,
                    "uploads",
                    "usuarios"
                );


            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(
                    carpeta
                );
            }


            // Crear nombre único
            var nombreArchivo =
                $"usuario_{idUsuario}_{Guid.NewGuid()}{extension}";


            var rutaFisica =
                Path.Combine(
                    carpeta,
                    nombreArchivo
                );


            // Guardar archivo
            using (var stream =
                new FileStream(
                    rutaFisica,
                    FileMode.Create))
            {
                await avatar.CopyToAsync(
                    stream
                );
            }


            // Ruta que se guarda en MySQL
            var rutaAvatar =
                $"/uploads/usuarios/{nombreArchivo}";


            // Guardamos temporalmente la ruta anterior.
            // No la borramos todavía por si falla
            // la actualización de la base de datos.
            string? avatarAnterior =
                usuario.Avatar;


            // Actualizar BD
            var actualizado =
                repositorio.ActualizarAvatar(
                    idUsuario,
                    rutaAvatar
                );


            if (!actualizado)
            {
                // Si falla MySQL,
                // eliminamos la nueva imagen.
                if (System.IO.File.Exists(
                    rutaFisica))
                {
                    System.IO.File.Delete(
                        rutaFisica
                    );
                }


                TempData["Error"] =
                    "No se pudo actualizar el avatar.";

                return RedirectToAction(
                    nameof(Perfil)
                );
            }


            // Si la BD se actualizó correctamente,
            // recién ahora eliminamos el avatar anterior.
            if (!string.IsNullOrWhiteSpace(
                avatarAnterior))
            {
                var rutaAnteriorRelativa =
                    avatarAnterior
                        .TrimStart('/')
                        .Replace(
                            '/',
                            Path.DirectorySeparatorChar
                        );


                var rutaAnteriorFisica =
                    Path.Combine(
                        environment.WebRootPath,
                        rutaAnteriorRelativa
                    );


                if (System.IO.File.Exists(
                    rutaAnteriorFisica))
                {
                    System.IO.File.Delete(
                        rutaAnteriorFisica
                    );
                }
            }


            // Recuperamos nuevamente al usuario.
            // Ahora ya contiene la nueva ruta del avatar.
            var usuarioActualizado =
                repositorio.ObtenerPorId(
                    idUsuario
                );


            if (usuarioActualizado == null)
            {
                return NotFound();
            }


            // Renovamos inmediatamente la cookie.
            // De esta manera el Layout recibe
            // el nuevo Claim "Avatar".
            await RenovarClaims(
                usuarioActualizado
            );


            TempData["Mensaje"] =
                "Avatar actualizado correctamente.";


            return RedirectToAction(
                nameof(Perfil)
            );
        }


        // =========================================================
        // MÉTODO PRIVADO PARA RENOVAR CLAIMS
        // =========================================================

        private async Task RenovarClaims(
            Usuario usuario)
        {
            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        usuario.IdUsuario.ToString()
                    ),

                    new Claim(
                        ClaimTypes.Name,
                        usuario.Nombre
                    ),

                    new Claim(
                        ClaimTypes.Email,
                        usuario.Email
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        usuario.Rol
                    ),

                    new Claim(
                        "Avatar",
                        usuario.Avatar ?? ""
                    )
                };


            var identity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme
                );


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,

                new ClaimsPrincipal(identity)
            );
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme
            );


            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}