using Bib_Hacienda.Dominio;
using System;
using static Bib_Hacienda.Dominio.Viva;

namespace Bib_Hacienda.Aplicacion
{
    public interface IFabricaVacunaViva
    {
        Viva Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion);
    }
}
