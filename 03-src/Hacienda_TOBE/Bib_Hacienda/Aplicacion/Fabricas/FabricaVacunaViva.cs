using Bib_Hacienda.Dominio;
using System;
using static Bib_Hacienda.Dominio.Viva;

namespace Bib_Hacienda.Aplicacion
{
    // H-05 (OCP): reemplaza la instanciación condicional por parámetros nulos
    // en crear_vacuna. Un 3er tipo de vacuna se incorpora agregando una
    // fábrica nueva.
    public class FabricaVacunaViva : IFabricaVacunaViva
    {
        public Viva Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion)
        {
            return new Viva(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
        }
    }
}
