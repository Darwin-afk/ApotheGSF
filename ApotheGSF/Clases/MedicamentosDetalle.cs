namespace ApotheGSF.Clases
{
    public class MedicamentosDetalle
    {
        public int DetalleId { get; set; }
        public List<int> CajasId { get; set; }
        public string NombreMedicamento { get; set; }
        public int TipoCantidad { get; set; }
        public int Cantidad { get; set; }
        public float Precio { get; set; }
        public float Total { get; set; }
    }
}
