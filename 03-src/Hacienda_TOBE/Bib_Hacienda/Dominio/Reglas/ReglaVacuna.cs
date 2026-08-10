using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio.Reglas
{
    // Conservada íntegramente del AS-IS (Bib_Hacienda.Reglas.ReglaVacuna),
    // solo cambia de namespace según el paquete "Bib_Hacienda.Dominio.Reglas" del UML.
    public abstract class ReglaVacuna
    {
        //Reglas de cantidad de vacunas para cada tipo de res
        public static readonly byte max_bac_ternero = 3;
        public static readonly byte max_bac_cebon = 1;
        public static readonly byte max_bac_novillo = 2;
        public static readonly byte max_viv_ternero = 1;
        public static readonly byte max_viv_cebon = 4;
        public static readonly byte max_viv_novillo = 2;

        //Reglas de periodos de aplicacion de vacunas
        public static readonly byte periodo_min_bac_aplic = 2;
        public static readonly byte periodo_max_bac_aplic = 4;
    }
}
