function AsignarPrecio() {
    document.getElementById('PrecioUnidad').value = (document.getElementById('Costo').value * 1.10).toFixed(0);

};