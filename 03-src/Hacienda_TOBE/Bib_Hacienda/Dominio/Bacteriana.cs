using Bib_Hacienda.Dominio.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Dominio
{
    // Conservada íntegramente del AS-IS (Bib_Hacienda.Clases.Bacteriana),
    // solo cambia de namespace según el paquete "Bib_Hacienda.Dominio" del UML.
    // Sigue dependiendo de ReglaVacuna para los límites del periodo de aplicación.
    public class Bacteriana : Vacuna //Hereda de Vacuna
    {
        //Atributos
        private uint periodo_aplicacion;

        //Constructor
        public Bacteriana(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion) : base(nombre, lote, fecha_vencimiento, fecha_aplicacion)
        {
            this.Periodo_aplicacion = periodo_aplicacion;
        }

        //Accesores
        public uint Periodo_aplicacion
        {
            get => periodo_aplicacion;
            set => periodo_aplicacion = value >= ReglaVacuna.periodo_min_bac_aplic && value <= ReglaVacuna.periodo_max_bac_aplic ? value :
                throw new Exception($"el valor del periodo de aplicacion debe estar entre {ReglaVacuna.periodo_min_bac_aplic} y {ReglaVacuna.periodo_max_bac_aplic} semanas");
        }
    }
}
