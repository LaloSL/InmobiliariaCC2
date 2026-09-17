using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;


namespace InmobiliariaCC2.Controllers
{
    public class ReservaController : Controller
    {
        private readonly RepositorioReserva _repoReserva;
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioInquilino _repoInquilino;
        private readonly RepositorioTipoInmueble _repoTipoInmueble;
        private readonly RepositorioPago _repoPago;
        private readonly RepositorioUsuario _repoUsuario;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public ReservaController(
            RepositorioReserva repoReserva,
            RepositorioInmueble repoInmueble,
            RepositorioInquilino repoInquilino,
            RepositorioTipoInmueble repoTipoInmueble,
            RepositorioPago repoPago,
            RepositorioUsuario repoUsuario)

        {
            _repoReserva = repoReserva;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _repoTipoInmueble = repoTipoInmueble;
            _repoPago = repoPago;
            _repoUsuario = repoUsuario;
        }


        // =====================================================
        // LISTADO DE RESERVAS
        // =====================================================

        [AllowAnonymous]
        public IActionResult Index()
        {
            var lista =
                _repoReserva.ObtenerTodas();


            // La información financiera solamente se calcula
            // para Administrador y Empleado.
            if (User.IsInRole("Administrador") ||
                User.IsInRole("Empleado"))
            {
                var resumenPagos =
                    new Dictionary<int, decimal>();


                foreach (var reserva in lista)
                {
                    decimal totalPagado =
                        _repoPago.ObtenerTotalPagado(
                            reserva.IdReserva
                        );


                    resumenPagos.Add(
                        reserva.IdReserva,
                        totalPagado
                    );
                }


                ViewBag.ResumenPagos =
                    resumenPagos;
            }


            return View(lista);
        }


        // =====================================================
        // CREAR RESERVA - GET
        // =====================================================

        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            CargarSelects();

            return View();
        }


        // =====================================================
        // CREAR RESERVA - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Create(
            Reserva reserva,
            int? idTipo)
        {
            // -------------------------------------------------
            // VALIDAR FECHAS
            // -------------------------------------------------

            if (reserva.FechaHasta <= reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha de finalización debe ser posterior a la fecha de inicio."
                );
            }


            // -------------------------------------------------
            // CONTROLAR DISPONIBILIDAD
            // -------------------------------------------------

            if (reserva.IdInmueble > 0 &&
                reserva.FechaHasta > reserva.FechaDesde)
            {
                bool ocupado =
                    _repoReserva.InmuebleOcupado(
                        reserva.IdInmueble,
                        reserva.FechaDesde,
                        reserva.FechaHasta
                    );


                if (ocupado)
                {
                    ModelState.AddModelError(
                        "",
                        "El inmueble seleccionado no está disponible para las fechas elegidas."
                    );
                }
            }


            // -------------------------------------------------
            // GUARDAR RESERVA
            // -------------------------------------------------

            if (ModelState.IsValid)
            {
                var inmueble =
                    _repoInmueble.ObtenerPorId(
                        reserva.IdInmueble
                    );


                if (inmueble == null)
                {
                    ModelState.AddModelError(
                        "",
                        "No se encontró el inmueble seleccionado."
                    );


                    CargarSelects(
                        idTipo,
                        reserva.IdInmueble
                    );


                    return View(reserva);
                }


                // Precio y porcentaje tomados directamente
                // desde el inmueble.
                reserva.MontoDia =
                    inmueble.PrecioDia;


                reserva.PorcentajeReserva =
                    inmueble.PorcentajeReserva;


                // Una reserva creada normalmente
                // no tiene reserva de origen.
                reserva.IdReservaOrigen = null;


                // -------------------------------------------------
                // AUDITORÍA
                // -------------------------------------------------

                var idUsuarioClaim =
                    User.FindFirst(
                        ClaimTypes.NameIdentifier
                    )?.Value;


                if (int.TryParse(
                    idUsuarioClaim,
                    out int idUsuario))
                {
                    reserva.IdUsuarioCreacion =
                        idUsuario;
                }


                _repoReserva.Guardar(reserva);


                TempData["Mensaje"] =
                    "Reserva creada exitosamente.";


                return RedirectToAction(
                    nameof(Index)
                );
            }


            CargarSelects(
                idTipo,
                reserva.IdInmueble
            );


            return View(reserva);
        }


        // =====================================================
        // DETALLE DE RESERVA
        // =====================================================
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Details(int id)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(id);

            if (reserva == null)
            {
                return NotFound();
            }

            // ==========================================
            // AUDITORÍA
            // Solo se prepara para Administradores
            // ==========================================

            if (User.IsInRole("Administrador"))
            {
                string usuarioCreacion = "Sin registrar";
                string usuarioTerminacion = "No corresponde";

                // Usuario que creó la reserva
                if (reserva.IdUsuarioCreacion.HasValue)
                {
                    var usuario =
                        _repoUsuario.ObtenerPorId(
                            reserva.IdUsuarioCreacion.Value
                        );

                    if (usuario != null)
                    {
                        usuarioCreacion =
                            $"{usuario.Nombre} ({usuario.Rol})";
                    }
                }

                // Usuario que realizó la terminación anticipada
                if (reserva.IdUsuarioTerminacion.HasValue)
                {
                    var usuario =
                        _repoUsuario.ObtenerPorId(
                            reserva.IdUsuarioTerminacion.Value
                        );

                    if (usuario != null)
                    {
                        usuarioTerminacion =
                            $"{usuario.Nombre} ({usuario.Rol})";
                    }
                }

                ViewBag.UsuarioCreacion =
                    usuarioCreacion;

                ViewBag.UsuarioTerminacion =
                    usuarioTerminacion;
            }

            return View(reserva);
        }

        // =====================================================
        // SALIDA ANTICIPADA - GET
        // =====================================================

        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public IActionResult FinalizarAnticipadamente(int id)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(id);


            if (reserva == null)
            {
                return NotFound();
            }


            // Solo se puede iniciar una terminación anticipada
            // sobre una reserva activa.
            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra finalizada.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = id }
                );
            }


            return View(reserva);
        }


        // =====================================================
        // SALIDA ANTICIPADA - POST
        //
        // IMPORTANTE:
        // Este método solamente calcula la multa.
        // NO finaliza todavía la reserva.
        //
        // La reserva se finalizará después de registrar
        // correctamente el pago de la multa.
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult FinalizarAnticipadamente(
            int idReserva,
            DateTime fechaTerminacion)
        {
            var reserva =
                _repoReserva.ObtenerPorIdConDetalles(
                    idReserva
                );


            if (reserva == null)
            {
                return NotFound();
            }


            // -------------------------------------------------
            // VERIFICAR QUE CONTINÚE ACTIVA
            // -------------------------------------------------

            if (!reserva.Estado)
            {
                TempData["Error"] =
                    "La reserva ya se encuentra finalizada.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = idReserva }
                );
            }


            // -------------------------------------------------
            // VALIDAR FECHA REAL DE SALIDA
            // -------------------------------------------------

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


            if (diasCumplidos <
                (diasPactados / 2.0))
            {
                multa =
                    costoTotalRestante *
                    0.50m;
            }
            else
            {
                multa =
                    costoTotalRestante *
                    0.25m;
            }



            return RedirectToAction(
                "PagarMulta",
                "Pago",
                new
                {
                    idReserva = idReserva,

                    fechaTerminacion =
                        fechaTerminacion.ToString(
                            "yyyy-MM-dd"
                        ),

                    multa = multa
                }
            );
        }


        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Renovar(int id)
        {
            var reservaOriginal =
                _repoReserva.ObtenerPorIdConDetalles(id);

            if (reservaOriginal == null)
            {
                return NotFound();
            }

            if (!reservaOriginal.Estado)
            {
                TempData["Error"] =
                    "No se puede renovar una reserva que se encuentra finalizada.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = id }
                );
            }

            var nuevaReserva =
                new Reserva
                {
                    IdInquilino =
                        reservaOriginal.IdInquilino,

                    IdInmueble =
                        reservaOriginal.IdInmueble,


                    FechaDesde =
                        reservaOriginal.FechaHasta,

                    FechaHasta =
                        reservaOriginal.FechaHasta.AddDays(1),

                    IdReservaOrigen =
                        reservaOriginal.IdReserva
                };


            ViewBag.ReservaOriginal =
                reservaOriginal;


            return View(nuevaReserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Renovar(Reserva reserva)
        {

            var reservaOriginal =
                reserva.IdReservaOrigen.HasValue
                    ? _repoReserva.ObtenerPorIdConDetalles(
                        reserva.IdReservaOrigen.Value
                    )
                    : null;


            if (reservaOriginal == null)
            {
                return NotFound();
            }


            if (!reservaOriginal.Estado)
            {
                TempData["Error"] =
                    "No se puede renovar una reserva que se encuentra finalizada.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = reservaOriginal.IdReserva
                    }
                );
            }



            reserva.IdInquilino =
                reservaOriginal.IdInquilino;

            reserva.IdInmueble =
                reservaOriginal.IdInmueble;

            if (reserva.FechaDesde <
                reservaOriginal.FechaHasta)
            {
                ModelState.AddModelError(
                    "FechaDesde",
                    "La renovación no puede comenzar antes de la finalización de la reserva original."
                );
            }


            if (reserva.FechaHasta <=
                reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha de finalización debe ser posterior a la fecha de inicio."
                );
            }

            if (reserva.FechaHasta >
                reserva.FechaDesde)
            {
                bool ocupado =
                    _repoReserva.InmuebleOcupado(
                        reserva.IdInmueble,
                        reserva.FechaDesde,
                        reserva.FechaHasta
                    );


                if (ocupado)
                {
                    ModelState.AddModelError(
                        "",
                        "El inmueble no se encuentra disponible para el período seleccionado."
                    );
                }
            }



            if (!ModelState.IsValid)
            {
                ViewBag.ReservaOriginal =
                    reservaOriginal;

                return View(reserva);
            }

            var inmueble =
                _repoInmueble.ObtenerPorId(
                    reserva.IdInmueble
                );


            if (inmueble == null)
            {
                return NotFound();
            }

            reserva.MontoDia =
                inmueble.PrecioDia;

            reserva.PorcentajeReserva =
                inmueble.PorcentajeReserva;


            var idUsuarioClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;


            if (int.TryParse(
                idUsuarioClaim,
                out int idUsuario))
            {
                reserva.IdUsuarioCreacion =
                    idUsuario;
            }

            reserva.IdReservaOrigen =
                reservaOriginal.IdReserva;

            reserva.Estado = true;

            int idNuevaReserva =
                _repoReserva.Guardar(
                    reserva
                );


            TempData["Mensaje"] =
                $"Reserva renovada correctamente. " +
                $"Se generó la Reserva N.º {idNuevaReserva}.";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = idNuevaReserva
                }
            );
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerInmueblesPorTipo(
            int idTipo)
        {
            var inmuebles =
                _repoInmueble
                    .ObtenerTodos()
                    .Where(
                        i => i.IdTipo == idTipo
                    )
                    .Select(
                        i => new
                        {
                            idInmueble =
                                i.IdInmueble,

                            direccion =
                                i.Direccion,

                            cupo =
                                i.Cupo,

                            precioDia =
                                i.PrecioDia,

                            porcentajeReserva =
                                i.PorcentajeReserva,

                            foto =
                                i.Foto
                        }
                    )
                    .ToList();


            return Json(inmuebles);
        }


        private void CargarSelects(
            int? idTipo = null,
            int? idInmueble = null)
        {

            var inquilinos =
                _repoInquilino
                    .ObtenerTodos()
                    .Select(
                        i => new
                        {
                            i.IdInquilino,

                            NombreCompleto =
                                $"{i.Nombre} {i.Apellido}"
                        }
                    )
                    .ToList();


            ViewBag.Inquilinos =
                new SelectList(
                    inquilinos,
                    "IdInquilino",
                    "NombreCompleto"
                );



            ViewBag.Tipos =
                new SelectList(
                    _repoTipoInmueble.ObtenerTodos(),
                    "IdTipo",
                    "Nombre",
                    idTipo
                );



            var inmuebles =
                new List<Inmueble>();


            if (idTipo.HasValue)
            {
                inmuebles =
                    _repoInmueble
                        .ObtenerTodos()
                        .Where(
                            i =>
                                i.IdTipo ==
                                idTipo.Value
                        )
                        .ToList();
            }


            ViewBag.Inmuebles =
                new SelectList(
                    inmuebles,
                    "IdInmueble",
                    "Direccion",
                    idInmueble
                );
        }
    }
}