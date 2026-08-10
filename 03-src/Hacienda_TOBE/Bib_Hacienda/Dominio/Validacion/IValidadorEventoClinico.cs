namespace Bib_Hacienda.Dominio.Validacion
{
    // ISP (H-12): un validador por entidad, igual que IValidadorVacuna,
    // IValidadorRes, etc. SC-3: valida EventoClinico antes de persistirlo.
    public interface IValidadorEventoClinico
    {
        bool EsValido(EventoClinico evento);
    }
}
