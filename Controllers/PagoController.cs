using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class PagoController : Controller
    {
        private readonly RepositorioPago _repoPago;
        private readonly RepositorioReserva _repoReserva;

        public PagoController(
            RepositorioPago repoPago,
            RepositorioReserva repoReserva)
        {
            _repoPago = repoPago;
            _repoReserva = repoReserva;
        }

        [HttpGet]
        public IActionResult Index(int idReserva)
        {
            var reserva = _repoReserva.ObtenerPorIdConDetalles(idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            var pagos = _repoPago.ObtenerPorReserva(idReserva);

            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(idReserva);

            decimal montoTotal =
                reserva.MontoTotal;

            decimal saldoPendiente =
                montoTotal - totalPagado;

            if (saldoPendiente < 0)
            {
                saldoPendiente = 0;
            }

            ViewBag.Reserva = reserva;
            ViewBag.TotalPagado = totalPagado;
            ViewBag.SaldoPendiente = saldoPendiente;

            return View(pagos);
        }


        [HttpGet]
        public IActionResult Create(int idReserva)
        {
            var reserva = _repoReserva.ObtenerPorIdConDetalles(idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(idReserva);

            decimal saldoPendiente =
                reserva.MontoTotal - totalPagado;

            if (saldoPendiente <= 0)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra totalmente pagada.";

                return RedirectToAction(
                    "Index",
                    new { idReserva = idReserva }
                );
            }

            var pago = new Pago
            {
                IdReserva = idReserva,
                FechaPago = DateTime.Now,
                Estado = true
            };

            CargarResumenReserva(
                reserva,
                totalPagado
            );

            return View(pago);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(pago.IdReserva);

            if (reserva == null)
            {
                ModelState.AddModelError(
                    "",
                    "La reserva seleccionada no existe."
                );

                return View(pago);
            }

            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(pago.IdReserva);

            decimal saldoPendiente =
                reserva.MontoTotal - totalPagado;


            if (saldoPendiente <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "La reserva ya se encuentra totalmente pagada."
                );
            }


            if (pago.Monto <= 0)
            {
                ModelState.AddModelError(
                    "Monto",
                    "El monto del pago debe ser mayor a cero."
                );
            }


            if (pago.Monto > saldoPendiente)
            {
                ModelState.AddModelError(
                    "Monto",
                    $"El pago no puede superar el saldo pendiente " +
                    $"de {saldoPendiente:C}."
                );
            }


            if (string.IsNullOrWhiteSpace(pago.MedioPago))
            {
                ModelState.AddModelError(
                    "MedioPago",
                    "Debe seleccionar un medio de pago."
                );
            }


            if (!ModelState.IsValid)
            {
                CargarResumenReserva(
                    reserva,
                    totalPagado
                );

                return View(pago);
            }


            pago.Estado = true;

            int idPago =
                _repoPago.Guardar(pago);



            decimal nuevoTotalPagado =
                totalPagado + pago.Monto;

            decimal nuevoSaldo =
                reserva.MontoTotal - nuevoTotalPagado;


            if (nuevoSaldo <= 0)
            {
                TempData["Mensaje"] =
                    $"Pago N.º {idPago} registrado correctamente. " +
                    "La reserva quedó totalmente pagada.";
            }
            else if (
                nuevoTotalPagado >= reserva.MontoMinimoReserva
            )
            {
                TempData["Mensaje"] =
                    $"Pago N.º {idPago} registrado correctamente. " +
                    "Se alcanzó el monto mínimo requerido para la reserva.";
            }
            else
            {
                decimal faltaParaMinimo =
                    reserva.MontoMinimoReserva -
                    nuevoTotalPagado;

                TempData["Mensaje"] =
                    $"Pago N.º {idPago} registrado correctamente. " +
                    $"Todavía faltan {faltaParaMinimo:C} " +
                    "para alcanzar el monto mínimo de reserva.";
            }


            return RedirectToAction(
                "Index",
                new { idReserva = pago.IdReserva }
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            var pago =
                _repoPago.ObtenerPorId(id);

            if (pago == null)
            {
                return NotFound();
            }

            _repoPago.Anular(id);

            TempData["Mensaje"] =
                "El pago fue anulado correctamente.";

            return RedirectToAction(
                "Index",
                new { idReserva = pago.IdReserva }
            );
        }

        private void CargarResumenReserva(
            Reserva reserva,
            decimal totalPagado)
        {
            decimal saldoPendiente =
                reserva.MontoTotal - totalPagado;

            if (saldoPendiente < 0)
            {
                saldoPendiente = 0;
            }

            decimal faltaParaMinimo =
                reserva.MontoMinimoReserva - totalPagado;

            if (faltaParaMinimo < 0)
            {
                faltaParaMinimo = 0;
            }

            ViewBag.Reserva = reserva;

            ViewBag.MontoTotal =
                reserva.MontoTotal;

            ViewBag.MontoMinimo =
                reserva.MontoMinimoReserva;

            ViewBag.TotalPagado =
                totalPagado;

            ViewBag.SaldoPendiente =
                saldoPendiente;

            ViewBag.FaltaParaMinimo =
                faltaParaMinimo;
        }
    }
}