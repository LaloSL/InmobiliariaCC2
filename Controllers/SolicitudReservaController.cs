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

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index()
        {
            var solicitudes = _repoSolicitud.ObtenerTodas();

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult ConfirmarReserva(int id)
        {

            var solicitud =
                _repoSolicitud.ObtenerPorId(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] =
                    "La solicitud ya fue procesada.";

                return RedirectToAction(nameof(Index));
            }

            if (solicitud.FechaHasta <= solicitud.FechaDesde)
            {
                TempData["Error"] =
                    "Las fechas de la solicitud no son válidas.";

                return RedirectToAction(nameof(Index));
            }

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

            var inquilino =
                _repoInquilino.ObtenerPorDni(
                    solicitud.Dni
                );


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

            int resultado =
                _repoReserva.Guardar(reserva);

            if (resultado <= 0)
            {
                TempData["Error"] =
                    "No se pudo crear la reserva.";

                return RedirectToAction(nameof(Index));
            }


            _repoSolicitud.CambiarEstado(
                solicitud.IdSolicitud,
                "Confirmada"
            );


            TempData["Mensaje"] =
                $"La solicitud N.º {solicitud.IdSolicitud} " +
                "fue confirmada y se creó la reserva correctamente.";

            return RedirectToAction(nameof(Index));
        }


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