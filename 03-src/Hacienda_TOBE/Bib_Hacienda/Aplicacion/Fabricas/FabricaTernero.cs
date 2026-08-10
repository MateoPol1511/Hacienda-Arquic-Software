using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Aplicacion
{
    public class FabricaTernero : IFabricaRes
    {
        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Ternero(nombre, peso, edad);
        }
    }
}
