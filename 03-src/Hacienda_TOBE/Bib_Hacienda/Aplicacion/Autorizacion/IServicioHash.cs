namespace Bib_Hacienda.Aplicacion
{
    // DIP (H-09): ServicioAutenticacion depende de esta abstracción para
    // calcular y verificar el hash de contraseñas. La implementación
    // concreta (ServicioHashSha256) vive en Bib_Hacienda.Infraestructura,
    // fuera del alcance de este bloque.
    public interface IServicioHash
    {
        string Hash(string textoPlano);
        bool Verificar(string textoPlano, string hash);
    }
}
