using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // ISP (H-12). Ver nota completa en IValidadorRes.cs.
    public interface IValidadorPotrero
    {
        bool EsValido(Potrero potrero);
    }
}
