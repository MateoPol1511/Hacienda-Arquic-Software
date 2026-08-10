using Bib_Hacienda.Dominio;
using System.Collections.Generic;

namespace Bib_Hacienda.Aplicacion
{
    // DIP (H-02, H-03, H-04, H-11, H-13). Ver nota completa en IRepositorioPotreros.
    public interface IRepositorioUsuarios
    {
        List<Usuario> ObtenerTodos();
        Usuario BuscarPorNombre(string nombre);
        void Agregar(Usuario usuario);
    }
}
