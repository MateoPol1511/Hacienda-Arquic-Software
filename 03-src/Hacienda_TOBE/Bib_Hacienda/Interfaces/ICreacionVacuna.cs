using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    // H-14 (SRP): cambia su tipo de retorno de "string" a ResultadoOperacion.
    // El texto del mensaje no cambia; solo deja de mezclarse con el control
    // de flujo (ver ResultadoOperacion).
    public interface ICreacionVacuna
    {
        ResultadoOperacion crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion);
        ResultadoOperacion crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion);
        ResultadoOperacion crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad);
        ResultadoOperacion crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Viva.enum_l_atenuaciones grado_atenuacion, uint cantidad);
    }
}
