using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // ISP (H-12): reemplaza a IValidarInformacion, que obligaba a las 4
    // clases a implementar métodos ajenos con NotImplementedException.
    // Se elimina también la superclase abstracta Validacion del AS-IS:
    // ya no aporta valor (ver inconsistencias).
    //
    // Solo se declara el contrato en este bloque; las implementaciones
    // concretas (ValidadorRes, etc., con la lógica de negocio) quedan
    // fuera del alcance de "estructura base" y se abordarán en un
    // bloque posterior.
    public interface IValidadorRes
    {
        bool EsValido(Res res);
    }
}
