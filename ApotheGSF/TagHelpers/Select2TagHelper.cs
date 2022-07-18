using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ApotheGSF.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-nombres, asp-valores, asp-valores-seleccionados, asp-id")]
    public class Select2TagHelper : TagHelper
    {
        [HtmlAttributeName("asp-nombres")]
        public IEnumerable<string> Nombres { get; set; }
        [HtmlAttributeName("asp-id")]
        public string Id { get; set; }
        [HtmlAttributeName("asp-valores")]
        public IEnumerable<int> Valores { get; set; }
        [HtmlAttributeName("asp-valores-seleccionados")]
        public IEnumerable<int>? ValoresSeleccionados { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string html = $@"<select class='form-control select2' multiple='multiple' id='{Id}' name='{Id}'>";
            string htmlClose = "</select>";
            string option = "";
            for (int i = 0; i < Nombres.Count(); i++)
            {
                if (ValoresSeleccionados != null)
                {
                    if (ValoresSeleccionados.Contains(i + 1))
                        option += $"<option value='{Valores.ElementAt(i)}' selected>{Nombres.ElementAt(i)}</option>";
                    else
                        option += $"<option value='{Valores.ElementAt(i)}'>{Nombres.ElementAt(i)}</option>";
                }
                else
                    option += $"<option value='{Valores.ElementAt(i)}'>{Nombres.ElementAt(i)}</option>";
            }
            output.Content.AppendHtml(html);
            output.Content.AppendHtml(option);
            output.Content.AppendHtml(htmlClose);
        }
    }
}
