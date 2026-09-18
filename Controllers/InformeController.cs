using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    [Authorize]
    public class InformeController : Controller
    {
        private readonly RepositorioInforme repositorio;

        public InformeController(RepositorioInforme repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MasReservados()
        {
            var lista = repositorio.ObtenerMasReservados(365);

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

            var lista = repositorio.ObtenerSinReservas(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }

        public IActionResult ReservasVigentes()
        {
            var lista = repositorio.ObtenerReservasVigentes();

            return View(lista);
        }

        [HttpGet]
        public IActionResult ProximosVencimientos(int dias = 7)
        {
            if (dias <= 0)
            {
                dias = 7;
            }

            var lista = repositorio.ObtenerReservasPorFinalizar(dias);

            ViewBag.Dias = dias;

            return View(lista);
        }
    }
}