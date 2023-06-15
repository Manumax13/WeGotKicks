function sendRequest(selectedValue, montoTotal) {
    fetch(`/Pago/PruebaApi?selectedValue=${selectedValue}&montoTotal=${montoTotal}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        // Manejar la respuesta del servidor
    })
    .catch(error => {
        // Manejar errores
    });
}