using System;
using System.Collections.Generic;

namespace Bib_Hacienda.Aplicacion
{
    // OCP (H-07): una fábrica por tipo de Res, indexadas por clave de texto
    // (Potrero.Tipo_potrero). Un 4º tipo se agrega registrando una clase
    // nueva en el composition root, sin tocar Potrero ni ServicioPotreros.
    // Ver ADR-03.
    public class RegistroFabricasRes : IRegistroFabricasRes
    {
        private Dictionary<string, IFabricaRes> fabricas;

        public RegistroFabricasRes(Dictionary<string, IFabricaRes> fabricas)
        {
            this.fabricas = fabricas;
        }

        public IFabricaRes ObtenerFabrica(string tipo)
        {
            if (tipo == null || !fabricas.TryGetValue(tipo, out IFabricaRes fabrica))
            {
                throw new Exception($"No existe una fábrica registrada para el tipo de res '{tipo}'.");
            }
            return fabrica;
        }
    }
}
