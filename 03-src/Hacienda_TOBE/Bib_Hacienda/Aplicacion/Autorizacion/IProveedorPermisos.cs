namespace Bib_Hacienda.Aplicacion
{
    public interface IProveedorPermisos
    {
        bool TienePermiso(string rol, string operacion);
    }
}
