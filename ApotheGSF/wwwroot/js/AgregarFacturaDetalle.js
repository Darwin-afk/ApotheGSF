$(document).ready(function () {
    $('#AgregarMedicamento').click(function () {
        $.ajax({
            async: true,
            data: $('#form').serialize(),
            type: "POST",
            url: '/Facturas/AgregarMedicamento',
            success: function (partialView) {
                $('#ListaDetalle').html(partialView);
            }
        });
    });
});