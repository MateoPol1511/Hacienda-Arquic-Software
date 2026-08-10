using Bib_Hacienda.Dominio.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio
{
    // OCP (H-10): miembros polimórficos (PesoMinimo, PesoRecomendadoVenta,
    // MaxVacunasBacterianas, MaxVacunasVivas) que reemplazan los
    // "is Ternero/Cebon/Novillo" repetidos en Publishers y en aplicar_vacuna.
    //
    // LSP (H-08): Edad ya NO lanza excepción al asignar (a diferencia del AS-IS,
    // donde Ternero/Cebon/Novillo sobrescribían el setter y lanzaban Exception si
    // la edad no correspondía). Ahora es una propiedad simple; la validez se
    // consulta aparte con EsEdadValida(), invocada por quien crea la res
    // (en un bloque posterior: el servicio de aplicación). Las reglas de negocio
    // de rango de edad NO cambian, solo el mecanismo para comunicarlas.
    public abstract class Res : IActivoVendible
    {
        //Atributos
        private string nombre;
        private uint peso;
        private ushort edad;
        private List<Vacuna> l_vacunas_aplicadas;

        //Constructor
        public Res(string nombre, uint peso, ushort edad)
        {
            this.Nombre = nombre;
            this.Peso = peso;
            this.Edad = edad;
            this.l_vacunas_aplicadas = new List<Vacuna>();
        }

        //Accesores
        public string Nombre { get => nombre; set => nombre = value; }
        public uint Peso { get => peso; set => peso = value; }
        public ushort Edad { get => edad; set => edad = value; }
        public List<Vacuna> L_vacunas_aplicadas { get => l_vacunas_aplicadas; set => l_vacunas_aplicadas = value; }

        // Implementación de IActivoVendible. El UML no especifica de dónde
        // proviene el valor; Res no tiene otro identificador propio distinto
        // del nombre en el AS-IS, así que se usa Nombre. Ver inconsistencias.
        public string Identificador => Nombre;

        //Miembros polimórficos (OCP, H-10)
        public abstract ushort PesoMinimo { get; }
        public abstract ushort PesoRecomendadoVenta { get; }
        public abstract byte MaxVacunasBacterianas { get; }
        public abstract byte MaxVacunasVivas { get; }
        public abstract bool EsEdadValida(ushort edad);
    }
}
