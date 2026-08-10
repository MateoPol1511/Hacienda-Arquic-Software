using System;
using System.Globalization;
using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Infraestructura
{
    // SC-3: reutiliza ISerializador<T> (no se crea una interfaz nueva, ver
    // nota en ISerializador.cs). Formato de línea para HistoriaClinica.txt:
    // "PotreroId|NombreRes|Fecha(yyyy-MM-dd)|TipoEvento|Descripcion".
    //
    // Descripcion es el ÚLTIMO campo de la línea. Al deserializar se separa
    // con un límite de 5 partes (Split('|', 5)) para que un '|' dentro de la
    // descripción no rompa el formato; al serializar se sanea la
    // descripción quitando saltos de línea, para no partir el archivo en
    // líneas adicionales.
    public class SerializadorEventoClinico : ISerializador<EventoClinico>
    {
        public string Serializar(EventoClinico entidad)
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            string fecha = entidad.Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string descripcion = (entidad.Descripcion ?? string.Empty).Replace("\r", " ").Replace("\n", " ");

            return $"{entidad.PotreroId}|{entidad.NombreRes}|{fecha}|{entidad.TipoEvento}|{descripcion}";
        }

        public EventoClinico Deserializar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                throw new ArgumentException("La línea de evento clínico a deserializar no puede estar vacía.", nameof(linea));
            }

            var partes = linea.Split('|', 5);
            if (partes.Length < 5)
            {
                throw new FormatException($"Línea de evento clínico con formato inválido: '{linea}'");
            }

            string potreroId = partes[0];
            string nombreRes = partes[1];

            if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
            {
                throw new FormatException($"Fecha de evento clínico con formato inválido: '{partes[2]}'");
            }

            string tipoEvento = partes[3];
            string descripcion = partes[4];

            return new EventoClinico(potreroId, nombreRes, fecha, tipoEvento, descripcion);
        }
    }
}
