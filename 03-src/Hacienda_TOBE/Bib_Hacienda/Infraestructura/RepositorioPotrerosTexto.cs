using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bib_Hacienda.Aplicacion;
using Bib_Hacienda.Dominio;
using Bib_Hacienda.Dominio.Validacion;

namespace Bib_Hacienda.Infraestructura
{
    // H-11/H-12: la validación que en el AS-IS hacía PersistenciaService
    // mediante proxies de Castle DynamicProxy (InterceptorValidarInformacion)
    // ahora es una llamada explícita del repositorio al validador
    // correspondiente antes de persistir. Mismo comportamiento observable,
    // sin AOP.
    //
    // Persiste en dos archivos, igual que el AS-IS:
    // - Potreros.txt: una línea por potrero (serializadorPotrero).
    // - Reses.txt: una línea por res, con el mismo formato de
    //   PersistenciaService.GuardarReses/CargarReses
    //   ("PotreroId|<línea de serializadorRes>"). serializadorRes solo
    //   conoce los campos propios de la res (ver SerializadorRes, Bloque
    //   3A); este repositorio antepone/quita el PotreroId, que es la
    //   relación potrero-res, no un dato de la res.
    //
    // INCONSISTENCIA REGISTRADA: IRepositorioPotreros no declara ningún
    // método para volver a persistir un Potrero ya existente después de
    // mutarlo (por ejemplo, tras Potrero.AgregarRes, o tras remover una res
    // vendida). Agregar(potrero) solo se invoca hoy al crear un potrero
    // nuevo (ServicioPotreros.CrearPotrero), con L_reses vacía. Este
    // repositorio persiste fielmente cualquier res que el Potrero recibido
    // ya traiga en L_reses en el momento de llamar a Agregar, pero las
    // altas/bajas de reses que ocurren DESPUÉS (AnadirResAPotrero,
    // vender_res, aplicar_vacuna) modifican el grafo cargado solo en
    // memoria: no hay, en el UML de este bloque, una forma de volver a
    // guardarlas. Se deja pendiente para un bloque posterior (requeriría
    // ampliar IRepositorioPotreros, fuera del alcance de "no inventar
    // métodos").
    public class RepositorioPotrerosTexto : IRepositorioPotreros
    {
        private const string ArchivoPotreros = "Potreros.txt";
        private const string ArchivoReses = "Reses.txt";

        private readonly ISerializador<Potrero> serializadorPotrero;
        private readonly ISerializador<Res> serializadorRes;
        private readonly IValidadorPotrero validadorPotrero;
        private readonly IValidadorRes validadorRes;

        public RepositorioPotrerosTexto(ISerializador<Potrero> serializadorPotrero, ISerializador<Res> serializadorRes, IValidadorPotrero validadorPotrero, IValidadorRes validadorRes)
        {
            this.serializadorPotrero = serializadorPotrero;
            this.serializadorRes = serializadorRes;
            this.validadorPotrero = validadorPotrero;
            this.validadorRes = validadorRes;
        }

        public List<Potrero> ObtenerTodos()
        {
            try
            {
                var potreros = CargarPotrerosDesdeArchivo();
                CargarResesDentroDePotreros(potreros);
                return potreros;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar potreros: {ex.Message}", ex);
            }
        }

        public Potrero ObtenerPorId(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return null;
            }

            return ObtenerTodos()
                .FirstOrDefault(p => string.Equals(p.Identificacion, identificacion, StringComparison.OrdinalIgnoreCase));
        }

        public bool Existe(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return false;
            }

            return ObtenerTodos()
                .Any(p => string.Equals(p.Identificacion, identificacion, StringComparison.OrdinalIgnoreCase));
        }

        public void Agregar(Potrero potrero)
        {
            if (potrero == null)
            {
                throw new ArgumentNullException(nameof(potrero));
            }

            if (!validadorPotrero.EsValido(potrero))
            {
                throw new Exception("Error de validación en potrero");
            }

            foreach (var res in potrero.L_reses)
            {
                if (!validadorRes.EsValido(res))
                {
                    throw new Exception("Error de validación en res");
                }
            }

            string rutaPotreros = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoPotreros);
            File.AppendAllLines(rutaPotreros, new[] { serializadorPotrero.Serializar(potrero) });

            if (potrero.L_reses.Count > 0)
            {
                string rutaReses = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoReses);
                var lineasReses = potrero.L_reses
                    .Select(res => $"{potrero.Identificacion}|{serializadorRes.Serializar(res)}");
                File.AppendAllLines(rutaReses, lineasReses);
            }
        }

        // Misma lógica que PersistenciaService.CargarPotreros del AS-IS:
        // normaliza identificaciones y evita duplicados (case-insensitive).
        private List<Potrero> CargarPotrerosDesdeArchivo()
        {
            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoPotreros);
            var potreros = new List<Potrero>();

            if (!File.Exists(ruta))
            {
                return potreros;
            }

            foreach (var linea in File.ReadAllLines(ruta))
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                var potrero = serializadorPotrero.Deserializar(linea);

                if (!potreros.Any(p => string.Equals(p.Identificacion, potrero.Identificacion, StringComparison.OrdinalIgnoreCase)))
                {
                    potreros.Add(potrero);
                }
            }

            return potreros;
        }

        // Misma lógica que PersistenciaService.CargarReses del AS-IS: cada
        // línea trae el PotreroId al frente; el resto de la línea es el
        // formato propio de serializadorRes.
        private void CargarResesDentroDePotreros(List<Potrero> potreros)
        {
            string ruta = Path.Combine(DirectorioDatos.ObtenerRuta(), ArchivoReses);

            if (!File.Exists(ruta))
            {
                return;
            }

            foreach (var linea in File.ReadAllLines(ruta))
            {
                if (string.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }

                int separador = linea.IndexOf('|');
                if (separador < 0)
                {
                    continue;
                }

                string potreroId = linea.Substring(0, separador).Trim();
                string lineaRes = linea.Substring(separador + 1);

                var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, potreroId, StringComparison.OrdinalIgnoreCase));
                if (potrero == null)
                {
                    continue;
                }

                var res = serializadorRes.Deserializar(lineaRes);
                potrero.L_reses.Add(res);
            }
        }
    }
}
