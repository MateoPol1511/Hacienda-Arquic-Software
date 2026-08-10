using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    // H-14 (SRP): cambia su tipo de retorno de "string" a ResultadoOperacion.
    public interface IVentaRes
    {
        //Metodo para vender res
        ResultadoOperacion vender_res(string id_potrero, string nombre, uint monto);
    }
}
