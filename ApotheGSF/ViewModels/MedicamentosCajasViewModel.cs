﻿namespace ApotheGSF.ViewModels
{
    public class MedicamentosCajasViewModel
    {
        public int CodigoCaja { get; set; }
        public int CodigoMedicamento { get; set; }
        public int CantidadUnidad { get; set; }
        public DateTime FechaAdquirido { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Detallada { get; set; }
        public bool Inactivo { get; set; }
        public string? NombreMedicamento { get; set; }
        public int Cajas { get; set; }
    }
}
