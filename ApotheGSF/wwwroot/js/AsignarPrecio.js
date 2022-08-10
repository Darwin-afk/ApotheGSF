function AsignarPrecio() {
    var costo = document.getElementById('Costo').value;
    var unidades = document.getElementById('UnidadesCaja').value;

    if (costo > 0 && unidades > 0) {
        document.getElementById('PrecioUnidad').value = ((costo * 1.10) / unidades).toFixed(2);
    }
    else {
        document.getElementById('PrecioUnidad').value = 0.00
    }

};