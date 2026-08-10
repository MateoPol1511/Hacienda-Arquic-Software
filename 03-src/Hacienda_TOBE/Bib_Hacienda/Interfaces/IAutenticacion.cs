using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    // LSP (ADR-5): antes devolvía void y comunicaba éxito/rechazo lanzando
    // excepción en ambos casos. Ahora retorna ResultadoOperacion de forma
    // uniforme (mismo mensaje descriptivo de antes), haciendo el contrato
    // explícito y sustituible.
    public interface IAutenticacion
    {
        //Autoriza la ejecución de una operación para un usuario
        ResultadoOperacion AutorizarOperacion(Usuario usuario, string operacion);
    }
}
