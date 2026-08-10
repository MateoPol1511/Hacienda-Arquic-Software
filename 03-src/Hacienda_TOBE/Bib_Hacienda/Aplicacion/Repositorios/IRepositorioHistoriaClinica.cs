using Bib_Hacienda.Dominio;
using System.Collections.Generic;

namespace Bib_Hacienda.Aplicacion
{
    // DIP (H-02, H-03, H-04, H-11, H-13). Ver nota completa en
    // IRepositorioPotreros. SC-3: repositorio dedicado a los eventos de
    // historia clínica, uno por res (identificada por PotreroId +
    // NombreRes, ver EventoClinico).
    public interface IRepositorioHistoriaClinica
    {
        // Eventos clínicos de una res concreta, en orden cronológico.
        List<EventoClinico> ObtenerPorRes(string potreroId, string nombreRes);

        // Agrega un nuevo evento clínico.
        void Agregar(EventoClinico evento);
    }
}
