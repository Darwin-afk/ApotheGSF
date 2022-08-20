function AgregarDetalle() {
    $.ajax({
        data: $('#form').serialize() + "&MedicamentoId=@MedicamentoId&LaboratorioId=@LaboratorioId&TipoCantidad=@TipoCantidad&Cantidad=@Cantidad",
        type: "POST",
        url: '/Facturas/AgregarMedicamento',
        success: function (resultado) {
            if (resultado.error == false) {
                $('#ListaDetalle').html(resultado.partial);

                document.getElementById('SubTotal').value = resultado.subtotal.toFixed(2);
                document.getElementById('Total').value = (resultado.subtotal * 1.18).toFixed(2);
                document.getElementById('itbis').value = (resultado.subtotal * 0.18).toFixed(2);

                alert("prueba 1");

                var selector = $('#MedicamentoId').selectize().data('selectize');
                selector.setValue('', false);

                var selector2 = $('#LaboratorioId').selectize().data('selectize');
                selector2.setValue('', false);
                selector2.clearOptions();

                var selector3 = $('#TipoCantidad').selectize().data('selectize');
                selector3.setValue('1', false);

                document.getElementById('Cantidad').value = 0;
            }
        }
    });
};