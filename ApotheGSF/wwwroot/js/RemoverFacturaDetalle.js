function RemoverDetalle(id) {
    $.ajax({
        async: true,
        data: $('#form').serialize() + "&RemoverId=" + id,
        type: "POST",
        url: '/Facturas/RemoverMedicamento',
        success: function (resultado) {
            $('#ListaDetalle').html(resultado.partial);

            document.getElementById('SubTotal').value = resultado.subtotal;
            document.getElementById('itbis').value = 0.00;
            document.getElementById('Total').value = resultado.subtotal * 1.18;
        }
    });
};