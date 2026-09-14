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

        // GET: Inquilino
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index()
        {
            var lista = _repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Inquilino/Details/5
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

        // GET: Inquilino/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquilino/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Guardar(inquilino);
                TempData["Mensaje"] = "Inquilino creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilino/Edit/5
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Edit(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repositorio.Modificar(inquilino);
                TempData["Mensaje"] = "Inquilino actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilino/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilino/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Inquilino eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}