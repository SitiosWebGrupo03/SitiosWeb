document.querySelectorAll('.edit-button').forEach(button => {
    button.addEventListener('click', () => {
        const row = button.closest('tr');
        const currentlyEditableRow = document.querySelector('.editable');


        if (currentlyEditableRow && currentlyEditableRow !== row) {
            currentlyEditableRow.classList.remove('editable');
            currentlyEditableRow.querySelectorAll('input').forEach(input => {
                input.readOnly = true;
            });
        }


        row.classList.toggle('editable');
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
    const editableRow = document.querySelector('.editable');
    if (editableRow) {
        const data = {};
        editableRow.querySelectorAll('input').forEach(input => {
            data[input.name] = input.value;
        });

        fetch('/EditarMarcas', {
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
                    editableRow.classList.remove('editable');
                    editableRow.querySelectorAll('input').forEach(input => {
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
