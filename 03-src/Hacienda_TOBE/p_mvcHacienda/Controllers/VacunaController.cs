using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Aplicacion;
using static Bib_Hacienda.Dominio.Viva;
using System.Globalization;

namespace p_mvcHacienda.Controllers
{
    public class VacunaController : Controller
    {
        // Atributos
        private readonly ServicioInventarioVacunas _servicioInventarioVacunas;
        private readonly ServicioVacunacion _servicioVacunacion;
        private readonly ServicioPotreros _servicioPotreros;

        //Constructor con inyección de dependencias
        public VacunaController(ServicioInventarioVacunas servicioInventarioVacunas, ServicioVacunacion servicioVacunacion, ServicioPotreros servicioPotreros)
        {
            _servicioInventarioVacunas = servicioInventarioVacunas;
            _servicioVacunacion = servicioVacunacion;
            _servicioPotreros = servicioPotreros;
        }

        // GET: Vacuna/Index - Listar todas las vacunas
        [HttpGet]
        public ActionResult Index()
        {
            var vacunas = _servicioInventarioVacunas.ObtenerVacunasDisponibles();
            var estadisticas = _servicioInventarioVacunas.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(vacunas);
        }

        // GET: Vacuna/Create - Mostrar formulario de creación
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Vacuna/Aplicar - Mostrar formulario de aplicación
        [HttpGet]
        public ActionResult Aplicar()
        {
            ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
            ViewBag.Reses = _servicioPotreros.ObtenerTodasLasReses();
            ViewBag.Vacunas = _servicioInventarioVacunas.ObtenerVacunasDisponibles();
            return View();
        }

        // POST: Vacuna/Create - Procesar creación de vacuna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string tipoVacuna, string nombre, string lote,
            string fechaVencimiento, string fechaAplicacion,    
            uint? periodoAplicacion, enum_l_atenuaciones? atenuacion)
        {
            try
            {
                // Validar campos requeridos básicos
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(lote))
                {
                    ViewBag.Mensaje = "El nombre y lote son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Parsear fechas desde inputs HTML date (yyyy-MM-dd)
                if (!DateTime.TryParseExact(fechaVencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaVenc))
                {
                    ViewBag.Mensaje = "Fecha de vencimiento inválida";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }
                if (!DateTime.TryParseExact(fechaAplicacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaAplic))
                {
                    ViewBag.Mensaje = "Fecha de aplicación inválida";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Reglas simples de fecha
                if (fechaAplic > fechaVenc)
                {
                    ViewBag.Mensaje = "La fecha de aplicación no puede ser posterior a la fecha de vencimiento";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                if (tipoVacuna == "Bacteriana" && !periodoAplicacion.HasValue)
                {
                    //HasValue para validar que no sea nulo la entrada del formulario en la vista
                    ViewBag.Mensaje = "El período de aplicación es requerido para vacunas bacterianas";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                if (tipoVacuna != "Bacteriana" && !atenuacion.HasValue)
                {
                    ViewBag.Mensaje = "La atenuación es requerida para vacunas vivas";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // ServicioInventarioVacunas.CrearVacuna decide bacteriana-vs-viva según
                // qué parámetro opcional llega con valor (mismo criterio que el if/else
                // tipoVacuna == "Bacteriana" del AS-IS, ahora centralizado en el servicio).
                var resultado = _servicioInventarioVacunas.CrearVacuna(nombre, lote, fechaVenc, fechaAplic, periodoAplicacion, atenuacion);

                if (resultado.Exito)
                {
                    TempData["Mensaje"] = resultado.Mensaje;
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Mensaje = resultado.Mensaje;
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $" Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                return View();
            }
        }

        // POST: Vacuna/Aplicar - Procesar aplicación de vacuna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aplicar(string potreroId, string nombreRes, string loteVacuna)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(potreroId) || string.IsNullOrWhiteSpace(nombreRes) || string.IsNullOrWhiteSpace(loteVacuna))
                {
                    ViewBag.Mensaje = " Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
                    ViewBag.Reses = _servicioPotreros.ObtenerTodasLasReses();
                    ViewBag.Vacunas = _servicioInventarioVacunas.ObtenerVacunasDisponibles();
                    return View();
                }

                // Buscar la vacuna disponible por su lote (ServicioInventarioVacunas
                // no expone una búsqueda directa por lote en el UML; se filtra sobre
                // el catálogo de disponibles, igual que hacía VacunaService.AplicarVacuna).
                var vacuna = _servicioInventarioVacunas.ObtenerVacunasDisponibles()
                    .FirstOrDefault(v => v.Lote == loteVacuna);

                if (vacuna == null)
                {
                    TempData["Mensaje"] = $"No se encontró una vacuna con el lote '{loteVacuna}'";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                var resultado = _servicioVacunacion.aplicar_vacuna(vacuna, nombreRes, potreroId);

                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = resultado.Exito ? "success" : "danger";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $" Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
                ViewBag.Reses = _servicioPotreros.ObtenerTodasLasReses();
                ViewBag.Vacunas = _servicioInventarioVacunas.ObtenerVacunasDisponibles();
                return View();
            }
        }
    }
}
