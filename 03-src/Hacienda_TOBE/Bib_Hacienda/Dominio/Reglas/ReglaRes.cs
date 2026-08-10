using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio.Reglas
{
    // Conservada íntegramente del AS-IS (Bib_Hacienda.Reglas.ReglaRes),
    // solo cambia de namespace según el paquete "Bib_Hacienda.Dominio.Reglas" del UML.
    public abstract class ReglaRes
    {
        //Reglas de peso para reses (En kg)
        public static readonly ushort peso_min_ternero = 150;
        public static readonly ushort peso_min_cebon = 290;
        public static readonly ushort peso_min_novillo = 400;
        public static readonly ushort peso_recom_venta_ternero = 250;
        public static readonly ushort peso_recom_venta_cebon = 420;
        public static readonly ushort peso_recom_venta_novillo = 550;

        //Reglas de edad para reses (en meses)
        public static readonly byte edad_max_ternero = 12;
        public static readonly byte edad_max_cebon = 48;
    }
}
