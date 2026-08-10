using Bib_Hacienda.Dominio;
using System.Collections.Generic;

namespace Bib_Hacienda.Aplicacion
{
    // DIP (H-02, H-03, H-04, H-11, H-13): un repositorio por entidad. Los
    // servicios de aplicación dependen de esta abstracción; la implementación
    // concreta vive en Bib_Hacienda.Infraestructura (fuera de este bloque).
    public interface IRepositorioPotreros
    {
        List<Potrero> ObtenerTodos();
        Potrero ObtenerPorId(string identificacion);
        void Agregar(Potrero potrero);
        bool Existe(string identificacion);
    }
}
