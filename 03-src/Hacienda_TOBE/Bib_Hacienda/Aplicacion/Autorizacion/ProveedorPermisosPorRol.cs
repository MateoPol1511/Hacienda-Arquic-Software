namespace Bib_Hacienda.Aplicacion
{
    // OCP: aísla el if/else de admin/empleado/visitante que en el AS-IS vivía
    // dentro de Autenticacion.AutorizarOperacion, para que un rol nuevo no
    // obligue a modificar ServicioAutenticacion.
    //
    // INCONSISTENCIA REGISTRADA: el UML declara "rol : string" como parámetro,
    // pero Usuario (Bib_Hacienda.Dominio) NO tiene una propiedad Rol; solo
    // tiene Nombre/ContrasenaHash. En el AS-IS el "rol" nunca existió como tal:
    // el if/else comparaba directamente contra usuario.Nombre ("admin",
    // "empleado", "visitante" eran, a la vez, nombres de usuario Y roles).
    // Para no inventar un concepto de Rol que el UML no modela en Usuario,
    // ServicioAutenticacion invoca TienePermiso pasando usuario.Nombre como
    // "rol", preservando exactamente el comportamiento observable del AS-IS.
    public class ProveedorPermisosPorRol : IProveedorPermisos
    {
        public bool TienePermiso(string rol, string operacion)
        {
            if (rol == "admin")
            {
                //Admin tiene todos los permisos
                return true;
            }
            else if (rol == "empleado")
            {
                //Empleado puede hacer todo excepto eliminar usuarios
                return !operacion.Contains("Eliminar");
            }
            else if (rol == "visitante")
            {
                //Visitante solo puede consultar
                return operacion.Contains("Consultar") || operacion.Contains("Listar");
            }

            //Rol desconocido: sin permisos (comportamiento no cubierto por el
            //AS-IS, que solo contemplaba estos 3 roles; se deniega por defecto).
            return false;
        }
    }
}
