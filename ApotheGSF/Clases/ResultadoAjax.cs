using Microsoft.AspNetCore.Mvc;

namespace ApotheGSF.Clases
{
    public class ResultadoAjax
    {
        public bool error { get; set; }
        public string mensaje { get; set; }
        public string partial { get; set; }
    }
}
