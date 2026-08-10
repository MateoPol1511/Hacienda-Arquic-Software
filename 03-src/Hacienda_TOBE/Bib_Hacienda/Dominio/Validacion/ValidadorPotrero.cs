using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // Misma regla de negocio que el AS-IS (ValidadorPotrero.ValidarPotrero):
    // potrero no nulo, identificación no vacía.
    public class ValidadorPotrero : IValidadorPotrero
    {
        public bool EsValido(Potrero potrero)
        {
            if (potrero == null || string.IsNullOrWhiteSpace(potrero.Identificacion))
            {
                return false;
            }
            return true;
        }
    }
}
