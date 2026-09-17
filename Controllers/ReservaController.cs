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

        public ReservaController(
            RepositorioReserva repoReserva,
            RepositorioInmueble repoInmueble,
            RepositorioInquilino repoInquilino,
            RepositorioTipoInmueble repoTipoInmueble,
            RepositorioPago repoPago)
        {
            _repoReserva = repoReserva;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _repoTipoInmueble = repoTipoInmueble;
            _repoPago = repoPago;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            var lista = _repoReserva.ObtenerTodas();

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

        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
        public IActionResult Create()
        {
            CargarSelects();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Create(
            Reserva reserva,
            int? idTipo)
        {

            if (reserva.FechaHasta <= reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha de finalización debe ser posterior a la fecha de inicio."
                );
            }


            if (reserva.IdInmueble > 0 &&
                reserva.FechaHasta > reserva.FechaDesde)
            {
                bool ocupado = _repoReserva.InmuebleOcupado(
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


            if (ModelState.IsValid)
            {
                var inmueble = _repoInmueble.ObtenerPorId(
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


                reserva.MontoDia =
                        inmueble.PrecioDia;

                reserva.PorcentajeReserva =
                    inmueble.PorcentajeReserva;




                var idUsuarioClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    reserva.IdUsuarioCreacion = idUsuario;
                }


                _repoReserva.Guardar(reserva);


                TempData["Mensaje"] = "Reserva creada exitosamente.";


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



        [Authorize(Roles = "Administrador,Empleado")]
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

        [Authorize(Roles = "Administrador,Empleado")]
        [HttpGet]
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
        [Authorize(Roles = "Administrador,Empleado")]
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
                multa =
                    costoTotalRestante * 0.50m;
            }
            else
            {
                multa =
                    costoTotalRestante * 0.25m;
            }


            // ==========================================
            // AUDITORÍA - USUARIO QUE FINALIZA
            // ==========================================

            int? idUsuarioTerminacion = null;

            var idUsuarioClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                idUsuarioTerminacion = idUsuario;
            }


            _repoReserva.FinalizarAnticipadamente(
                idReserva,
                fechaTerminacion,
                multa,
                idUsuarioTerminacion
            );


            TempData["Mensaje"] =
                $"Reserva finalizada. Multa calculada: ${multa:N2}";


            return RedirectToAction(
                nameof(Details),
                new { id = idReserva }
            );
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerInmueblesPorTipo(int idTipo)
        {
            var inmuebles = _repoInmueble
                .ObtenerTodos()
                .Where(i => i.IdTipo == idTipo)
                .Select(i => new
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
                })
                .ToList();


            return Json(inmuebles);
        }


        private void CargarSelects(
            int? idTipo = null,
            int? idInmueble = null)
        {


            var inquilinos = _repoInquilino
                .ObtenerTodos()
                .Select(i => new
                {
                    i.IdInquilino,

                    NombreCompleto =
                        $"{i.Nombre} {i.Apellido}"
                })
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
                inmuebles = _repoInmueble
                    .ObtenerTodos()
                    .Where(
                        i => i.IdTipo == idTipo.Value
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