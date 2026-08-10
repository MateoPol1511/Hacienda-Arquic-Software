using Bib_Hacienda.Dominio;
using System;

namespace Bib_Hacienda.Aplicacion
{
    public class FabricaVacunaBacteriana : IFabricaVacunaBacteriana
    {
        public Bacteriana Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            return new Bacteriana(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
        }
    }
}
