using System;
using System.Globalization;
using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Infraestructura
{
    // Conserva el formato de Vacunas.txt del AS-IS
    // (PersistenciaService.GuardarVacunas / CargarVacunas):
    // "Nombre|Lote|FechaVencimiento(yyyy-MM-dd)|FechaAplicacion(yyyy-MM-dd)|Tipo|Periodo".
    // Periodo es el Periodo_aplicacion de Bacteriana, o 0 si la vacuna es Viva
    // (idéntico al AS-IS).
    //
    // Reutilización (sin inventar formato nuevo): el AS-IS también persistía
    // "vacunas aplicadas" en VacunasAplicadas.txt, con líneas
    // "PotreroId|NombreRes|Nombre|Lote|FechaVenc|FechaAplic|Tipo|Periodo". Esa
    // línea es, otra vez, "PotreroId|NombreRes|" seguido de exactamente los
    // mismos 6 campos que este serializador ya produce/consume para Vacuna.
    // El UML de este bloque no define un ISerializador ni una entidad
    // separada para "vacuna aplicada" (no hay una clase VacunaAplicada: una
    // vacuna aplicada sigue siendo una Vacuna dentro de
    // Res.L_vacunas_aplicadas), así que no se inventa una interfaz ni un
    // formato nuevos aquí. El repositorio que en un bloque posterior escriba
    // VacunasAplicadas.txt puede reutilizar este mismo SerializadorVacuna,
    // anteponiendo "PotreroId|NombreRes|" al resultado de Serializar (y
    // quitándolo antes de llamar a Deserializar), igual que se documenta en
    // SerializadorRes para Reses.txt.
    public class SerializadorVacuna : ISerializador<Vacuna>
    {
        public string Serializar(Vacuna entidad)
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            string fechaVenc = entidad.Fecha_vencimiento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string fechaAplic = entidad.Fecha_aplicacion.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string tipo = entidad.GetType().Name;
            uint periodo = entidad is Bacteriana bacteriana ? bacteriana.Periodo_aplicacion : 0;

            return $"{entidad.Nombre}|{entidad.Lote}|{fechaVenc}|{fechaAplic}|{tipo}|{periodo}";
        }

        public Vacuna Deserializar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                throw new ArgumentException("La línea de vacuna a deserializar no puede estar vacía.", nameof(linea));
            }

            var partes = linea.Split('|');
            if (partes.Length < 6)
            {
                throw new FormatException($"Línea de vacuna con formato inválido: '{linea}'");
            }

            string nombre = partes[0];
            string lote = partes[1];

            if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVenc))
            {
                throw new FormatException($"Fecha de vencimiento con formato inválido: '{partes[2]}'");
            }

            if (!DateTime.TryParseExact(partes[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaAplic))
            {
                throw new FormatException($"Fecha de aplicación con formato inválido: '{partes[3]}'");
            }

            string tipo = partes[4].Trim();
            uint periodo = uint.TryParse(partes[5].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var per) ? per : 0u;

            // Misma decisión que el AS-IS: cualquier tipo distinto de
            // "Bacteriana" (comparación case-insensitive) se trata como Viva,
            // con atenuación por defecto Atenuacion10 (el AS-IS tampoco
            // persistía la atenuación real en Vacunas.txt).
            if (tipo.Equals("Bacteriana", StringComparison.OrdinalIgnoreCase))
            {
                return new Bacteriana(nombre, lote, fechaVenc, fechaAplic, periodo);
            }

            return new Viva(nombre, lote, fechaVenc, fechaAplic, Viva.enum_l_atenuaciones.Atenuacion10);
        }
    }
}
