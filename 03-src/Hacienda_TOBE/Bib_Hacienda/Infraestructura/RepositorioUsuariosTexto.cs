using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;

namespace Bib_Hacienda.Infraestructura
{
    // Misma lógica que PersistenciaService.GuardarUsuarios/CargarUsuarios
    // del AS-IS ("validación simple, sin proxies"). El UML no le asigna a
    // esta clase ningún IValidador* (a diferencia de los otros tres
    // RepositorioXTexto): la validación simple de nombre/contraseña no
    // vacíos ya la hace ServicioAutenticacion.crear_usuario antes de llamar
    // a Agregar (ver Aplicacion/Servicios/ServicioAutenticacion.cs), así que
    // este repositorio no repite esa validación.
    //
    // El UML no muestra explícitamente el constructor de esta clase (a
    // diferencia de los otros tres RepositorioXTexto, que sí lo declaran),
    // pero el campo "- serializadorUsuario : ISerializador<Usuario>" no
    // podría inicializarse de otra forma sin violar encapsulamiento; se
    // agrega el constructor mínimo necesario para recibirlo, análogo al
    // patrón de los demás repositorios.
    public class RepositorioUsuariosTexto : IRepositorioUsuarios
    {
        private const string ArchivoUsuarios = "Usuarios.txt";

        private readonly ISerializador<Usuario> serializadorUsuario;

        public RepositorioUsuariosTexto(ISerializador<Usuario> serializadorUsuario)
        {
            this.serializadorUsuario = serializadorUsuario;
        }

        public List<Usuario> ObtenerTodos()
        {
            try
            {
                string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoUsuarios);
                var usuarios = new List<Usuario>();

                if (!File.Exists(ruta))
                {
                    return usuarios;
                }

                foreach (var linea in File.ReadAllLines(ruta))
                {
                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue;
                    }

                    usuarios.Add(serializadorUsuario.Deserializar(linea));
                }

                return usuarios;
            }
            catch (Exception ex)
            {
                // Misma degradación que CargarUsuarios en el AS-IS: no
                // interrumpe la aplicación por un archivo de usuarios
                // corrupto, retorna lista vacía.
                Console.WriteLine($"Error al cargar usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        public Usuario BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return null;
            }

            return ObtenerTodos()
                .FirstOrDefault(u => string.Equals(u.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        }

        public void Agregar(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoUsuarios);
            File.AppendAllLines(ruta, new[] { serializadorUsuario.Serializar(usuario) });
        }
    }
}
