using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Aplicacion
{
    public class FabricaCebon : IFabricaRes
    {
        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Cebon(nombre, peso, edad);
        }
    }
}
