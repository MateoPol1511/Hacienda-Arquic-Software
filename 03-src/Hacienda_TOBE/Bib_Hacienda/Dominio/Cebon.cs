using Bib_Hacienda.Dominio.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio
{
    public class Cebon : Res //Hereda de Res
    {
        //Constructor
        public Cebon(string nombre, uint peso, ushort edad) : base(nombre, peso, edad)
        {
        }

        // OCP (H-10): reemplaza al antiguo override de Edad, que lanzaba
        // Exception si el valor no estaba entre edad_max_ternero (exclusivo)
        // y edad_max_cebon (inclusive). Misma regla de negocio, expuesta
        // ahora como consulta.
        public override bool EsEdadValida(ushort edad)
        {
            return edad > ReglaRes.edad_max_ternero && edad <= ReglaRes.edad_max_cebon;
        }

        public override ushort PesoMinimo => ReglaRes.peso_min_cebon;
        public override ushort PesoRecomendadoVenta => ReglaRes.peso_recom_venta_cebon;
        public override byte MaxVacunasBacterianas => ReglaVacuna.max_bac_cebon;
        public override byte MaxVacunasVivas => ReglaVacuna.max_viv_cebon;
    }
}
