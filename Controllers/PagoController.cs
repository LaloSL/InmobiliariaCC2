using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(idReserva);


            if (reserva == null)
            {
                return NotFound();
            }


            var pagos =
                _repoPago.ObtenerPorReserva(idReserva);


            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(idReserva);


            decimal montoAlquiler =
                reserva.MontoTotal;


            decimal multa =
                reserva.Multa;


            decimal montoTotalAdeudado =
                montoAlquiler + multa;


            decimal saldoPendiente =
                montoTotalAdeudado - totalPagado;


            if (saldoPendiente < 0)
            {
                saldoPendiente = 0;
            }


            ViewBag.Reserva =
                reserva;

            ViewBag.MontoAlquiler =
                montoAlquiler;

            ViewBag.Multa =
                multa;

            ViewBag.MontoTotalAdeudado =
                montoTotalAdeudado;

            ViewBag.TotalPagado =
                totalPagado;

            ViewBag.SaldoPendiente =
                saldoPendiente;


            return View(pagos);
        }

        [HttpGet]
        public IActionResult Create(int idReserva)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(idReserva);


            if (reserva == null)
            {
                return NotFound();
            }


            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(idReserva);


            decimal montoTotalAdeudado =
                reserva.MontoTotal +
                reserva.Multa;


            decimal saldoPendiente =
                montoTotalAdeudado -
                totalPagado;


            if (saldoPendiente <= 0)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra totalmente pagada.";


                return RedirectToAction(
                    "Index",
                    new
                    {
                        idReserva = idReserva
                    }
                );
            }


            var pago =
                new Pago
                {
                    IdReserva =
                        idReserva,

                    FechaPago =
                        DateTime.Now,

                    Estado =
                        true
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
                _repoReserva.ObtenerPorIdConDetalles(
                    pago.IdReserva
                );


            if (reserva == null)
            {
                ModelState.AddModelError(
                    "",
                    "La reserva seleccionada no existe."
                );


                return View(pago);
            }


            decimal totalPagado =
                _repoPago.ObtenerTotalPagado(
                    pago.IdReserva
                );


            decimal montoTotalAdeudado =
                reserva.MontoTotal +
                reserva.Multa;


            decimal saldoPendiente =
                montoTotalAdeudado -
                totalPagado;




            if (saldoPendiente <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "La reserva ya se encuentra totalmente pagada."
                );
            }


            if (string.IsNullOrWhiteSpace(
                pago.Concepto))
            {
                ModelState.AddModelError(
                    "Concepto",
                    "El concepto del pago es obligatorio."
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


            if (string.IsNullOrWhiteSpace(
                pago.MedioPago))
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


            pago.Concepto =
                pago.Concepto.Trim();


            pago.Estado =
                true;


            pago.FechaPago =
                DateTime.Now;


            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                pago.IdUsuarioCreacion =
                    idUsuario;
            }

            int idPago =
                _repoPago.Guardar(pago);


            decimal nuevoTotalPagado =
                totalPagado +
                pago.Monto;


            decimal nuevoSaldo =
                montoTotalAdeudado -
                nuevoTotalPagado;


            if (nuevoSaldo <= 0)
            {
                TempData["Mensaje"] =
                    $"Pago N.º {idPago} registrado correctamente. " +
                    "La reserva quedó totalmente pagada.";
            }
            else if (
                nuevoTotalPagado >=
                reserva.MontoMinimoReserva)
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
                new
                {
                    idReserva =
                        pago.IdReserva
                }
            );
        }



        // ==========================================================
        // PAGAR MULTA POR TERMINACIÓN ANTICIPADA - GET
        // ==========================================================

        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult PagarMulta(
            int idReserva,
            DateTime fechaTerminacion)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra finalizada.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }

            if (fechaTerminacion <= reserva.FechaDesde ||
                fechaTerminacion >= reserva.FechaHasta)
            {
                TempData["Error"] =
                    "La fecha de terminación anticipada no es válida.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }


            // ------------------------------------------------------
            // CALCULAR MULTA EN EL SERVIDOR
            // ------------------------------------------------------

            int diasPactados =
                (reserva.FechaHasta -
                 reserva.FechaDesde).Days;

            int diasCumplidos =
                (fechaTerminacion -
                 reserva.FechaDesde).Days;

            int diasRestantes =
                (reserva.FechaHasta -
                 fechaTerminacion).Days;


            decimal costoTotalRestante =
                diasRestantes *
                reserva.MontoDia;


            decimal multa;

            if (diasCumplidos < (diasPactados / 2.0))
            {
                multa =
                    costoTotalRestante * 0.50m;
            }
            else
            {
                multa =
                    costoTotalRestante * 0.25m;
            }


            if (multa <= 0)
            {
                TempData["Error"] =
                    "No fue posible calcular correctamente la multa.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }


            var pago =
                new Pago
                {
                    IdReserva =
                        idReserva,

                    Concepto =
                        "Multa por terminación anticipada",

                    Monto =
                        multa,

                    FechaPago =
                        DateTime.Now,

                    MedioPago =
                        string.Empty,

                    Observacion =
                        null,

                    Estado =
                        true
                };


            ViewBag.Reserva =
                reserva;

            ViewBag.FechaTerminacion =
                fechaTerminacion;

            ViewBag.Multa =
                multa;


            return View(pago);
        }


        // ==========================================================
        // PAGAR MULTA POR TERMINACIÓN ANTICIPADA - POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult PagarMulta(
            int idReserva,
            DateTime fechaTerminacion,
            string medioPago,
            string? observacion)
        {
            // ------------------------------------------------------
            // BUSCAR RESERVA
            // ------------------------------------------------------

            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(
                    idReserva
                );


            if (reserva == null)
            {
                return NotFound();
            }


            // ------------------------------------------------------
            // VALIDAR QUE SIGA ACTIVA
            // ------------------------------------------------------

            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra finalizada.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }


            // ------------------------------------------------------
            // VALIDAR FECHA
            // ------------------------------------------------------

            if (fechaTerminacion <= reserva.FechaDesde ||
                fechaTerminacion >= reserva.FechaHasta)
            {
                TempData["Error"] =
                    "La fecha de terminación anticipada no es válida.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }


            // ------------------------------------------------------
            // RECALCULAR MULTA
            // NUNCA CONFIAMOS EN UN MONTO ENVIADO POR LA VISTA
            // ------------------------------------------------------

            int diasPactados =
                (reserva.FechaHasta -
                 reserva.FechaDesde).Days;


            int diasCumplidos =
                (fechaTerminacion -
                 reserva.FechaDesde).Days;


            int diasRestantes =
                (reserva.FechaHasta -
                 fechaTerminacion).Days;


            decimal costoTotalRestante =
                diasRestantes *
                reserva.MontoDia;


            decimal multaCalculada;


            if (diasCumplidos < (diasPactados / 2.0))
            {
                multaCalculada =
                    costoTotalRestante *
                    0.50m;
            }
            else
            {
                multaCalculada =
                    costoTotalRestante *
                    0.25m;
            }


            if (multaCalculada <= 0)
            {
                TempData["Error"] =
                    "No fue posible calcular correctamente la multa.";

                return RedirectToAction(
                    "Details",
                    "Reserva",
                    new
                    {
                        id = idReserva
                    }
                );
            }


            // ------------------------------------------------------
            // VALIDAR MEDIO DE PAGO
            // ------------------------------------------------------

            if (string.IsNullOrWhiteSpace(medioPago))
            {
                var pagoVista =
                    new Pago
                    {
                        IdReserva =
                            idReserva,

                        Concepto =
                            "Multa por terminación anticipada",

                        Monto =
                            multaCalculada,

                        FechaPago =
                            DateTime.Now,

                        MedioPago =
                            string.Empty,

                        Observacion =
                            observacion,

                        Estado =
                            true
                    };


                ViewBag.Reserva =
                    reserva;

                ViewBag.FechaTerminacion =
                    fechaTerminacion;

                ViewBag.Multa =
                    multaCalculada;


                ModelState.AddModelError(
                    "MedioPago",
                    "Debe seleccionar un medio de pago."
                );


                return View(pagoVista);
            }


            // ------------------------------------------------------
            // OBTENER USUARIO AUTENTICADO
            // ------------------------------------------------------

            int? idUsuarioActual =
                null;


            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                idUsuarioActual =
                    idUsuario;
            }


            // ------------------------------------------------------
            // CREAR EL PAGO
            // ------------------------------------------------------

            var pago =
                new Pago
                {
                    IdReserva =
                        idReserva,

                    Concepto =
                        "Multa por terminación anticipada",

                    Monto =
                        multaCalculada,

                    FechaPago =
                        DateTime.Now,

                    MedioPago =
                        medioPago.Trim(),

                    Observacion =
                        string.IsNullOrWhiteSpace(observacion)
                            ? null
                            : observacion.Trim(),

                    Estado =
                        true,

                    IdUsuarioCreacion =
                        idUsuarioActual
                };


            // ------------------------------------------------------
            // GUARDAR PAGO
            // ------------------------------------------------------

            int idPago =
                _repoPago.Guardar(
                    pago
                );


            // ------------------------------------------------------
            // FINALIZAR RESERVA
            // SOLAMENTE DESPUÉS DE REGISTRAR LA MULTA
            // ------------------------------------------------------

            _repoReserva.FinalizarAnticipadamente(
                idReserva,
                fechaTerminacion,
                multaCalculada,
                idUsuarioActual
            );


            // ------------------------------------------------------
            // MENSAJE
            // ------------------------------------------------------

            TempData["Mensaje"] =
                $"Terminación anticipada registrada correctamente. " +
                $"Pago N.º {idPago} por multa de " +
                $"{multaCalculada:C} registrado correctamente.";


            // ------------------------------------------------------
            // VOLVER AL DETALLE
            // ------------------------------------------------------

            return RedirectToAction(
                "Details",
                "Reserva",
                new
                {
                    id = idReserva
                }
            );
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pago =
                _repoPago.ObtenerPorId(id);


            if (pago == null)
            {
                return NotFound();
            }


            if (!pago.Estado)
            {
                TempData["Error"] =
                    "No se puede modificar un pago anulado.";


                return RedirectToAction(
                    "Index",
                    new
                    {
                        idReserva =
                            pago.IdReserva
                    }
                );
            }


            return View(pago);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int idPago,
            string concepto)
        {
            var pago =
                _repoPago.ObtenerPorId(
                    idPago
                );


            if (pago == null)
            {
                return NotFound();
            }


            if (!pago.Estado)
            {
                TempData["Error"] =
                    "No se puede modificar un pago anulado.";


                return RedirectToAction(
                    "Index",
                    new
                    {
                        idReserva =
                            pago.IdReserva
                    }
                );
            }


            if (string.IsNullOrWhiteSpace(
                concepto))
            {
                ModelState.AddModelError(
                    "Concepto",
                    "El concepto del pago es obligatorio."
                );


                pago.Concepto =
                    concepto ??
                    string.Empty;


                return View(pago);
            }


            if (concepto.Trim().Length > 100)
            {
                ModelState.AddModelError(
                    "Concepto",
                    "El concepto no puede superar los 100 caracteres."
                );


                pago.Concepto =
                    concepto;


                return View(pago);
            }


            _repoPago.ModificarConcepto(
                idPago,
                concepto.Trim()
            );


            TempData["Mensaje"] =
                "El concepto del pago fue modificado correctamente.";


            return RedirectToAction(
                "Index",
                new
                {
                    idReserva =
                        pago.IdReserva
                }
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


            if (!pago.Estado)
            {
                TempData["Error"] =
                    "El pago ya se encuentra anulado.";


                return RedirectToAction(
                    "Index",
                    new
                    {
                        idReserva =
                            pago.IdReserva
                    }
                );
            }

            int? idUsuarioAnulacion =
                null;


            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                idUsuarioAnulacion =
                    idUsuario;
            }


            _repoPago.Anular(
                id,
                idUsuarioAnulacion
            );


            TempData["Mensaje"] =
                "El pago fue anulado correctamente.";


            return RedirectToAction(
                "Index",
                new
                {
                    idReserva =
                        pago.IdReserva
                }
            );
        }


        private void CargarResumenReserva(
            Reserva reserva,
            decimal totalPagado)
        {
            decimal montoTotalAdeudado =
                reserva.MontoTotal +
                reserva.Multa;


            decimal saldoPendiente =
                montoTotalAdeudado -
                totalPagado;


            if (saldoPendiente < 0)
            {
                saldoPendiente = 0;
            }


            decimal faltaParaMinimo =
                reserva.MontoMinimoReserva -
                totalPagado;


            if (faltaParaMinimo < 0)
            {
                faltaParaMinimo = 0;
            }


            ViewBag.Reserva =
                reserva;


            ViewBag.MontoAlquiler =
                reserva.MontoTotal;


            ViewBag.Multa =
                reserva.Multa;


            ViewBag.MontoTotalAdeudado =
                montoTotalAdeudado;


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