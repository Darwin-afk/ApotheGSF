function RemoverDetalle(id) {
    $.ajax({
        async: true,
        data: $('#form').serialize() + "&RemoverId=" + id,
        type: "GET",
        url: '/Facturas/RemoverMedicamento',
        success: function (partialView) {
            $('#ListaDetalle').html(partialView);
        }
    });
};