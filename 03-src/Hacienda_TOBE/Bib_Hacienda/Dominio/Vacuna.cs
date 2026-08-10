using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio
{
    // Conservada íntegramente del AS-IS (Bib_Hacienda.Clases.Vacuna),
    // solo cambia de namespace según el paquete "Bib_Hacienda.Dominio" del UML.
    public abstract class Vacuna
    {
        //Atributos
        private string nombre;
        private string lote;
        private DateTime fecha_vencimiento;
        private DateTime fecha_aplicacion;

        //Constructor
        public Vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion)
        {
            this.Nombre = nombre;
            this.Lote = lote;
            this.Fecha_vencimiento = fecha_vencimiento;
            this.Fecha_aplicacion = fecha_aplicacion;
        }

        //Accesores
        public string Nombre { get => nombre; set => nombre = value; }
        public string Lote { get => lote; set => lote = value; }
        public DateTime Fecha_vencimiento { get => fecha_vencimiento; set => fecha_vencimiento = value; }
        public DateTime Fecha_aplicacion { get => fecha_aplicacion; set => fecha_aplicacion = value; }
    }
}
