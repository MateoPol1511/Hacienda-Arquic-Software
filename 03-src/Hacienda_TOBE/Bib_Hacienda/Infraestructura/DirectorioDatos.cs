using System;
using System.IO;

namespace Bib_Hacienda.Infraestructura
{
    // Utilidad interna compartida por los cuatro repositorios *Texto.
    // No es una dependencia inyectada (no aparece como campo en el UML de
    // ninguno de los RepositorioXTexto): es, igual que "Datos" lo era en
    // PersistenciaService del AS-IS, el nombre de una carpeta fija donde
    // viven los .txt. La diferencia frente al AS-IS es la forma de resolver
    // la ruta: PersistenciaService recibía IWebHostEnvironment por
    // constructor (disponible porque vivía en el proyecto ASP.NET Core
    // p_mvcHacienda); los RepositorioXTexto viven en Bib_Hacienda (biblioteca
    // de clases, sin referencia a ASP.NET Core) y el UML no les agrega un
    // parámetro de configuración/ruta al constructor. Por eso aquí se usa
    // AppDomain.CurrentDomain.BaseDirectory (la carpeta de salida del
    // ensamblado) en vez de ContentRootPath. Ver nota de inconsistencias.
    internal static class DirectorioDatos
    {
        private const string NombreCarpeta = "Datos";

        public static string ObtenerRuta()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NombreCarpeta);

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            return ruta;
        }
    }
}
