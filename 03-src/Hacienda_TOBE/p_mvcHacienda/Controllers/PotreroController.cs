using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Aplicacion;

namespace p_mvcHacienda.Controllers
{
    public class PotreroController : Controller
    {
        //Atributos
        private readonly ServicioPotreros _servicioPotreros;

        //Inyección de dependencias del servicio
        public PotreroController(ServicioPotreros servicioPotreros)
        {
            _servicioPotreros = servicioPotreros;
        }

        // GET
        [HttpGet]

        //Mostrar la lista de potreros y estadisticas
        public ActionResult Index()
        {
            var potreros = _servicioPotreros.ObtenerTodosLosPotreros();
            var estadisticas = _servicioPotreros.ObtenerEstadisticas();
      
            ViewBag.Estadisticas = estadisticas;

            return View(potreros);
        }

        
        // GET: Potrero/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            return View();
        }

        //Detalles de un potrero
        public ActionResult Details(string id)
        {
            var potrero = _servicioPotreros.ObtenerPotreroPorIdentificacion(id);

            if (potrero == null)
            {
                TempData["Mensaje"] = "Potrero no encontrado";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            return View(potrero);
        }

        // POST:
        [HttpPost]

        // Procesar creación de potrero
        // NOTA: Tipo_potrero pasa de "enum l_tipos_potreros" (AS-IS) a "string" en el
        // TO-BE (clave que indexa RegistroFabricasRes, ver Bib_Hacienda.Dominio.Potrero).
        // El formulario (Views/Potrero/Create.cshtml) sigue posteando el mismo valor de
        // texto que antes, por lo que el binding a string es compatible sin tocar la vista.
        public ActionResult Create(string identificacion, string tipo)
        {
            try
            { 
                // Validar entrada
                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    ViewBag.Mensaje = "La identificación no puede estar vacía";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Llamar al servicio para crear potrero (persiste internamente vía repositorio)
                var resultado = _servicioPotreros.CrearPotrero(identificacion, tipo);

                if (!resultado.Exito)
                {
                    ViewBag.Mensaje = resultado.Mensaje;
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                // Si es exitoso, redirigir con mensaje de éxito
                TempData["Mensaje"] = resultado.Mensaje;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }
  
            return View();
        }
    }
}
