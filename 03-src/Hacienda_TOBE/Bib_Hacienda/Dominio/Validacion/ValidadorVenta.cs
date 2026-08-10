using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Dominio.Validacion
{
    // Misma regla de negocio que el AS-IS (ValidadorVenta.ValidarVenta): venta no nula,
    // potrero y activo vendido no nulos, monto mayor a 0. El AS-IS validaba "venta.Res"；
    // en el Bloque 1 ese campo pasó a llamarse "venta.Activo" (IActivoVendible, OCP/SC-1),
    // así que la misma comprobación de nulidad se aplica sobre Activo.
    public class ValidadorVenta : IValidadorVenta
    {
        public bool EsValido(Venta venta)
        {
            if (venta == null || venta.Potrero == null || venta.Activo == null || venta.Monto <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
