document.addEventListener('DOMContentLoaded', function () {
    // Selecciona el botón de Aceptar
    document.querySelector('.accept-btn').addEventListener('click', function () {
        // Obtiene todas las filas seleccionadas
        const selectedRows = document.querySelectorAll('input.select-row:checked');
        const data = [];

        console.log(Número de filas seleccionadas: ${ selectedRows.length }); // Depuración

        selectedRows.forEach(function (checkbox) {
            // Obtiene la fila asociada al checkbox
            const row = checkbox.closest('tr');
            if (row) {
                const rowData = {
                    id: checkbox.getAttribute('data-id'),
                    identificacion: row.cells[1].textContent.trim(),
                    cantidadDiasFuera: row.cells[2].textContent.trim(),
                    fechaInicio: row.cells[3].textContent.trim(),
                    fechaFinal: row.cells[4].textContent.trim(),
                    puesto: row.cells[5].textContent.trim(),
                    tipoIncapacidad: row.cells[6].textContent.trim()
                };

                console.log('Datos de la fila:', rowData); // Depuración
                data.push(rowData);
            }
        });

        // Verifica si hay datos para enviar
        if (data.length > 0) {
            fetch('/Aprob', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            }).then(response => {
                if (response.ok) {
                    alert('Datos enviados correctamente');
                } else {
                    alert('Error al enviar los datos');
                }
            }).catch(error => {
                console.error('Error:', error);
            });
        } else {
            alert('No se han seleccionado filas.');
        }
    });
});