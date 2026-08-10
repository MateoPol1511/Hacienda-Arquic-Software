using Bib_Hacienda.Dominio;
using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;

namespace Bib_Hacienda.Aplicacion
{
    // SC-3 ("Además de las vacunas, se va a requerir tener la historia
    // clínica de cada res en un futuro"): agrega la posibilidad de manejar
    // la historia clínica de cada Res, siguiendo el mismo estilo que
    // ServicioVacunacion: valida que la res exista dentro del potrero
    // indicado (IRepositorioPotreros + Potrero.BuscarRes) antes de
    // registrar el evento, y delega la persistencia en
    // IRepositorioHistoriaClinica (DIP).
    public class ServicioHistoriaClinica : IHistoriaClinica
    {
        private IRepositorioPotreros repositorioPotreros;
        private IRepositorioHistoriaClinica repositorioHistoriaClinica;

        public ServicioHistoriaClinica(IRepositorioPotreros repositorioPotreros, IRepositorioHistoriaClinica repositorioHistoriaClinica)
        {
            this.repositorioPotreros = repositorioPotreros;
            this.repositorioHistoriaClinica = repositorioHistoriaClinica;
        }

        public ResultadoOperacion RegistrarEvento(string potreroId, string nombreRes, DateTime fecha, string tipoEvento, string descripcion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipoEvento))
                {
                    return ResultadoOperacion.Fallo("El tipo de evento clínico no puede estar vacío");
                }

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    return ResultadoOperacion.Fallo("La descripción del evento clínico no puede estar vacía");
                }

                Potrero potrero = repositorioPotreros.ObtenerPorId(potreroId);
                if (potrero == null)
                {
                    return ResultadoOperacion.Fallo($"No se encontró el potrero '{potreroId}'");
                }

                Res res;
                try
                {
                    res = potrero.BuscarRes(nombreRes);
                }
                catch (Exception ex)
                {
                    return ResultadoOperacion.Fallo($"No se encontró la res '{nombreRes}' en el potrero '{potreroId}': {ex.Message}");
                }

                var evento = new EventoClinico(potrero.Identificacion, res.Nombre, fecha, tipoEvento, descripcion);
                repositorioHistoriaClinica.Agregar(evento);

                return ResultadoOperacion.Ok($"Evento clínico registrado correctamente para la res {res.Nombre}.");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el método RegistrarEvento: " + ex.Message);
            }
        }

        public List<EventoClinico> ConsultarHistoria(string potreroId, string nombreRes)
        {
            try
            {
                return repositorioHistoriaClinica.ObtenerPorRes(potreroId, nombreRes);
            }
            catch
            {
                return new List<EventoClinico>();
            }
        }
    }
}
