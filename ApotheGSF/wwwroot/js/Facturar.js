function Facturar() {
    $.ajax({
        data: $('#form').serialize(),
        type: "POST",
        url: '/Facturas/Create',
        success: function (r) {
            if (r.resultado) {
                var url = 'ReporteFactura?CodigoFactura=' + r.codigofactura;
                window.open(url, "_blank");
                window.location.href = "/Home/Index?Mensaje=" + r.mensaje;
            }
        }
    });
};