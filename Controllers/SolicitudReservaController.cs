using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    public class SolicitudReservaController : Controller
    {
        private readonly RepositorioSolicitudReserva _repoSolicitud;
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioReserva _repoReserva;
        private readonly RepositorioInquilino _repoInquilino;


        public SolicitudReservaController(
            RepositorioSolicitudReserva repoSolicitud,
            RepositorioInmueble repoInmueble,
            RepositorioReserva repoReserva,
            RepositorioInquilino repoInquilino)
        {
            _repoSolicitud = repoSolicitud;
            _repoInmueble = repoInmueble;
            _repoReserva = repoReserva;
            _repoInquilino = repoInquilino;
        }


        // =====================================================
        // INDEX - SOLICITUDES
        // Administrador y Empleado
        // =====================================================

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index()
        {
            var solicitudes = _repoSolicitud.ObtenerTodas();

            // Calculamos cuáles solicitudes pendientes
            // todavía pueden convertirse en una reserva.
            var disponibilidad = new Dictionary<int, bool>();

            foreach (var solicitud in solicitudes)
            {
                if (solicitud.Estado == "Pendiente")
                {
                    bool ocupado = _repoReserva.InmuebleOcupado(
                        solicitud.IdInmueble,
                        solicitud.FechaDesde,
                        solicitud.FechaHasta
                    );

                    disponibilidad[solicitud.IdSolicitud] = !ocupado;
                }
            }

            ViewBag.Disponibilidad = disponibilidad;

            return View(solicitudes);
        }


        // =====================================================
        // GET - FORMULARIO PÚBLICO DE SOLICITUD
        // =====================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create(
            int idInmueble,
            DateTime desde,
            DateTime hasta)
        {
            var inmueble =
                _repoInmueble.ObtenerPorId(idInmueble);

            if (inmueble == null)
            {
                return NotFound();
            }

            if (hasta <= desde)
            {
                TempData["Error"] =
                    "Las fechas seleccionadas no son válidas.";

                return RedirectToAction(
                    "BuscarDisponibles",
                    "Inmueble"
                );
            }

            var solicitud = new SolicitudReserva
            {
                IdInmueble = idInmueble,
                FechaDesde = desde,
                FechaHasta = hasta,
                Estado = "Pendiente"
            };

            ViewBag.Inmueble = inmueble;

            return View(solicitud);
        }


        // =====================================================
        // POST - GUARDAR SOLICITUD PÚBLICA
        // =====================================================

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SolicitudReserva solicitud)
        {
            var inmueble =
                _repoInmueble.ObtenerPorId(
                    solicitud.IdInmueble
                );

            if (inmueble == null)
            {
                ModelState.AddModelError(
                    "",
                    "El inmueble seleccionado no existe."
                );
            }

            if (solicitud.FechaHasta <= solicitud.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha hasta debe ser posterior a la fecha desde."
                );
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Inmueble = inmueble;

                return View(solicitud);
            }

            // El visitante nunca decide el estado.
            solicitud.Estado = "Pendiente";

            int idSolicitud =
                _repoSolicitud.Guardar(solicitud);

            TempData["Mensaje"] =
                $"Solicitud N.º {idSolicitud} enviada correctamente. " +
                "La solicitud queda pendiente de confirmación.";

            return RedirectToAction(
                "BuscarDisponibles",
                "Inmueble"
            );
        }


        // =====================================================
        // POST - CONFIRMAR RESERVA
        // Administrador y Empleado
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult ConfirmarReserva(int id)
        {
            // -------------------------------------------------
            // 1. Buscar la solicitud
            // -------------------------------------------------

            var solicitud =
                _repoSolicitud.ObtenerPorId(id);

            if (solicitud == null)
            {
                return NotFound();
            }


            // -------------------------------------------------
            // 2. Solamente una solicitud pendiente
            //    puede confirmarse
            // -------------------------------------------------

            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] =
                    "La solicitud ya fue procesada.";

                return RedirectToAction(nameof(Index));
            }


            // -------------------------------------------------
            // 3. Validar nuevamente las fechas
            // -------------------------------------------------

            if (solicitud.FechaHasta <= solicitud.FechaDesde)
            {
                TempData["Error"] =
                    "Las fechas de la solicitud no son válidas.";

                return RedirectToAction(nameof(Index));
            }


            // -------------------------------------------------
            // 4. COMPROBAR NUEVAMENTE DISPONIBILIDAD
            //
            // Aunque el botón estuviera habilitado,
            // volvemos a consultar la base de datos.
            // -------------------------------------------------

            bool ocupado =
                _repoReserva.InmuebleOcupado(
                    solicitud.IdInmueble,
                    solicitud.FechaDesde,
                    solicitud.FechaHasta
                );

            if (ocupado)
            {
                TempData["Error"] =
                    "No se puede confirmar la reserva porque " +
                    "el inmueble ya está ocupado para las fechas solicitadas.";

                return RedirectToAction(nameof(Index));
            }


            // -------------------------------------------------
            // 5. Buscar el inmueble
            // -------------------------------------------------

            var inmueble =
                _repoInmueble.ObtenerPorId(
                    solicitud.IdInmueble
                );

            if (inmueble == null)
            {
                TempData["Error"] =
                    "No se encontró el inmueble de la solicitud.";

                return RedirectToAction(nameof(Index));
            }


            // -------------------------------------------------
            // 6. Buscar al inquilino por DNI
            // -------------------------------------------------

            var inquilino =
                _repoInquilino.ObtenerPorDni(
                    solicitud.Dni
                );


            // -------------------------------------------------
            // 7. Si no existe, crear el inquilino
            // -------------------------------------------------

            int idInquilino;

            if (inquilino == null)
            {
                var nuevoInquilino = new Inquilino
                {
                    Dni = solicitud.Dni,
                    Nombre = solicitud.Nombre,
                    Apellido = solicitud.Apellido,
                    Email = solicitud.Email,
                    Telefono = solicitud.Telefono,

                    // La solicitud pública actualmente
                    // no solicita domicilio.
                    DireccionOrigen = null,

                    Estado = true
                };

                idInquilino =
                    _repoInquilino.Guardar(
                        nuevoInquilino
                    );

                if (idInquilino <= 0)
                {
                    TempData["Error"] =
                        "No se pudo registrar al nuevo inquilino.";

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                idInquilino =
                    inquilino.IdInquilino;
            }


            // -------------------------------------------------
            // 8. Crear la RESERVA REAL
            //
            // El precio se obtiene del inmueble.
            // No utilizamos ningún precio enviado
            // por el visitante.
            // -------------------------------------------------

            var reserva = new Reserva
            {
                IdInquilino = idInquilino,

                IdInmueble =
                    solicitud.IdInmueble,

                MontoDia =
                    inmueble.PrecioDia,

                FechaDesde =
                    solicitud.FechaDesde,

                FechaHasta =
                    solicitud.FechaHasta,

                Estado = true
            };


            // -------------------------------------------------
            // 9. Guardar Reserva
            // -------------------------------------------------

            int resultado =
                _repoReserva.Guardar(reserva);

            if (resultado <= 0)
            {
                TempData["Error"] =
                    "No se pudo crear la reserva.";

                return RedirectToAction(nameof(Index));
            }


            // -------------------------------------------------
            // 10. Marcar solicitud como CONFIRMADA
            // -------------------------------------------------

            _repoSolicitud.CambiarEstado(
                solicitud.IdSolicitud,
                "Confirmada"
            );


            TempData["Mensaje"] =
                $"La solicitud N.º {solicitud.IdSolicitud} " +
                "fue confirmada y se creó la reserva correctamente.";

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // POST - RECHAZAR SOLICITUD
        // Administrador y Empleado
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Rechazar(int id)
        {
            var solicitud =
                _repoSolicitud.ObtenerPorId(id);

            if (solicitud == null)
            {
                return NotFound();
            }


            // Solamente podemos rechazar
            // solicitudes que todavía estén pendientes.

            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] =
                    "La solicitud ya fue procesada.";

                return RedirectToAction(nameof(Index));
            }


            int resultado =
                _repoSolicitud.CambiarEstado(
                    id,
                    "Rechazada"
                );


            if (resultado <= 0)
            {
                TempData["Error"] =
                    "No se pudo rechazar la solicitud.";

                return RedirectToAction(nameof(Index));
            }


            TempData["Mensaje"] =
                $"La solicitud N.º {id} fue rechazada.";

            return RedirectToAction(nameof(Index));
        }
    }
}