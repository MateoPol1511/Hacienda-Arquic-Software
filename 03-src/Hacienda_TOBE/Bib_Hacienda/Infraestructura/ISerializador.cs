namespace Bib_Hacienda.Infraestructura
{
    // SRP (H-11): aísla el formato posicional de texto (Reses.txt, Ventas.txt, ...)
    // del repositorio que lo usa. Una única interfaz genérica, reutilizada para
    // Potrero, Res, Venta, Vacuna y Usuario (ver UML, paquete
    // Bib_Hacienda.Infraestructura).
    public interface ISerializador<T>
    {
        // Convierte una entidad en su representación de una sola línea de texto.
        string Serializar(T entidad);

        // Reconstruye una entidad a partir de una línea de texto previamente
        // generada por Serializar.
        T Deserializar(string linea);
    }
}
