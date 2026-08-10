using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio
{
    // DIP (H-09): ya no almacena la contraseña en texto plano (AS-IS: "Contrasena");
    // guarda el hash calculado por IServicioHash (Infraestructura, fuera de este
    // bloque), inyectado en ServicioAutenticacion. Usuario deja de tener que saber
    // CÓMO se hashea, solo transporta el hash ya calculado.
    public class Usuario
    {
        private string nombre;
        private string contrasenaHash;

        public Usuario(string nombre, string contrasenaHash)
        {
            this.Nombre = nombre;
            this.ContrasenaHash = contrasenaHash;
        }

        public string Nombre { get => nombre; set => nombre = value; }
        public string ContrasenaHash { get => contrasenaHash; set => contrasenaHash = value; }
    }
}
