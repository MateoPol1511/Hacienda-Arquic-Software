using Bib_Hacienda.Dominio;
using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Aplicacion
{
    public class ServicioVentas : IVentaRes
    {
        private IRepositorioPotreros repositorioPotreros;
        private IRepositorioVentas repositorioVentas;

        public ServicioVentas(IRepositorioPotreros repositorioPotreros, IRepositorioVentas repositorioVentas)
        {
            this.repositorioPotreros = repositorioPotreros;
            this.repositorioVentas = repositorioVentas;
        }

        // Misma lógica que Hacienda.vender_res del AS-IS: crea la Venta,
        // la agrega al repositorio y remueve la res del potrero.
        public ResultadoOperacion vender_res(string id_potrero, string nombre, uint monto)
        {
            try
            {
                Potrero potrero = repositorioPotreros.ObtenerPorId(id_potrero);
                if (potrero == null)
                {
                    return ResultadoOperacion.Fallo($"No se encontró el potrero '{id_potrero}'");
                }

                Res res = potrero.BuscarRes(nombre);

                Venta venta = new Venta(potrero, DateTime.Now, res, monto);
                repositorioVentas.Agregar(venta);
                potrero.L_reses.Remove(res);

                return ResultadoOperacion.Ok($"Venta de la res {res.Nombre} realizada con exito");
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el metodo vender_res: " + er.Message);
            }
        }

        public List<Venta> ObtenerTodasLasVentas()
        {
            return repositorioVentas.ObtenerTodas().OrderByDescending(v => v.Fecha).ToList();
        }

        public List<Venta> ObtenerVentasPorPotrero(string potreroId)
        {
            return repositorioVentas.ObtenerTodas()
                .Where(v => v.Potrero.Identificacion == potreroId)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        public List<Venta> ObtenerVentasPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return repositorioVentas.ObtenerTodas()
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var ventas = repositorioVentas.ObtenerTodas();

            return new Dictionary<string, object>
            {
                { "TotalVentas", ventas.Count },
                { "MontoTotal", ventas.Sum(v => v.Monto) },
                { "PromedioVenta", ventas.Any() ? ventas.Average(v => v.Monto) : 0 },
                { "VentasEsteMes", ventas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year) },
                { "MontoEsteMes", ventas.Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).Sum(v => v.Monto) }
            };
        }
    }
}
