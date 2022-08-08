function FacturarEditar() {
    $.ajax({
        data: $('#form').serialize(),
        type: "POST",
        url: '/Facturas/Edit',
        success: function (r) {
            if (r.resultado) {
                var url = '../ReporteFactura?CodigoFactura=' + r.codigofactura;
                window.open(url, "_blank");
                window.location.href = "/Home/Index";
            }
            else {
                alert("invalido")
            }
        }
    });
};