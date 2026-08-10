using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Validacion;

namespace Bib_Hacienda.Infraestructura
{
    // Misma lógica que PersistenciaService.GuardarVacunas/CargarVacunas del
    // AS-IS (inventario de vacunas disponibles; no incluye vacunas ya
    // aplicadas, ver nota en SerializadorVacuna del Bloque 3A).
    //
    // Remover(vacuna) reemplaza lo que en el AS-IS era, en la práctica,
    // "sacar la vacuna de _hacienda.L_vacunas en memoria y volver a llamar a
    // GuardarVacunas con la lista completa" (VacunaService.AplicarVacuna).
    // Aquí, al no existir un método de reescritura masiva en
    // IRepositorioVacunas, Remover relee el inventario completo, descarta la
    // entrada que coincide (mismo criterio de igualdad que el resto del
    // proyecto: Nombre + Lote) y reescribe el archivo con el resto.
    public class RepositorioVacunasTexto : IRepositorioVacunas
    {
        private const string ArchivoVacunas = "Vacunas.txt";

        private readonly ISerializador<Vacuna> serializadorVacuna;
        private readonly IValidadorVacuna validadorVacuna;

        public RepositorioVacunasTexto(ISerializador<Vacuna> serializadorVacuna, IValidadorVacuna validadorVacuna)
        {
            this.serializadorVacuna = serializadorVacuna;
            this.validadorVacuna = validadorVacuna;
        }

        public List<Vacuna> ObtenerDisponibles()
        {
            try
            {
                return CargarVacunasDesdeArchivo();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vacunas: {ex.Message}", ex);
            }
        }

        public void Agregar(Vacuna vacuna)
        {
            if (vacuna == null)
            {
                throw new ArgumentNullException(nameof(vacuna));
            }

            if (!validadorVacuna.EsValido(vacuna))
            {
                throw new Exception("Error de validación en vacuna");
            }

            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoVacunas);
            File.AppendAllLines(ruta, new[] { serializadorVacuna.Serializar(vacuna) });
        }

        public void Remover(Vacuna vacuna)
        {
            if (vacuna == null)
            {
                throw new ArgumentNullException(nameof(vacuna));
            }

            var restantes = CargarVacunasDesdeArchivo()
                .Where(v => !(string.Equals(v.Nombre, vacuna.Nombre, StringComparison.Ordinal) && string.Equals(v.Lote, vacuna.Lote, StringComparison.Ordinal)))
                .ToList();

            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoVacunas);
            var lineas = restantes.Select(v => serializadorVacuna.Serializar(v));
            File.WriteAllLines(ruta, lineas);
        }

        private List<Vacuna> CargarVacunasDesdeArchivo()
        {
            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoVacunas);
            var vacunas = new List<Vacuna>();

            if (!File.Exists(ruta))
            {
                return vacunas;
            }

            foreach (var linea in File.ReadAllLines(ruta))
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                vacunas.Add(serializadorVacuna.Deserializar(linea));
            }

            return vacunas;
        }
    }
}
