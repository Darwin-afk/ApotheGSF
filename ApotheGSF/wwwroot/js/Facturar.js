function Facturar() {
    alert("Facturar");
    var modelo = $('#form').serialize();
    $.ajax({
        data: modelo,
        type: "POST",
        url: '/Facturas/Create',
        success: function (resultado) {
            if (resultado) {
                alert("valido");
                var url = '@Url.Action("ReporteFactura", "Facturas", new { factura = modelo })'
                alert(url);
                window.open(url, '_blank');
            }
            else {
                alert("invalido")
            }
        }
    });
};