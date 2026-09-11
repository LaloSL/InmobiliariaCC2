using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaCC2.Models;
using InmobiliariaCC2.Repositories;

namespace InmobiliariaCC2.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioPropietario _repoPropietario;
        private readonly RepositorioTipoInmueble _repoTipoInmueble;
        private readonly IWebHostEnvironment _environment;

        public InmuebleController(
            RepositorioInmueble repoInmueble,
            RepositorioPropietario repoPropietario,
            RepositorioTipoInmueble repoTipoInmueble,
            IWebHostEnvironment environment)
        {
            _repoInmueble = repoInmueble;
            _repoPropietario = repoPropietario;
            _repoTipoInmueble = repoTipoInmueble;
            _environment = environment;
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            var lista = _repoInmueble.ObtenerTodos();
            return View(lista);
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            CargarDesplegables();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                int idCreado = _repoInmueble.Guardar(inmueble);

                if (inmueble.FotoFile != null && inmueble.FotoFile.Length > 0)
                {
                    string wwwPath = _environment.WebRootPath;
                    string pathUploads = Path.Combine(wwwPath, "uploads", "inmuebles");

                    if (!Directory.Exists(pathUploads))
                    {
                        Directory.CreateDirectory(pathUploads);
                    }

                    string extension = Path.GetExtension(inmueble.FotoFile.FileName);
                    string nombreArchivo = $"inmueble_{idCreado}_{Guid.NewGuid()}{extension}";
                    string rutaCompleta = Path.Combine(pathUploads, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        inmueble.FotoFile.CopyTo(stream);
                    }

                    inmueble.Foto = $"/uploads/inmuebles/{nombreArchivo}";

                    _repoInmueble.Actualizar(inmueble);
                }

                TempData["Mensaje"] = "Inmueble registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            CargarDesplegables();
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
                return NotFound();

            CargarDesplegables();
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.IdInmueble)
                return NotFound();

            if (ModelState.IsValid)
            {
                var inmuebleExistente = _repoInmueble.ObtenerPorId(id);
                if (inmuebleExistente == null)
                    return NotFound();

                if (inmueble.FotoFile != null && inmueble.FotoFile.Length > 0)
                {
                    string wwwPath = _environment.WebRootPath;
                    string pathUploads = Path.Combine(wwwPath, "uploads", "inmuebles");

                    if (!Directory.Exists(pathUploads))
                    {
                        Directory.CreateDirectory(pathUploads);
                    }

                    if (!string.IsNullOrEmpty(inmuebleExistente.Foto))
                    {
                        string fotoAntiguaRuta = Path.Combine(wwwPath, inmuebleExistente.Foto.TrimStart('/'));
                        if (System.IO.File.Exists(fotoAntiguaRuta))
                        {
                            System.IO.File.Delete(fotoAntiguaRuta);
                        }
                    }

                    string extension = Path.GetExtension(inmueble.FotoFile.FileName);
                    string nombreArchivo = $"inmueble_{id}_{Guid.NewGuid()}{extension}";
                    string rutaCompleta = Path.Combine(pathUploads, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        inmueble.FotoFile.CopyTo(stream);
                    }

                    inmueble.Foto = $"/uploads/inmuebles/{nombreArchivo}";
                }
                else
                {
                    inmueble.Foto = inmuebleExistente.Foto;
                }

                _repoInmueble.Actualizar(inmueble);
                TempData["Mensaje"] = "Inmueble actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            CargarDesplegables();
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble == null)
                return NotFound();

            return View(inmueble);
        }

        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var inmueble = _repoInmueble.ObtenerPorId(id);
            if (inmueble != null)
            {
                if (!string.IsNullOrEmpty(inmueble.Foto))
                {
                    string wwwPath = _environment.WebRootPath;
                    string fotoRuta = Path.Combine(wwwPath, inmueble.Foto.TrimStart('/'));

                    if (System.IO.File.Exists(fotoRuta))
                    {
                        System.IO.File.Delete(fotoRuta);
                    }
                }

                _repoInmueble.Baja(id);
                TempData["Mensaje"] = "Inmueble eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult BuscarDisponibles(DateTime? desde, DateTime? hasta)
        {
            var lista = new List<Inmueble>();

            if (desde.HasValue && hasta.HasValue)
            {
                if (hasta <= desde)
                {
                    ViewBag.Error = "La fecha 'Hasta' debe ser posterior a la fecha 'Desde'.";
                    return View(lista);
                }

                ViewBag.Desde = desde.Value.ToString("yyyy-MM-dd");
                ViewBag.Hasta = hasta.Value.ToString("yyyy-MM-dd");

                lista = _repoInmueble.BuscarDisponibles(desde.Value, hasta.Value);
            }

            return View(lista);
        }

        private void CargarDesplegables()
        {
            var propietarios = _repoPropietario
                .ObtenerTodos()
                .Select(p => new
                {
                    p.IdPropietario,
                    NombreCompleto = $"{p.Nombre} {p.Apellido}"
                })
                .ToList();

            ViewBag.Propietarios = new SelectList(
                propietarios,
                "IdPropietario",
                "NombreCompleto"
            );

            ViewBag.Tipos = new SelectList(
                _repoTipoInmueble.ObtenerTodos(),
                "IdTipo",
                "Nombre"
            );
        }
    }
}