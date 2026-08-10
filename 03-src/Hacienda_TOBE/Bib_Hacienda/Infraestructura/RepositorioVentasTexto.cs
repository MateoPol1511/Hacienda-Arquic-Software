using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Validacion;

namespace Bib_Hacienda.Infraestructura
{
    // Misma lógica que PersistenciaService.GuardarVentas/CargarVentas del
    // AS-IS, con la validación explícita antes de persistir (H-11/H-12, ver
    // nota de RepositorioPotrerosTexto).
    public class RepositorioVentasTexto : IRepositorioVentas
    {
        private const string ArchivoVentas = "Ventas.txt";

        private readonly ISerializador<Venta> serializadorVenta;
        private readonly IValidadorVenta validadorVenta;

        public RepositorioVentasTexto(ISerializador<Venta> serializadorVenta, IValidadorVenta validadorVenta)
        {
            this.serializadorVenta = serializadorVenta;
            this.validadorVenta = validadorVenta;
        }

        public List<Venta> ObtenerTodas()
        {
            try
            {
                string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoVentas);
                var ventas = new List<Venta>();

                if (!File.Exists(ruta))
                {
                    return ventas;
                }

                foreach (var linea in File.ReadAllLines(ruta))
                {
                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue;
                    }

                    ventas.Add(serializadorVenta.Deserializar(linea));
                }

                return ventas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar ventas: {ex.Message}", ex);
            }
        }

        public void Agregar(Venta venta)
        {
            if (venta == null)
            {
                throw new ArgumentNullException(nameof(venta));
            }

            if (!validadorVenta.EsValido(venta))
            {
                throw new Exception("Error de validación en venta");
            }

            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoVentas);
            File.AppendAllLines(ruta, new[] { serializadorVenta.Serializar(venta) });
        }
    }
}
