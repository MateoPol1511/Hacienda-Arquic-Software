using Bib_Hacienda.Dominio;
using System;

namespace Bib_Hacienda.Aplicacion
{
    public interface IFabricaVacunaBacteriana
    {
        Bacteriana Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion);
    }
}
