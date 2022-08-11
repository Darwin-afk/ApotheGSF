function AgregarDetalle() {
    $.ajax({
        data: $('#form').serialize() + "&MedicamentoId=@MedicamentoId&TipoCantidad=@TipoCantidad&Cantidad=@Cantidad",
        type: "POST",
        url: '/Facturas/AgregarMedicamento',
        success: function (resultado) {
            if (resultado.error == false) {
                $('#ListaDetalle').html(resultado.partial);

                document.getElementById('SubTotal').value = resultado.subtotal.toFixed(2);
                document.getElementById('Total').value = (resultado.subtotal * 1.18).toFixed(2);
                document.getElementById('itbis').value = ((resultado.subtotal * 1.18)* 0.18).toFixed(2);
            }
        }
    });
};