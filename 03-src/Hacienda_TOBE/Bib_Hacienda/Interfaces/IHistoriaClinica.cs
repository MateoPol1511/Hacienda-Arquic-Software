using Bib_Hacienda.Dominio;
using System;
using System.Collections.Generic;

namespace Bib_Hacienda.Interfaces
{
    // SC-3: contrato de aplicación para registrar y consultar la historia
    // clínica de una Res. Mismo criterio que IVacunacion/ICreacionVacuna
    // (H-14): la operación de comando retorna ResultadoOperacion en vez de
    // string o excepciones como control de flujo.
    public interface IHistoriaClinica
    {
        // Registra un nuevo evento clínico para la res indicada.
        ResultadoOperacion RegistrarEvento(string potreroId, string nombreRes, DateTime fecha, string tipoEvento, string descripcion);

        // Consulta cronológica de la historia clínica de una res.
        List<EventoClinico> ConsultarHistoria(string potreroId, string nombreRes);
    }
}
