using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    [Authorize]
    public class InformeController : Controller
    {
        private readonly RepositorioInforme repositorio;
        private readonly RepositorioPropietario repositorioPropietario;

        public InformeController(
            RepositorioInforme repositorio,
            RepositorioPropietario repositorioPropietario)
        {
            this.repositorio = repositorio;
            this.repositorioPropietario = repositorioPropietario;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MasReservados()
        {
            var lista =
                repositorio.ObtenerMasReservados(365);

            ViewBag.Dias = 365;

            return View(lista);
        }

        [HttpGet]
        public IActionResult SinReservas(int dias = 30)
        {
            if (dias <= 0)
            {
                dias = 30;
            }

            var lista =
                repositorio.ObtenerSinReservas(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }

        public IActionResult ReservasVigentes()
        {
            var lista =
                repositorio.ObtenerReservasVigentes();

            return View(lista);
        }

        [HttpGet]
        public IActionResult ReservasPorFinalizar(int dias = 7)
        {
            if (dias <= 0)
            {
                dias = 7;
            }

            var lista =
                repositorio.ObtenerReservasPorFinalizar(dias);

            ViewBag.Dias = dias;

            return View("ProximosVencimientos", lista);
        }

        [HttpGet]
        public IActionResult Inmuebles(string estado = "todos")
        {
            bool? filtroEstado = null;

            if (estado == "disponibles")
            {
                filtroEstado = true;
            }
            else if (estado == "nodisponibles")
            {
                filtroEstado = false;
            }

            var lista =
                repositorio.ObtenerInmuebles(
                    filtroEstado
                );

            ViewBag.Estado = estado;

            return View(lista);
        }

        [HttpGet]
        public IActionResult PorPropietario(
            int? idPropietario)
        {
            var propietarios =
                repositorioPropietario.ObtenerTodos();

            ViewBag.Propietarios = propietarios;
            ViewBag.IdPropietario = idPropietario;

            if (!idPropietario.HasValue ||
                idPropietario.Value <= 0)
            {
                return View(
                    new List<
                        InmobiliariaCC2.Models.InformeInmueble
                    >()
                );
            }

            var lista =
                repositorio.ObtenerPorPropietario(
                    idPropietario.Value
                );

            return View(lista);
        }

        [HttpGet]
public IActionResult Disponibilidad(
    DateTime? fechaDesde,
    DateTime? fechaHasta)
{
    ViewBag.FechaDesde = fechaDesde;
    ViewBag.FechaHasta = fechaHasta;
    ViewBag.ConsultaRealizada = false;

    if (!fechaDesde.HasValue ||
        !fechaHasta.HasValue)
    {
        return View(
            new List<
                InmobiliariaCC2.Models.InformeInmueble
            >()
        );
    }

    if (fechaHasta.Value.Date <
        fechaDesde.Value.Date)
    {
        TempData["Error"] =
            "La fecha hasta no puede ser anterior a la fecha desde.";

        return View(
            new List<
                InmobiliariaCC2.Models.InformeInmueble
            >()
        );
    }

    var lista =
        repositorio.ObtenerDisponibles(
            fechaDesde.Value.Date,
            fechaHasta.Value.Date
        );

    ViewBag.ConsultaRealizada = true;

    return View(lista);
}
    }
}