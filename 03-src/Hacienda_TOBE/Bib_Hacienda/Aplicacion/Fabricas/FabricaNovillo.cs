using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Aplicacion
{
    public class FabricaNovillo : IFabricaRes
    {
        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Novillo(nombre, peso, edad);
        }
    }
}
