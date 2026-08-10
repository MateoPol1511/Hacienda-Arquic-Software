using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    // H-14 (SRP): cambia su tipo de retorno de "string" a ResultadoOperacion.
    public interface IVacunacion
    {
        //Metodo para aplicar vacuna
        ResultadoOperacion aplicar_vacuna(Vacuna vacuna, string nombre, string id_potrero);
    }
}
