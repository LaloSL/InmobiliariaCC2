using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
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

        public ReservaController(
            RepositorioReserva repoReserva,
            RepositorioInmueble repoInmueble,
            RepositorioInquilino repoInquilino,
            RepositorioTipoInmueble repoTipoInmueble)
        {
            _repoReserva = repoReserva;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _repoTipoInmueble = repoTipoInmueble;
        }


        // =====================================================
        // LISTADO DE RESERVAS
        // Administrador y Empleado
        // =====================================================

        [AllowAnonymous]
        public IActionResult Index()
        {
            var lista = _repoReserva.ObtenerTodas();

            return View(lista);
        }


        // =====================================================
        // CREAR RESERVA - GET
        // ACCESO PÚBLICO
        // =====================================================

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Create()
        {
            CargarSelects();

            return View();
        }


        // =====================================================
        // CREAR RESERVA - POST
        // ACCESO PÚBLICO
        // =====================================================

        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Create(
            Reserva reserva,
            int? idTipo)
        {
            // =================================================
            // VALIDACIÓN DE FECHAS
            // =================================================

            if (reserva.FechaHasta <= reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    "FechaHasta",
                    "La fecha de finalización debe ser posterior a la fecha de inicio."
                );
            }


            // =================================================
            // VERIFICAR DISPONIBILIDAD DEL INMUEBLE
            // =================================================

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


            // =================================================
            // SI LOS DATOS SON VÁLIDOS
            // =================================================

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


                // =================================================
                // EL PRECIO SIEMPRE SE OBTIENE DESDE EL INMUEBLE
                // No confiamos en un monto enviado desde el navegador
                // =================================================

                reserva.MontoDia = inmueble.PrecioDia;


                // =================================================
                // GUARDAR RESERVA
                // =================================================

                _repoReserva.Guardar(reserva);


                TempData["Mensaje"] =
                    "Reserva creada exitosamente.";


                // =================================================
                // IMPORTANTE
                // No enviamos al visitante a Reserva/Index
                // porque Index está protegido para empleados/admin.
                // =================================================

                return RedirectToAction(
                    "Index",
                    "Inmueble"
                );
            }


            // =================================================
            // SI HUBO ERRORES, VOLVER A CARGAR LOS SELECT
            // =================================================

            CargarSelects(
                idTipo,
                reserva.IdInmueble
            );


            return View(reserva);
        }


        // =====================================================
        // DETALLE DE RESERVA
        // Administrador y Empleado
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


            return View(reserva);
        }


        // =====================================================
        // FINALIZAR ANTICIPADAMENTE - GET
        // Administrador y Empleado
        // =====================================================

        [Authorize(Roles = "Administrador,Empleado")]
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


        // =====================================================
        // FINALIZAR ANTICIPADAMENTE - POST
        // Administrador y Empleado
        // =====================================================

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


        // =====================================================
        // OBTENER INMUEBLES POR TIPO
        // ACCESO PÚBLICO
        //
        // Esta acción es utilizada por el formulario Create
        // para cargar los inmuebles según el tipo seleccionado.
        // =====================================================

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

                    foto =
                        i.Foto
                })
                .ToList();


            return Json(inmuebles);
        }


        // =====================================================
        // MÉTODO AUXILIAR PRIVADO
        // No necesita Authorize
        // =====================================================

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