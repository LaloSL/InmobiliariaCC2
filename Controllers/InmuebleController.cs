using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioPropietario _repoPropietario;
        private readonly RepositorioTipoInmueble _repoTipoInmueble;

        public InmuebleController(
            RepositorioInmueble repoInmueble,
            RepositorioPropietario repoPropietario,
            RepositorioTipoInmueble repoTipoInmueble)
        {
            _repoInmueble = repoInmueble;
            _repoPropietario = repoPropietario;
            _repoTipoInmueble = repoTipoInmueble;
        }

        public IActionResult Index()
        {
            var lista = _repoInmueble.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);

            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        public IActionResult Create()
        {
            CargarDesplegables();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                _repoInmueble.Guardar(inmueble);

                TempData["Mensaje"] =
                    "Inmueble registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            CargarDesplegables();

            return View(inmueble);
        }

        private void CargarDesplegables()
        {
            var propietarios = _repoPropietario
                .ObtenerTodos()
                .Select(p => new
                {
                    p.IdPropietario,
                    NombreCompleto = $"{p.Nombre} {p.Apellido}"
                })
                .ToList();

            ViewBag.Propietarios = new SelectList(
                propietarios,
                "IdPropietario",
                "NombreCompleto"
            );

            ViewBag.Tipos = new SelectList(
                _repoTipoInmueble.ObtenerTodos(),
                "IdTipo",
                "Nombre"
            );
        }

        public IActionResult BuscarDisponibles(
            DateTime? desde,
            DateTime? hasta)
        {
            var lista = new List<Inmueble>();

            if (desde.HasValue && hasta.HasValue)
            {
                if (hasta <= desde)
                {
                    ViewBag.Error =
                        "La fecha 'Hasta' debe ser posterior a la fecha 'Desde'.";

                    return View(lista);
                }

                ViewBag.Desde =
                    desde.Value.ToString("yyyy-MM-dd");

                ViewBag.Hasta =
                    hasta.Value.ToString("yyyy-MM-dd");

                lista = _repoInmueble.BuscarDisponibles(
                    desde.Value,
                    hasta.Value
                );
            }

            return View(lista);
        }
    }
}