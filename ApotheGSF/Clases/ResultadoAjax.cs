using ApotheGSF.ViewModels;

namespace ApotheGSF.Clases
{
    public class ResultadoAjax
    {
        public bool error { get; set; }
        public string mensaje { get; set; }
        public string partial { get; set; }
        public FacturaViewModel viewModel { get; set; }
        public float subtotal { get; set; }
    }
}
