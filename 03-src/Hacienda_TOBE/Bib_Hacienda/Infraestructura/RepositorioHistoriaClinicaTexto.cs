using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Validacion;

namespace Bib_Hacienda.Infraestructura
{
    // SC-3: mismo patrón que RepositorioVacunasTexto (H-11): un archivo de
    // texto plano en la carpeta Datos (DirectorioDatos), validación
    // explícita antes de persistir (sin AOP), altas por apéndice.
    //
    // A diferencia del inventario de vacunas, la historia clínica no tiene
    // operación de "Remover" (SC-3 solo pide asociar, registrar y
    // consultar), por lo que el archivo solo crece por apéndice, igual que
    // Vacunas.txt en sus altas.
    public class RepositorioHistoriaClinicaTexto : IRepositorioHistoriaClinica
    {
        private const string ArchivoHistoriaClinica = "HistoriaClinica.txt";

        private readonly ISerializador<EventoClinico> serializadorEventoClinico;
        private readonly IValidadorEventoClinico validadorEventoClinico;

        public RepositorioHistoriaClinicaTexto(ISerializador<EventoClinico> serializadorEventoClinico, IValidadorEventoClinico validadorEventoClinico)
        {
            this.serializadorEventoClinico = serializadorEventoClinico;
            this.validadorEventoClinico = validadorEventoClinico;
        }

        public List<EventoClinico> ObtenerPorRes(string potreroId, string nombreRes)
        {
            try
            {
                return CargarEventosDesdeArchivo()
                    .Where(e => string.Equals(e.PotreroId, potreroId, StringComparison.OrdinalIgnoreCase)
                             && string.Equals(e.NombreRes, nombreRes, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(e => e.Fecha)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar historia clínica: {ex.Message}", ex);
            }
        }

        public void Agregar(EventoClinico evento)
        {
            if (evento == null)
            {
                throw new ArgumentNullException(nameof(evento));
            }

            if (!validadorEventoClinico.EsValido(evento))
            {
                throw new Exception("Error de validación en evento clínico");
            }

            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoHistoriaClinica);
            File.AppendAllLines(ruta, new[] { serializadorEventoClinico.Serializar(evento) });
        }

        private List<EventoClinico> CargarEventosDesdeArchivo()
        {
            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoHistoriaClinica);
            var eventos = new List<EventoClinico>();

            if (!File.Exists(ruta))
            {
                return eventos;
            }

            foreach (var linea in File.ReadAllLines(ruta))
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                eventos.Add(serializadorEventoClinico.Deserializar(linea));
            }

            return eventos;
        }
    }
}
