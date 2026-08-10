using Bib_Hacienda.Dominio;
using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bib_Hacienda.Dominio.Viva;

namespace Bib_Hacienda.Aplicacion
{
    public class ServicioInventarioVacunas : ICreacionVacuna
    {
        private IRepositorioVacunas repositorioVacunas;
        private IFabricaVacunaBacteriana fabricaBacteriana;
        private IFabricaVacunaViva fabricaViva;

        public ServicioInventarioVacunas(IRepositorioVacunas repositorioVacunas, IFabricaVacunaBacteriana fabricaBacteriana, IFabricaVacunaViva fabricaViva)
        {
            this.repositorioVacunas = repositorioVacunas;
            this.fabricaBacteriana = fabricaBacteriana;
            this.fabricaViva = fabricaViva;
        }

        // Misma lógica que Hacienda.crear_vacuna (bacteriana individual) del AS-IS.
        public ResultadoOperacion crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return ResultadoOperacion.Fallo("El nombre de la vacuna no puede estar vacío");

                if (string.IsNullOrWhiteSpace(lote))
                    return ResultadoOperacion.Fallo("El lote de la vacuna no puede estar vacío");

                if (fecha_vencimiento <= fecha_aplicacion)
                    return ResultadoOperacion.Fallo("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                if (repositorioVacunas.ObtenerDisponibles().Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                    return ResultadoOperacion.Fallo($"Ya existe una vacuna con el lote '{lote}' en el inventario");

                Bacteriana nueva_vacuna = fabricaBacteriana.Crear(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                repositorioVacunas.Agregar(nueva_vacuna);

                return ResultadoOperacion.Ok($"Vacuna bacteriana '{nombre}' del lote '{lote}' agregada al inventario con éxito. Período de aplicación: {periodo_aplicacion} semanas.");
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el método crear_vacuna (bacteriana): " + er.Message);
            }
        }

        // Misma lógica que Hacienda.crear_vacuna (viva individual) del AS-IS.
        public ResultadoOperacion crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return ResultadoOperacion.Fallo("El nombre de la vacuna no puede estar vacío");

                if (string.IsNullOrWhiteSpace(lote))
                    return ResultadoOperacion.Fallo("El lote de la vacuna no puede estar vacío");

                if (fecha_vencimiento <= fecha_aplicacion)
                    return ResultadoOperacion.Fallo("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                if (repositorioVacunas.ObtenerDisponibles().Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                    return ResultadoOperacion.Fallo($"Ya existe una vacuna con el lote '{lote}' en el inventario");

                Viva nueva_vacuna = fabricaViva.Crear(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                repositorioVacunas.Agregar(nueva_vacuna);

                return ResultadoOperacion.Ok($"Vacuna viva '{nombre}' del lote '{lote}' agregada al inventario con éxito. Grado de atenuación: {(int)grado_atenuacion}.");
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el método crear_vacuna (viva): " + er.Message);
            }
        }

        // Misma lógica que Hacienda.crear_vacuna (lote bacteriano) del AS-IS.
        // NOTA: se conserva textualmente el defecto del AS-IS en el mensaje de
        // éxito, donde la línea "- Nombre: {nombre}" no está interpolada (falta
        // el prefijo $) y por lo tanto imprime literalmente "{nombre}" en vez
        // del valor. No se corrige porque el mensaje/resultado observable no
        // debe cambiar salvo necesidad estricta del TO-BE.
        public ResultadoOperacion crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            try
            {
                if (cantidad <= 0)
                    return ResultadoOperacion.Fallo("La cantidad debe ser mayor a 0");

                if (cantidad > 100)
                    return ResultadoOperacion.Fallo("No se pueden crear más de 100 vacunas en un solo lote");

                if (string.IsNullOrWhiteSpace(nombre))
                    return ResultadoOperacion.Fallo("El nombre de la vacuna no puede estar vacío");

                if (string.IsNullOrWhiteSpace(lote_base))
                    return ResultadoOperacion.Fallo("El lote base no puede estar vacío");

                if (fecha_vencimiento <= fecha_aplicacion)
                    return ResultadoOperacion.Fallo("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                int vacunas_creadas = 0;

                for (int i = 1; i <= cantidad; i++)
                {
                    string lote_numerado = $"{lote_base}-{i:D3}";

                    if (repositorioVacunas.ObtenerDisponibles().Any(v => v.Lote.Equals(lote_numerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    Bacteriana nueva_vacuna = fabricaBacteriana.Crear(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                    repositorioVacunas.Agregar(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    return ResultadoOperacion.Fallo("No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return ResultadoOperacion.Ok(
                    $"Lote de vacunas bacterianas creado con éxito:\n" +
                    "- Nombre: {nombre}\n" +
                    $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                    $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                    $"- Período de aplicación: {periodo_aplicacion} semanas");
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el método crear_vacuna (lote bacteriano): " + er.Message);
            }
        }

        // Misma lógica que Hacienda.crear_vacuna (lote vivo) del AS-IS.
        public ResultadoOperacion crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, enum_l_atenuaciones grado_atenuacion, uint cantidad)
        {
            try
            {
                if (cantidad <= 0)
                    return ResultadoOperacion.Fallo("La cantidad debe ser mayor a 0");

                if (cantidad > 100)
                    return ResultadoOperacion.Fallo("No se pueden crear más de 100 vacunas en un solo lote");

                if (string.IsNullOrWhiteSpace(nombre))
                    return ResultadoOperacion.Fallo("El nombre de la vacuna no puede estar vacío");

                if (string.IsNullOrWhiteSpace(lote_base))
                    return ResultadoOperacion.Fallo("El lote base no puede estar vacío");

                if (fecha_vencimiento <= fecha_aplicacion)
                    return ResultadoOperacion.Fallo("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

                int vacunas_creadas = 0;

                for (int i = 1; i <= cantidad; i++)
                {
                    string lote_numerado = $"{lote_base}-{i:D3}";

                    if (repositorioVacunas.ObtenerDisponibles().Any(v => v.Lote.Equals(lote_numerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    Viva nueva_vacuna = fabricaViva.Crear(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                    repositorioVacunas.Agregar(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    return ResultadoOperacion.Fallo("No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return ResultadoOperacion.Ok(
                    $"Lote de vacunas vivas creado con éxito:\n" +
                    $"- Nombre: {nombre}\n" +
                    $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                    $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                    $"- Grado de atenuación: {(int)grado_atenuacion}");
            }
            catch (Exception er)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el método crear_vacuna (lote vivo): " + er.Message);
            }
        }

        // H-17 (SRP): método fachada que decide bacteriana-vs-viva UNA sola vez
        // y delega en las sobrecargas de crear_vacuna(), igual que hacía
        // VacunaService.CrearVacuna en el AS-IS (que a su vez llamaba a
        // Hacienda.crear_vacuna). Reemplaza el if/else duplicado en
        // VacunaController y en VacunaService.
        public ResultadoOperacion CrearVacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint? periodoAplicacion, enum_l_atenuaciones? atenuacion)
        {
            if (periodoAplicacion.HasValue && !atenuacion.HasValue)
            {
                return crear_vacuna(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodoAplicacion.Value);
            }
            else if (!periodoAplicacion.HasValue && atenuacion.HasValue)
            {
                return crear_vacuna(nombre, lote, fecha_vencimiento, fecha_aplicacion, atenuacion.Value);
            }
            else
            {
                return ResultadoOperacion.Fallo("Error: parámetros inválidos para crear la vacuna (revise tipo, período o atenuación)");
            }
        }

        public List<Vacuna> ObtenerVacunasDisponibles()
        {
            return repositorioVacunas.ObtenerDisponibles().OrderBy(v => v.Nombre).ToList();
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var vacunas = repositorioVacunas.ObtenerDisponibles();

            return new Dictionary<string, object>
            {
                { "TotalVacunas", vacunas.Count },
                { "Bacterianas", vacunas.Count(v => v is Bacteriana) },
                { "Vivas", vacunas.Count(v => v is Viva) },
                { "Vencidas", vacunas.Count(v => v.Fecha_vencimiento < DateTime.Now) },
                { "Vigentes", vacunas.Count(v => v.Fecha_vencimiento >= DateTime.Now) }
            };
        }
    }
}
