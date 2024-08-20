document.addEventListener('DOMContentLoaded', function () {
    const searchBtn = document.getElementById('search-btn');
    const searchInput = document.getElementById('identificacion');

    searchBtn.addEventListener('click', function () {
        const searchValue = searchInput.value.trim().toLowerCase();
        const rows = document.querySelectorAll('table tbody tr');
        let found = false;

        rows.forEach(row => {
            const idEmpleado = row.cells[1].textContent.trim().toLowerCase();
            if (idEmpleado.includes(searchValue) || searchValue === '') {
                row.style.display = '';
                found = true;
            } else {
                row.style.display = 'none';
            }
        });

        if (!found && searchValue !== '') {
            alert('No se encontraron resultados para la identificación proporcionada.');
        }
    });

    document.querySelector('.accept-btn').addEventListener('click', function () {
        enviarDatos(1);
    });

    document.querySelector('.cancel-btn').addEventListener('click', function () {
        enviarDatos(2);
    });
});

function enviarDatos(estado) {
    const selectedRows = document.querySelectorAll('input.select-row:checked');
    const data = [];

    selectedRows.forEach(function (checkbox) {
        const row = checkbox.closest('tr');
        const rowData = {
            IdEmpleado: row.querySelector('input.select-row').getAttribute('data-id'),
            DOH: true,
            DiasHorasFuera: parseInt(row.cells[2].textContent.trim()) || 0,
            IdTipoPermiso: parseInt(row.cells[6].textContent.trim()) || null,
            PuestoLaboral: row.cells[5].textContent.trim(),
            FechaInicio: row.cells[3].textContent.trim(),
            FechaFin: row.cells[4].textContent.trim(),
            Estado: estado
        };

        data.push(rowData);
    });

    if (data.length > 0) {
        fetch('/Aprob', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (response.ok) {
                    // Acción si la solicitud fue exitosa
                    console.error('Datos enviados exitosamente.');
                    window.location.reload();
                } else {
                    // Manejo de errores
                    console.error('Error al enviar los datos.');
                }
            })
            .catch(error => {
                console.error('Error:', error);

            });
    } else {
        alert('No se han seleccionado filas.');
    }
}
