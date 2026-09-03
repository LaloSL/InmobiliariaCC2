using Microsoft.AspNetCore.Mvc;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble _repo;

        public TipoInmuebleController(RepositorioTipoInmueble repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var lista = _repo.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var tipo = _repo.ObtenerPorId(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult DeleteAsync(int id)
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