function RemoverDetalle(id) {
    $.ajax({
        async: true,
        data: $('#form').serialize() + "&RemoverId=id",
        type: "POST",
        url: '/Facturas/RemoverMedicamento',
        success: function (partialView) {
            $('#ListaDetalle').html(partialView);
        }
    });
};