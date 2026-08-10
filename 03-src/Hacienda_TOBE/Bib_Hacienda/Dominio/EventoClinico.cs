using System;

namespace Bib_Hacienda.Dominio
{
    // SC-3 ("Además de las vacunas, se va a requerir tener la historia
    // clínica de cada res en un futuro"): clase de datos simple, en el
    // mismo estilo que Vacuna (atributos privados + accesores get/set),
    // que representa un evento/registro individual de la historia clínica
    // de una Res.
    //
    // Identificación de la Res: Res no tiene un identificador propio
    // distinto del nombre (ver Res.Identificador) y ese nombre solo es
    // único DENTRO de un Potrero (Potrero.BuscarRes hace coincidencia por
    // nombre en su propia L_reses). Por eso, igual que ya documentaba
    // SerializadorVacuna para "vacunas aplicadas" (formato
    // "PotreroId|NombreRes|..."), un EventoClinico se identifica con la
    // pareja PotreroId + NombreRes, sin inventar un identificador global
    // que el UML/dominio actual no tiene.
    public class EventoClinico
    {
        //Atributos
        private string potreroId;
        private string nombreRes;
        private DateTime fecha;
        private string tipoEvento;
        private string descripcion;

        //Constructor
        public EventoClinico(string potreroId, string nombreRes, DateTime fecha, string tipoEvento, string descripcion)
        {
            this.PotreroId = potreroId;
            this.NombreRes = nombreRes;
            this.Fecha = fecha;
            this.TipoEvento = tipoEvento;
            this.Descripcion = descripcion;
        }

        //Accesores
        public string PotreroId { get => potreroId; set => potreroId = value; }
        public string NombreRes { get => nombreRes; set => nombreRes = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }
        public string TipoEvento { get => tipoEvento; set => tipoEvento = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
    }
}
