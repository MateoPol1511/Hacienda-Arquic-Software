using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio.Eventos
{
    // OCP (H-10): ya no verifica "is Ternero/is Cebon/is Novillo"; consulta
    // el miembro polimórfico Res.PesoRecomendadoVenta. Mismo mensaje/
    // comportamiento observable que en el AS-IS.
    public class PublisherPesoVenta
    {
        //Definicion del delegado y el evento
        public delegate void dele_peso_venta(string peso_venta);
        public event dele_peso_venta evt_peso_venta;

        //Metodo para informar si la res está apta para la venta
        public void Informar_Peso_Venta(Res res)
        {
            try
            {
                //Informar si la res está apta para la venta
                if (res.Peso >= res.PesoRecomendadoVenta)
                {
                    string mensaje = $"[Evento] La res '{res.Nombre}' tiene un peso {res.Peso}, apta para venta.";

                    if (evt_peso_venta != null)
                    {
                        evt_peso_venta(mensaje);
                    }
                    else
                    {
                        // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo Informar_Peso_Venta: " + er.Message);
            }
        }
    }
}
