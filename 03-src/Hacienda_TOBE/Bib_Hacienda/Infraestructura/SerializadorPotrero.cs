using System;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Eventos;

namespace Bib_Hacienda.Infraestructura
{
    // Conserva el formato de Potreros.txt del AS-IS
    // (PersistenciaService.GuardarPotreros / CargarPotreros):
    // "Identificacion|Tipo_potrero".
    //
    // Diferencia de tipos frente al AS-IS: Tipo_potrero pasó de
    // "enum l_tipos_potreros" a "string" (ver Potrero.cs del TO-BE), así que
    // aquí ya no se hace Enum.Parse: el valor de texto se conserva tal cual,
    // que es exactamente el mismo texto que el AS-IS escribía con
    // "l_tipos_potreros.ToString()". El contenido del archivo no cambia.
    //
    // El constructor de Potrero en el TO-BE exige PublisherPotreroMitad y
    // PublisherPotreroLleno (inyectados). Como una línea de texto no contiene
    // esa información, Deserializar crea instancias nuevas de esos
    // publishers (igual que el AS-IS, donde Potrero los creaba interna e
    // implícitamente); no hay forma de preservar identidad de publisher a
    // través de un archivo de texto en ninguno de los dos diseños.
    public class SerializadorPotrero : ISerializador<Potrero>
    {
        public string Serializar(Potrero entidad)
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            return $"{entidad.Identificacion}|{entidad.Tipo_potrero}";
        }

        public Potrero Deserializar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                throw new ArgumentException("La línea de potrero a deserializar no puede estar vacía.", nameof(linea));
            }

            var partes = linea.Split('|');
            if (partes.Length < 2)
            {
                throw new FormatException($"Línea de potrero con formato inválido: '{linea}'");
            }

            string identificacion = partes[0].Trim();
            string tipoPotrero = partes[1].Trim();

            return new Potrero(identificacion, tipoPotrero, new PublisherPotreroMitad(), new PublisherPotreroLleno());
        }
    }
}
