using Microsoft.AspNetCore.Mvc;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;
using Microsoft.AspNetCore.Authorization;


namespace InmobiliariaCC2.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble _repo;

        public TipoInmuebleController(RepositorioTipoInmueble repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index()
        {
            var lista = _repo.ObtenerTodos();
            return View(lista);
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Details(int id)
        {
            var tipo = _repo.ObtenerPorId(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(TipoInmueble tipo)
        {
            if (ModelState.IsValid)
            {
                _repo.Guardar(tipo);

                TempData["Mensaje"] =
                    "Tipo de inmueble creado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var tipo = _repo.ObtenerPorId(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, TipoInmueble tipo)
        {
            if (id != tipo.IdTipo)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _repo.Editar(tipo);

                TempData["Mensaje"] =
                    "Tipo de inmueble actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]

        public IActionResult Delete(int id)
        {
            try
            {
                int filas = _repo.BajaLogica(id);

                if (filas > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Tipo de inmueble eliminado."
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "No se encontró el registro."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }
    }
}