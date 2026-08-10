using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Dominio.Eventos;
using Bib_Hacienda.Dominio.Reglas;

namespace Bib_Hacienda.Dominio
{
    // SRP (H-07/H-15): ya no instancia con switch ni resuscribe eventos por
    // llamada distinta a lo que sigue abajo; AgregarRes recibe la res ya
    // construida (en un bloque posterior: por IRegistroFabricasRes). El
    // AS-IS construía Ternero/Cebon/Novillo dentro de anadir_res según un
    // switch sobre el enum l_tipos_potreros; esa responsabilidad se traslada
    // fuera del dominio y por eso no se conserva aquí.
    //
    // Tipo_potrero pasa de "enum l_tipos_potreros" a "string" (clave que en
    // un bloque posterior indexará RegistroFabricasRes): gana OCP, pierde
    // chequeo de tipos en compilación. El enum anidado l_tipos_potreros del
    // AS-IS se elimina en consecuencia (ver inconsistencias).
    //
    // Los Publisher* se reciben por constructor (inyección), ya no se crean
    // con "new" dentro de Potrero como en el AS-IS. PublisherPesoMin y
    // PublisherPesoVenta ya no son responsabilidad de Potrero: el UML los
    // mueve a ServicioAlimentacion (fuera de este bloque).
    public class Potrero
    {
        //Atributos
        private string identificacion;
        private List<Res> l_reses;
        private string tipo_potrero;

        //Eventos (inyectados)
        private PublisherPotreroMitad publisher_potrero_mitad;
        private PublisherPotreroLleno publisher_potrero_lleno;

        //Constructor
        public Potrero(string identificacion, string tipo_potrero, PublisherPotreroMitad potreroMitad, PublisherPotreroLleno potreroLleno)
        {
            this.Identificacion = identificacion;
            this.Tipo_potrero = tipo_potrero;
            this.publisher_potrero_mitad = potreroMitad;
            this.publisher_potrero_lleno = potreroLleno;
            this.l_reses = new List<Res>();
        }

        //Accesores
        public string Identificacion { get => identificacion; set => identificacion = value; }
        public List<Res> L_reses { get => l_reses; set => l_reses = value; }
        public string Tipo_potrero { get => tipo_potrero; set => tipo_potrero = value; }

        //Indica si el potrero tiene capacidad para una res más
        public bool TieneCapacidad()
        {
            return l_reses.Count() < ReglaPotrero.max_reses_potrero;
        }

        //Añade una res ya construida al potrero, si hay capacidad.
        //H-14 (SRP): retorna ResultadoOperacion en vez de string; el texto
        //del mensaje de éxito y de potrero lleno es el MISMO que en el AS-IS.
        public ResultadoOperacion AgregarRes(Res res)
        {
            try
            {
                if (res == null)
                {
                    return ResultadoOperacion.Fallo("La res a añadir no puede ser nula");
                }

                if (!TieneCapacidad())
                {
                    return ResultadoOperacion.Fallo($"La res no puede ser añadida al potrero {this.identificacion} porque este está lleno");
                }

                l_reses.Add(res);

                //Cuenta las reses actuales en el potrero
                ushort cantidad_reses = (ushort)L_reses.Count();

                string mensajes_eventos = "";

                //Suscribirse a los eventos ANTES de dispararlos
                publisher_potrero_mitad.evt_potrero_mitad += mensaje =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajes_eventos += mensaje + "\n";
                };

                publisher_potrero_lleno.evt_potrero_lleno += mensaje =>
                {
                    if (!string.IsNullOrEmpty(mensaje))
                        mensajes_eventos += mensaje + "\n";
                };

                //AHORA SÍ disparar los eventos (después de suscribirnos)
                publisher_potrero_mitad.Informar_Potrero_Mitad(cantidad_reses, this);
                publisher_potrero_lleno.Informar_Potrero_Lleno(cantidad_reses, this);

                //Construir mensaje de retorno
                string mensaje_final = $"La res {res.Nombre} ha sido añadida al potrero {this.identificacion} con exito.";
                if (!string.IsNullOrEmpty(mensajes_eventos))
                {
                    mensaje_final += "\n" + mensajes_eventos.TrimEnd();
                }

                return ResultadoOperacion.Ok(mensaje_final);
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Fallo("Error inesperado en el metodo AgregarRes: " + ex.Message);
            }
        }

        //Metodo para buscar res por el nombre. Conserva la lógica y los
        //mensajes del AS-IS (buscar_res); el UML no lo marca para el cambio
        //a ResultadoOperacion (retorna Res, no ResultadoOperacion).
        public Res BuscarRes(string nombre)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                // Buscar la res que contengan el texto (ignorando mayúsculas/minúsculas)
                var res_encontrada = l_reses
                    .Where(p => p.Nombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Si no hay resultados
                if (res_encontrada.Count == 0)
                {
                    throw new Exception($"No se encontró ningúna vaca con el nombre o coincidencia '{nombre}'.");
                }

                // Si hay más de un resultado, mostrar opciones
                if (res_encontrada.Count > 1)
                {
                    throw new Exception($" se encontró mas de una res con el nombre o coincidencia '{nombre}'.");
                }

                //  devolver res
                return res_encontrada.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }
    }
}
