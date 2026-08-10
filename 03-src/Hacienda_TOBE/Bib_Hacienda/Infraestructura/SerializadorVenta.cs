using System;
using System.Globalization;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Eventos;

namespace Bib_Hacienda.Infraestructura
{
    // Conserva el formato de Ventas.txt del AS-IS
    // (PersistenciaService.GuardarVentas / CargarVentas):
    // "PotreroId|Fecha(yyyy-MM-dd)|NombreRes|PesoRes|EdadRes|TipoRes|Monto".
    //
    // Diferencia de tipos frente al AS-IS: Venta.Res (Res) pasó a ser
    // Venta.Activo (IActivoVendible), para permitir a futuro productos
    // vendibles que no sean Res (SC-1, fuera de este bloque). Hoy la única
    // implementación de IActivoVendible es Res, así que este serializador
    // solo sabe volcar a texto el caso en que Activo sea una Res; si en un
    // futuro bloque aparece otro IActivoVendible, este serializador tendría
    // que revisarse (o Venta necesitaría su propio serializador por tipo de
    // activo). No se maneja aquí para no inventar un formato de archivo que
    // el UML de este bloque no define.
    //
    // Nota de diseño (SRP, H-11), igual que en SerializadorRes: la línea de
    // Ventas.txt combina Potrero + Res + Venta. Serializar SÍ puede volcar
    // todo porque recibe el grafo completo (venta.Potrero, venta.Activo).
    // Deserializar, en cambio, solo recibe la línea de texto: no tiene forma
    // de recuperar la instancia real de Potrero (eso vive en
    // RepositorioPotrerosTexto, fuera de este bloque). Por eso, igual que
    // hacía PersistenciaService.CargarVentas en el AS-IS cuando el potrero
    // de la venta no existía todavía en la lista cargada, aquí se construye
    // un Potrero "cascarón" con la misma Identificacion y Tipo_potrero vacío;
    // se espera que el repositorio (Bloque 3B) lo reemplace por la instancia
    // real cuando corresponda.
    public class SerializadorVenta : ISerializador<Venta>
    {
        public string Serializar(Venta entidad)
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            if (!(entidad.Activo is Res res))
            {
                throw new NotSupportedException(
                    "SerializadorVenta solo admite ventas cuyo Activo sea una Res (Ternero/Novillo/Cebon); " +
                    "otros IActivoVendible quedan fuera de este bloque (SC-1).");
            }

            string fecha = entidad.Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string tipoRes = res.GetType().Name;

            return $"{entidad.Potrero.Identificacion}|{fecha}|{res.Nombre}|{res.Peso}|{res.Edad}|{tipoRes}|{entidad.Monto}";
        }

        public Venta Deserializar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                throw new ArgumentException("La línea de venta a deserializar no puede estar vacía.", nameof(linea));
            }

            var partes = linea.Split('|');
            if (partes.Length < 7)
            {
                throw new FormatException($"Línea de venta con formato inválido: '{linea}'");
            }

            string potreroId = partes[0].Trim();

            if (!DateTime.TryParseExact(partes[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
            {
                throw new FormatException($"Fecha de venta con formato inválido: '{partes[1]}'");
            }

            string nombreRes = partes[2];
            uint peso = uint.Parse(partes[3], CultureInfo.InvariantCulture);
            ushort edad = ushort.Parse(partes[4], CultureInfo.InvariantCulture);
            string tipoRes = partes[5].Trim();
            uint monto = uint.Parse(partes[6], CultureInfo.InvariantCulture);

            Res res = tipoRes switch
            {
                "Ternero" => new Ternero(nombreRes, peso, edad),
                "Novillo" => new Novillo(nombreRes, peso, edad),
                "Cebon" => new Cebon(nombreRes, peso, edad),
                _ => new Ternero(nombreRes, peso, edad)
            };

            var potrero = new Potrero(potreroId, string.Empty, new PublisherPotreroMitad(), new PublisherPotreroLleno());

            return new Venta(potrero, fecha, res, monto);
        }
    }
}
