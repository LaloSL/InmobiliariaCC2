using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    public class ReservaController : Controller
    {
        private readonly RepositorioReserva _repoReserva;
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioInquilino _repoInquilino;

        public ReservaController(
            RepositorioReserva repoReserva,
            RepositorioInmueble repoInmueble,
            RepositorioInquilino repoInquilino)
        {
            _repoReserva = repoReserva;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
        }

        public IActionResult Index()
        {
            var lista = _repoReserva.ObtenerTodas();
            return View(lista);
        }

        public IActionResult Create()
        {
            CargarSelects();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            if (reserva.FechaHasta <= reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha de finalización debe ser posterior a la fecha de inicio."
                );
            }

            if (_repoReserva.InmuebleOcupado(
                    reserva.IdInmueble,
                    reserva.FechaDesde,
                    reserva.FechaHasta))
            {
                ModelState.AddModelError(
                    "",
                    "El inmueble seleccionado ya se encuentra reservado en el rango de fechas elegido."
                );
            }

            if (ModelState.IsValid)
            {
                var inmueble = _repoInmueble.ObtenerPorId(reserva.IdInmueble);

                if (inmueble == null)
                {
                    ModelState.AddModelError(
                        "",
                        "No se encontró el inmueble seleccionado."
                    );

                    CargarSelects();
                    return View(reserva);
                }

                reserva.MontoDia = inmueble.PrecioDia;

                _repoReserva.Guardar(reserva);

                TempData["Mensaje"] =
                    "Reserva creada exitosamente.";

                return RedirectToAction(nameof(Index));
            }

            CargarSelects();

            return View(reserva);
        }

        public IActionResult Details(int id)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        public IActionResult FinalizarAnticipadamente(int id)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(id);

            if (reserva == null || !reserva.Estado)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarAnticipadamente(
            int idReserva,
            DateTime fechaTerminacion)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            if (fechaTerminacion <= reserva.FechaDesde ||
                fechaTerminacion >= reserva.FechaHasta)
            {
                ModelState.AddModelError(
                    "",
                    "La fecha de terminación debe ser posterior al inicio y anterior al fin pactado."
                );

                return View(reserva);
            }

            int diasPactados =
                (reserva.FechaHasta - reserva.FechaDesde).Days;

            int diasCumplidos =
                (fechaTerminacion - reserva.FechaDesde).Days;

            int diasRestantes =
                (reserva.FechaHasta - fechaTerminacion).Days;

            decimal costoTotalRestante =
                diasRestantes * reserva.MontoDia;

            decimal multa;

            if (diasCumplidos < (diasPactados / 2.0))
            {
                multa = costoTotalRestante * 0.50m;
            }
            else
            {
                multa = costoTotalRestante * 0.25m;
            }

            _repoReserva.FinalizarAnticipadamente(
                idReserva,
                fechaTerminacion,
                multa
            );

            TempData["Mensaje"] =
                $"Reserva finalizada. Multa calculada: ${multa:N2}";

            return RedirectToAction(
                nameof(Details),
                new { id = idReserva }
            );
        }

        private void CargarSelects()
        {
            ViewBag.Inquilinos =
                new SelectList(
                    _repoInquilino.ObtenerTodos(),
                    "IdInquilino",
                    "Nombre"
                );

            ViewBag.Inmuebles =
                new SelectList(
                    _repoInmueble.ObtenerTodos(),
                    "IdInmueble",
                    "Direccion"
                );
        }
    }
}