using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio.Reglas
{
    // Conservada íntegramente del AS-IS (Bib_Hacienda.Reglas.ReglaPotrero),
    // solo cambia de namespace según el paquete "Bib_Hacienda.Dominio.Reglas" del UML.
    public abstract class ReglaPotrero
    {
        //Maximo de reses por potrero
        public static readonly ushort max_reses_potrero = 150;
    }
}
