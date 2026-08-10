namespace Bib_Hacienda.Dominio.Validacion
{
    // Misma filosofía que ValidadorVacuna: evento no nulo y con los campos
    // mínimos que exige SC-3 (identificación de la res, tipo y descripción
    // del evento) presentes.
    public class ValidadorEventoClinico : IValidadorEventoClinico
    {
        public bool EsValido(EventoClinico evento)
        {
            if (evento == null
                || string.IsNullOrWhiteSpace(evento.PotreroId)
                || string.IsNullOrWhiteSpace(evento.NombreRes)
                || string.IsNullOrWhiteSpace(evento.TipoEvento)
                || string.IsNullOrWhiteSpace(evento.Descripcion))
            {
                return false;
            }
            return true;
        }
    }
}
