using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Eventos
{
    // OCP (H-10): ya no verifica "is Ternero/is Cebon/is Novillo"; consulta
    // el miembro polimórfico Res.PesoMinimo. Mismo mensaje/comportamiento
    // observable que en el AS-IS.
    //
    // Nota: se retira el operador de conversión implícita
    // "implicit operator PublisherPesoMin(PublisherPesoVenta v)" presente en
    // el AS-IS: no aparece en el UML, no tenía ninguna llamada en el resto
    // del proyecto y su cuerpo solo lanzaba NotImplementedException, por lo
    // que no había comportamiento observable que preservar. Ver inconsistencias.
    public class PublisherPesoMin
    {
        //delegado y evento
        public delegate void dele_peso_min(string peso_min);
        public event dele_peso_min evt_peso_min;

        //Metodo para informar si la res está por debajo del peso mínimo
        public void Informar_Peso_Min(Res res)
        {
            try
            {
                //Informar si la res está en desnutrición
                if (res.Peso < res.PesoMinimo)
                {
                    string mensaje = $"[Evento] La res '{res.Nombre}' tiene un peso {res.Peso}, está en desnutrición.";

                    if (evt_peso_min != null)
                    {
                        evt_peso_min(mensaje);
                    }
                    else
                    {
                        // Si no hay suscriptores, solo no hacer nada (el evento es opcional)
                    }
                }
            }
            catch (Exception er)
            {
                throw new Exception("[Evento] Error inesperado en el metodo Informar_Peso_Min: " + er.Message);
            }
        }
    }
}
