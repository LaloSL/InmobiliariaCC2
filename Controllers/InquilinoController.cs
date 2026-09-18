using Microsoft.AspNetCore.Mvc;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaCC2.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino _repositorio;

        public InquilinoController(RepositorioInquilino repositorio)
        {
            _repositorio = repositorio;
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index(
            string? buscar,
            int pagina = 1)
        {
            const int cantidadPorPagina = 5;

            if (pagina < 1)
            {
                pagina = 1;
            }

            int totalRegistros =
                _repositorio.ContarPaginados(buscar);

            int totalPaginas =
                (int)Math.Ceiling(
                    totalRegistros /
                    (double)cantidadPorPagina
                );

            if (totalPaginas > 0 &&
                pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }

            var lista =
                _repositorio.ObtenerPaginados(
                    buscar,
                    pagina,
                    cantidadPorPagina
                );

            ViewBag.Buscar = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;

            return View(lista);
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Details(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Guardar(inquilino);

                TempData["Mensaje"] =
                    "Inquilino creado exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(inquilino);
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Edit(int id)
        {
            var inquilino =
                _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Edit(
            int id,
            Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repositorio.Modificar(inquilino);

                TempData["Mensaje"] =
                    "Inquilino actualizado exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inquilino =
                _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Baja(id);

            TempData["Mensaje"] =
                "Inquilino eliminado exitosamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}