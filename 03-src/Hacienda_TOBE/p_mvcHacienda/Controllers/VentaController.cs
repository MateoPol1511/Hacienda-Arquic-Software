using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bib_Hacienda.Aplicacion;

namespace p_mvcHacienda.Controllers
{
    public class VentaController : Controller
    {
        private readonly ServicioVentas _servicioVentas;

        public VentaController(ServicioVentas servicioVentas)
        {
            _servicioVentas = servicioVentas;
        }

        // GET: VentaController
        public ActionResult Index()
        {
            var ventas = _servicioVentas.ObtenerTodasLasVentas();
            var estadisticas = _servicioVentas.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(ventas);
        }

        // GET: VentaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VentaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: VentaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // GET: VentaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VentaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: VentaController/Edit/5
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: VentaController/Delete/5
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
