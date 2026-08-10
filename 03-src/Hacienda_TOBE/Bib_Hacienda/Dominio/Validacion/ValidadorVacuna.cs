using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // Misma regla de negocio que el AS-IS (ValidadorVacuna.ValidarVacuna):
    // vacuna no nula, nombre y lote no vacíos.
    public class ValidadorVacuna : IValidadorVacuna
    {
        public bool EsValido(Vacuna vacuna)
        {
            if (vacuna == null || string.IsNullOrWhiteSpace(vacuna.Nombre) || string.IsNullOrWhiteSpace(vacuna.Lote))
            {
                return false;
            }
            return true;
        }
    }
}
