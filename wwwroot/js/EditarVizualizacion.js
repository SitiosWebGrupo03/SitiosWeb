document.querySelectorAll('.Editar-button').forEach(button => {
    button.addEventListener('click', () => {
        const row = button.closest('tr');
        const currentlyEditarableRow = document.querySelector('.Editarable');


        if (currentlyEditarableRow && currentlyEditarableRow !== row) {
            currentlyEditarableRow.classList.remove('Editarable');
            currentlyEditarableRow.querySelectorAll('input').forEach(input => {
                input.readOnly = true;
            });
        }


        row.classList.toggle('Editarable');
        row.querySelectorAll('input').forEach(input => {
            input.readOnly = !input.readOnly;
            if (!input.readOnly) {
                input.focus();
            }
        });
    });
});

document.getElementById('buscarButton').addEventListener('click', () => {
    const cedula = document.getElementById('buscarCedula').value.toLowerCase();
    const rows = document.querySelectorAll('#marcasTable tbody tr');
    rows.forEach(row => {
        const idEmpleado = row.cells[1].querySelector('input').value.toLowerCase();
        if (idEmpleado.includes(cedula)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});

document.getElementById('saveButton').addEventListener('click', () => {
    const EditarableRow = document.querySelector('.Editarable');
    if (EditarableRow) {
        const data = {};
        EditarableRow.querySelectorAll('input').forEach(input => {
            data[input.name] = input.value;
        });

        fetch('/EditararMarcas', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(result => {
                if (result.success) {
                    alert('Marca actualizada correctamente');
                    EditarableRow.classList.remove('Editarable');
                    EditarableRow.querySelectorAll('input').forEach(input => {
                        input.readOnly = true;
                    });
                } else {
                    alert('Error al actualizar la marca');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Error al actualizar la marca');
            });
    } else {
        alert('No hay ninguna fila en modo de ediciÃ³n.');
    }
});
