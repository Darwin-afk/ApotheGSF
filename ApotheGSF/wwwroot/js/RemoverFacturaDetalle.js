function RemoverDetalle(id) {
    $.ajax({
        async: true,
        data: $('#form').serialize() + "&RemoverId=" + id,
        type: "POST",
        url: '/Facturas/RemoverMedicamento',
        success: function (resultado) {
            $('#ListaDetalle').html(resultado.partial);

            document.getElementById('SubTotal').value = resultado.subtotal.toFixed(2);
            document.getElementById('Total').value = (resultado.subtotal * 1.18).toFixed(2);
            document.getElementById('itbis').value = (resultado.subtotal * 0.18).toFixed(2);
        }
    });
};