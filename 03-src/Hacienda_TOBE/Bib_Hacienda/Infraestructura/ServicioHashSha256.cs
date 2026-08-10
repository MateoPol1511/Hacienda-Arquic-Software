using System;
using System.Security.Cryptography;
using System.Text;
using Bib_Hacienda.Aplicacion;

namespace Bib_Hacienda.Infraestructura
{
    // Implementación concreta de IServicioHash (Bib_Hacienda.Aplicacion.Autorizacion).
    // El AS-IS (Bib_Hacienda.Clases.Autenticacion / UsuarioService) no hasheaba
    // contraseñas: las comparaba en texto plano (u.Contrasena == contrasena).
    // El UML TO-BE introduce IServicioHash como abstracción nueva (H-09, DIP)
    // para que ServicioAutenticacion deje de manejar contraseñas en claro; no
    // existe una implementación previa que "portar" desde el AS-IS.
    //
    // Se usa SHA-256 (System.Security.Cryptography, disponible tanto en
    // net472 como en net8.0) por ser el algoritmo de hashing más simple y
    // ampliamente disponible sin agregar paquetes nuevos. No se agrega salt
    // ni iteraciones (PBKDF2/BCrypt) porque el contrato de IServicioHash
    // (Hash/Verificar, sin parámetros adicionales) no lo prevé y el UML no
    // pide un mecanismo más robusto; ver inconsistencias.
    public class ServicioHashSha256 : IServicioHash
    {
        public string Hash(string textoPlano)
        {
            if (textoPlano == null)
            {
                throw new ArgumentNullException(nameof(textoPlano));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
                StringBuilder sb = new StringBuilder(bytesHash.Length * 2);
                foreach (byte b in bytesHash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public bool Verificar(string textoPlano, string hash)
        {
            if (textoPlano == null || hash == null)
            {
                return false;
            }

            string hashCalculado = Hash(textoPlano);
            return string.Equals(hashCalculado, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
