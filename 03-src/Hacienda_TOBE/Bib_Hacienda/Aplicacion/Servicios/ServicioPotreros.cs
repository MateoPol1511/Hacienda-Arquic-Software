using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Aplicacion
{
    // SRP (H-01/H-03): agrupa las consultas que en el AS-IS estaban
    // duplicadas entre PotreroService y ResService (ambas envolvían
    // Hacienda/Persistencia con el mismo patrón). Como Res solo existe
    // dentro de un Potrero, sus consultas quedan aquí: crear un
    // ServicioReses aparte sería una interfaz sin responsabilidad propia.
    //
    // INCONSISTENCIA REGISTRADA: Potrero (Bloque 1) exige recibir
    // PublisherPotreroMitad/PublisherPotreroLleno por constructor, pero el
    // UML no le da a ServicioPotreros ninguna dependencia declarada hacia
    // esos Publisher* (solo repositorioPotreros y registroFabricas). Sin
    // inventar una interfaz o parámetro de constructor ausente del UML,
    // CrearPotrero instancia esos dos Publisher* directamente con "new"
    // (son clases sin dependencias propias, igual que hacía el AS-IS dentro
    // de Potrero antes de la inyección introducida en el Bloque 1).
    public class ServicioPotreros
    {
        private IRepositorioPotreros repositorioPotreros;
        private IRegistroFabricasRes registroFabricas;

        public ServicioPotreros(IRepositorioPotreros repositorioPotreros, IRegistroFabricasRes registroFabricas)
        {
            this.repositorioPotreros = repositorioPotreros;
            this.registroFabricas = registroFabricas;
        }

        // Misma validación/mensaje que PotreroService.CrearPotrero + Hacienda.crear_potrero del AS-IS.
        public ResultadoOperacion CrearPotrero(string identificacion, string tipo_potrero)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    return ResultadoOperacion.Fallo("El nombre de la res no puede estar vacío");
                }

                if (repositorioPotreros.Existe(identificacion))
                {
                    return ResultadoOperacion.Fallo($"Ya existe un potrero con la identificación '{identificacion}'");
                }

                Potrero nuevo_potrero = new Potrero(identificacion, tipo_potrero, new PublisherPotreroMitad(), new PublisherPotreroLleno());
                repositorioPotreros.Agregar(nuevo_potrero);

                return ResultadoOperacion.Ok($"El potrero {identificacion} se a añadido a la hacienda. ");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al crear el potrero: {ex.Message}");
            }
        }

        // Misma lógica que Hacienda.buscar_potrero del AS-IS (coincidencia parcial,
        // exige exactamente un resultado).
        public Potrero BuscarPotrero(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                var potreros_encontrados = repositorioPotreros.ObtenerTodos()
                    .Where(p => p.Identificacion.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (potreros_encontrados.Count == 0)
                {
                    throw new Exception($"No se encontró ningún potrero con el nombre o coincidencia '{nombre}'.");
                }

                if (potreros_encontrados.Count > 1)
                {
                    throw new Exception($" se encontró mas de un potrero con el nombre o coincidencia '{nombre}'.");
                }

                return potreros_encontrados.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }

        // SRP (H-07/H-15): la res ya no se construye con un switch aquí ni en
        // Potrero; se obtiene la fábrica adecuada según Tipo_potrero y se
        // valida su edad con el método polimórfico Res.EsEdadValida (LSP,
        // H-08) antes de intentar añadirla.
        public ResultadoOperacion AnadirResAPotrero(string id_potrero, string nombre, ushort edad, uint peso)
        {
            try
            {
                Potrero potrero;
                try
                {
                    potrero = BuscarPotrero(id_potrero);
                }
                catch (Exception ex)
                {
                    return ResultadoOperacion.Fallo($"No se encontró el potrero '{id_potrero}': {ex.Message}");
                }

                if (potrero.L_reses.Any(r => r.Nombre == nombre))
                {
                    return ResultadoOperacion.Fallo($"Ya existe una res con el nombre '{nombre}' en el potrero '{id_potrero}'");
                }

                IFabricaRes fabrica = registroFabricas.ObtenerFabrica(potrero.Tipo_potrero);
                Res res = fabrica.Crear(nombre, peso, edad);

                // LSP (H-08): la res ya no lanza excepción al construirse con una
                // edad fuera de rango; se consulta explícitamente aquí. El
                // mensaje deja de ser específico por subtipo (el AS-IS decía
                // "El ternero/cebon/novillo excedió la edad maxima" dentro del
                // setter de Edad) porque EsEdadValida solo expone un booleano;
                // ver inconsistencias.
                if (!res.EsEdadValida(edad))
                {
                    return ResultadoOperacion.Fallo($"La edad {edad} no es válida para el tipo de res del potrero '{id_potrero}'.");
                }

                return potrero.AgregarRes(res);
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo($"Error al agregar la res: {ex.Message}");
            }
        }

        public List<Potrero> ObtenerTodosLosPotreros()
        {
            return repositorioPotreros.ObtenerTodos().OrderBy(p => p.Identificacion).ToList();
        }

        public Potrero ObtenerPotreroPorIdentificacion(string identificacion)
        {
            try
            {
                return repositorioPotreros.ObtenerPorId(identificacion);
            }
            catch
            {
                return null;
            }
        }

        public List<(Potrero Potrero, Res Res)> ObtenerTodasLasReses()
        {
            var resesConPotrero = new List<(Potrero, Res)>();

            foreach (var potrero in repositorioPotreros.ObtenerTodos())
            {
                foreach (var res in potrero.L_reses)
                {
                    resesConPotrero.Add((potrero, res));
                }
            }

            return resesConPotrero;
        }

        public Res BuscarRes(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = repositorioPotreros.ObtenerPorId(potreroId);
                return potrero.BuscarRes(nombreRes);
            }
            catch
            {
                return null;
            }
        }

        // Fusiona las estadísticas que en el AS-IS calculaban por separado
        // PotreroService.ObtenerEstadisticas y ResService.ObtenerEstadisticas.
        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var potreros = repositorioPotreros.ObtenerTodos();
            var todasLasReses = ObtenerTodasLasReses();

            return new Dictionary<string, object>
            {
                { "TotalPotreros", potreros.Count },
                { "TotalReses", todasLasReses.Count },
                { "PotrerosVacios", potreros.Count(p => p.L_reses.Count == 0) },
                { "PotrerosConReses", potreros.Count(p => p.L_reses.Count > 0) },
                { "Terneros", todasLasReses.Count(r => r.Res is Ternero) },
                { "Cebones", todasLasReses.Count(r => r.Res is Cebon) },
                { "Novillos", todasLasReses.Count(r => r.Res is Novillo) },
                { "PesoPromedio", todasLasReses.Any() ? todasLasReses.Average(r => r.Res.Peso) : 0 }
            };
        }
    }
}
