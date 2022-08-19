namespace ApotheGSF.Clases
{
    public class MedicamentosDetalle
    {
        public int CodigoDetalle { get; set; }
        public int CodigoCaja { get; set; }
        public int CodigoMedicamento { get; set; }
        public int CodigoLaboratorio { get; set; }
        public string NombreMedicamento { get; set; }
        public string NombreLaboratorio { get; set; }
        public int TipoCantidad { get; set; }
        public int Cantidad { get; set; }
        public float Precio { get; set; }
        public float Total { get; set; }
        public bool Abierto { get; set; }
    }
}
