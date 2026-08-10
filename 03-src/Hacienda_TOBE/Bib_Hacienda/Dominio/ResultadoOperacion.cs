using System;

namespace Bib_Hacienda.Dominio
{
    // Nuevo en el TO-BE (H-08/H-14). Sustituye a "string" y a las excepciones
    // usadas como control de flujo. Mensaje conserva el MISMO texto que antes
    // devolvían los métodos: la salida observable para el usuario final no cambia.
    public class ResultadoOperacion
    {
        public bool Exito { get; }
        public string Mensaje { get; }

        public ResultadoOperacion(bool exito, string mensaje)
        {
            Exito = exito;
            Mensaje = mensaje;
        }

        public static ResultadoOperacion Ok(string mensaje)
        {
            return new ResultadoOperacion(true, mensaje);
        }

        public static ResultadoOperacion Fallo(string mensaje)
        {
            return new ResultadoOperacion(false, mensaje);
        }
    }
}
