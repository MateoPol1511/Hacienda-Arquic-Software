using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Eventos;
using System;

namespace Bib_Hacienda.Aplicacion
{
    public class ServicioAlimentacion
    {
        private IRepositorioPotreros repositorioPotreros;
        private PublisherPesoMin publisherPesoMin;
        private PublisherPesoVenta publisherPesoVenta;

        public ServicioAlimentacion(IRepositorioPotreros repositorioPotreros, PublisherPesoMin publisherPesoMin, PublisherPesoVenta publisherPesoVenta)
        {
            this.repositorioPotreros = repositorioPotreros;
            this.publisherPesoMin = publisherPesoMin;
            this.publisherPesoVenta = publisherPesoVenta;
        }

        // Misma lógica que Hacienda.alimentar_res(id_potrero, nombre) del AS-IS:
        // incrementa el peso en 1 e informa los eventos de peso.
        public ResultadoOperacion AlimentarRes(string id_potrero, string nombre)
        {
            return AlimentarInterno(id_potrero, nombre, 1);
        }

        // Misma lógica que la sobrecarga Hacienda.alimentar_res(id_potrero, nombre, cantidadAlimento).
        public ResultadoOperacion AlimentarRes(string id_potrero, string nombre, uint cantidadAlimento)
        {
            return AlimentarInterno(id_potrero, nombre, cantidadAlimento);
        }

        private ResultadoOperacion AlimentarInterno(string id_potrero, string nombre, uint cantidadAlimento)
        {
            try
            {
                Potrero potrero = repositorioPotreros.ObtenerPorId(id_potrero);
                if (potrero == null)
                {
                    return ResultadoOperacion.Fallo($"No se encontró el potrero '{id_potrero}'");
                }

                Res res = potrero.BuscarRes(nombre);

                res.Peso += cantidadAlimento;

                string mensaje_eventos = "";

                publisherPesoMin.evt_peso_min += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensaje_eventos += mensaje + "\n";
                };

                publisherPesoVenta.evt_peso_venta += (mensaje) =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensaje_eventos += mensaje + "\n";
                };

                publisherPesoMin.Informar_Peso_Min(res);
                publisherPesoVenta.Informar_Peso_Venta(res);

                string mensaje_final = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensaje_eventos))
                {
                    mensaje_final += "\n" + mensaje_eventos.TrimEnd();
                }

                return ResultadoOperacion.Ok(mensaje_final);
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el metodo AlimentarRes: " + er.Message);
            }
        }
    }
}
