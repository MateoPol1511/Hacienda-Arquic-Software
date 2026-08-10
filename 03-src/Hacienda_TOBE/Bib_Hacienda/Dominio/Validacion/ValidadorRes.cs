using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // Misma regla de negocio que el AS-IS (Bib_Hacienda.Clases.Validaciones.ValidadorRes.ValidarRes):
    // res no nula, nombre no vacío, peso y edad mayores a 0. ISP (H-12): ya no hereda de
    // una superclase con métodos ajenos que lanzaban NotImplementedException.
    public class ValidadorRes : IValidadorRes
    {
        public bool EsValido(Res res)
        {
            if (res == null || string.IsNullOrWhiteSpace(res.Nombre) || res.Peso <= 0 || res.Edad <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
