using System;
using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Infraestructura
{
    // Conserva el formato de Usuarios.txt del AS-IS
    // (PersistenciaService.GuardarUsuarios / CargarUsuarios): "Nombre|Contrasena".
    //
    // Diferencia de tipos frente al AS-IS: Usuario.Contrasena pasó a
    // Usuario.ContrasenaHash (ver Usuario.cs del TO-BE): el dominio ya no
    // transporta la contraseña en texto plano, sino el hash calculado por
    // IServicioHash (Infraestructura, fuera de este bloque). Este
    // serializador no calcula ni verifica el hash: solo vuelca/lee el valor
    // que ya trae el objeto Usuario, igual que el AS-IS volcaba/leía
    // Contrasena tal cual. El formato de archivo (dos campos separados por
    // "|") no cambia.
    public class SerializadorUsuario : ISerializador<Usuario>
    {
        public string Serializar(Usuario entidad)
        {
            if (entidad == null)
            {
                throw new ArgumentNullException(nameof(entidad));
            }

            return $"{entidad.Nombre}|{entidad.ContrasenaHash}";
        }

        public Usuario Deserializar(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                throw new ArgumentException("La línea de usuario a deserializar no puede estar vacía.", nameof(linea));
            }

            var partes = linea.Split('|');
            if (partes.Length < 2)
            {
                throw new FormatException($"Línea de usuario con formato inválido: '{linea}'");
            }

            string nombre = partes[0];
            string contrasenaHash = partes[1];

            return new Usuario(nombre, contrasenaHash);
        }
    }
}
