using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Aplicacion;
using System.Globalization;

namespace p_mvcHacienda.Controllers
{
    public class ResController : Controller
    {
        // Atributos
        private readonly ServicioPotreros _servicioPotreros;
        private readonly ServicioAlimentacion _servicioAlimentacion;
        private readonly ServicioVentas _servicioVentas;
        // SC-3: servicio de aplicación para historia clínica.
        private readonly ServicioHistoriaClinica _servicioHistoriaClinica;

        //Constructor con inyección de dependencias
        public ResController(ServicioPotreros servicioPotreros, ServicioAlimentacion servicioAlimentacion, ServicioVentas servicioVentas, ServicioHistoriaClinica servicioHistoriaClinica)
        {
            _servicioPotreros = servicioPotreros;
            _servicioAlimentacion = servicioAlimentacion;
            _servicioVentas = servicioVentas;
            _servicioHistoriaClinica = servicioHistoriaClinica;
        }

        // GET: Res/Index - Listar todas las reses
        [HttpGet]
        public ActionResult Index()
        {
            var resesConPotrero = _servicioPotreros.ObtenerTodasLasReses();
            var estadisticas = _servicioPotreros.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(resesConPotrero);
        }

        // Ver vacunas aplicadas por res
        [HttpGet]
        public ActionResult DetalleVacunas(string potreroId, string nombreRes)
        {
            try
            {
                var res = _servicioPotreros.BuscarRes(potreroId, nombreRes);
                if (res == null)
                {
                    TempData["Mensaje"] = "Res no encontrada";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.PotreroId = potreroId;
                ViewBag.NombreRes = nombreRes;
                return View(res.L_vacunas_aplicadas);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        // SC-3: Ver / registrar historia clínica de una res
        [HttpGet]
        public ActionResult HistoriaClinica(string potreroId, string nombreRes)
        {
            try
            {
                var res = _servicioPotreros.BuscarRes(potreroId, nombreRes);
                if (res == null)
                {
                    TempData["Mensaje"] = "Res no encontrada";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.PotreroId = potreroId;
                ViewBag.NombreRes = nombreRes;

                var historia = _servicioHistoriaClinica.ConsultarHistoria(potreroId, nombreRes);
                return View(historia);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        // SC-3: POST Res/RegistrarEventoClinico - Registrar un nuevo evento clínico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEventoClinico(string potreroId, string nombreRes, string fecha, string tipoEvento, string descripcion)
        {
            try
            {
                if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaEvento))
                {
                    TempData["Mensaje"] = "Fecha del evento clínico inválida";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(HistoriaClinica), new { potreroId, nombreRes });
                }

                var resultado = _servicioHistoriaClinica.RegistrarEvento(potreroId, nombreRes, fechaEvento, tipoEvento, descripcion);

                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = resultado.Exito ? "success" : "danger";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(HistoriaClinica), new { potreroId, nombreRes });
        }

        // GET: Res/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
            return View();
        }

        // POST: Res/Create - Procesar creación de res
        [HttpPost]
        public ActionResult Create(string potreroId, string nombre, ushort edad, uint peso)
        {
            try
            {
                // Validar entrada
                if (string.IsNullOrWhiteSpace(potreroId) || string.IsNullOrWhiteSpace(nombre))
                {
                    ViewBag.Mensaje = "Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
                    return View();
                }

                // Usar ServicioPotreros para agregar la res (persiste internamente vía repositorio)
                var resultado = _servicioPotreros.AnadirResAPotrero(potreroId, nombre, edad, peso);

                if (!resultado.Exito)
                {
                    ViewBag.Mensaje = resultado.Mensaje;
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
                    return View();
                }

                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }

            ViewBag.Potreros = _servicioPotreros.ObtenerTodosLosPotreros();
            return View();
        }

        // POST: Res/Alimentar - Alimentar una res
        public ActionResult Alimentar(string potreroId, string nombreRes, uint cantidadAlimento)
        {
            try
            {
                var resultado = _servicioAlimentacion.AlimentarRes(potreroId, nombreRes, cantidadAlimento);

                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = resultado.Exito ? "success" : "danger";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Res/Vender - Vender una res (validando overflow de monto)
        public ActionResult Vender(string potreroId, string nombreRes, string monto)
        {
            try
            {
                // Validar y convertir monto de forma segura
                if (string.IsNullOrWhiteSpace(monto))
                {
                    TempData["Mensaje"] = "El monto es requerido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Intentar convertir a decimal primero
                if (!decimal.TryParse(monto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var montoDec))
                {
                    TempData["Mensaje"] = "Monto inválido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Validar límites de uint
                if (montoDec < 0 || montoDec > uint.MaxValue)
                {
                    TempData["Mensaje"] = $"El monto excede el máximo permitido ({uint.MaxValue})";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                var montoUint = (uint)montoDec;

                // Vende la res y envía el mensaje (persiste internamente vía repositorio)
                var resultado = _servicioVentas.vender_res(potreroId, nombreRes, montoUint);

                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = resultado.Exito ? "success" : "danger";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
