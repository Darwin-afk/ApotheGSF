function AgregarDetalle() {
    $.ajax({
        data: $('#form').serialize() + "&MedicamentoId=@MedicamentoId&TipoCantidad=@TipoCantidad&Cantidad=@Cantidad",
        type: "POST",
        url: '/Facturas/AgregarMedicamento',
        success: function (resultado) {
            if (resultado.error == false) {
                $('#ListaDetalle').html(resultado.partial);

                document.getElementById('SubTotal').value = resultado.subtotal;
                document.getElementById('Total').value = resultado.subtotal * 1.18;
            }
        }
    });
};