using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Aplicacion
{
    // OCP (H-07): una fábrica por tipo de Res. Ver nota en RegistroFabricasRes.
    public interface IFabricaRes
    {
        Res Crear(string nombre, uint peso, ushort edad);
    }
}
