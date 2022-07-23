function AgregarDetalle() {
    $.ajax({
        async: true,
        data: $('#form').serialize() + "&MedicamentoId=@MedicamentoId&TipoCantidad=@TipoCantidad&Cantidad=@Cantidad",
        type: "POST",
        url: '/Facturas/AgregarMedicamento',
        success: function (partialView) {
            $('#ListaDetalle').html(partialView);
        }
    });
};