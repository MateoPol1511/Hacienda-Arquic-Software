using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Aplicacion;

namespace p_mvcHacienda.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ServicioAutenticacion _servicioAutenticacion;

        public UsuarioController(ServicioAutenticacion servicioAutenticacion)
        {
            _servicioAutenticacion = servicioAutenticacion;
        }

        // GET: Usuario/Index - Listar todos los usuarios
        [HttpGet]
        public ActionResult Index()
        {
            var usuarios = _servicioAutenticacion.listar_usuarios();
            var estadisticas = _servicioAutenticacion.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(usuarios);
        }

        // GET: Usuario/Create - Mostrar formulario de creación
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create - Procesar creación de usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string nombre, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contrasena))
                {
                    ViewBag.Mensaje = "❌ Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                var resultado = _servicioAutenticacion.crear_usuario(nombre, contrasena);

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
                ViewBag.Mensaje = $"❌ Error: {ex.Message}";
                ViewBag.TipoMensaje = "danger";
                return View();
            }
        }
    }
}
