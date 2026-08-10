namespace Bib_Hacienda.Dominio
{
    // Nueva en el TO-BE. Punto de extensión para SC-1 (productos derivados).
    // Res la implementa hoy; un futuro ProductoDerivado podría implementarla
    // sin tocar Venta ni ServicioVentas. No se implementa comportamiento
    // adicional en este bloque (SC-1 queda fuera de esta etapa).
    public interface IActivoVendible
    {
        string Identificador { get; }
    }
}
