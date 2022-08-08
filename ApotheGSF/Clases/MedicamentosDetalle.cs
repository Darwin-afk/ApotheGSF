﻿namespace ApotheGSF.Clases
{
    public class MedicamentosDetalle
    {
        public int CodigoDetalle { get; set; }
        public List<int> CodigosCajas { get; set; }
        public string NombreMedicamento { get; set; }
        public int TipoCantidad { get; set; }
        public int Cantidad { get; set; }
        public float Precio { get; set; }
        public float Total { get; set; }
        public bool Abierto { get; set; }
        public int CantidadAbierto { get; set; }
    }
}
