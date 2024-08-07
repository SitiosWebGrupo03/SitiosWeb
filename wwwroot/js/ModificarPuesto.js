function cargarPuesto() {
    var selectElement = document.getElementById('idPuesto');
    var selectedOption = selectElement.options[selectElement.selectedIndex];

    var idPuesto = selectElement.value;
    var nombre = selectedOption.getAttribute('data-nombre');
    var salario = selectedOption.getAttribute('data-salario');
    var idDepartamento = selectedOption.getAttribute('data-departamento');
    var estado = parseInt(selectedOption.getAttribute('data-estado'));

    console.log('ID Puesto:', idPuesto);
    console.log('Nombre:', nombre);
    console.log('Salario:', salario);
    console.log('ID Departamento:', idDepartamento);
    console.log('Estado:', estado);

    if (idPuesto) {
        document.getElementById('nombre').value = nombre;
        document.getElementById('salario').value = salario;
        document.getElementById('idDepartamento').value = idDepartamento;
        document.getElementById('idPuesto').value = idPuesto;

        document.getElementById('activo').checked = (estado === 1);
        document.getElementById('inactivo').checked = (estado === 2);
    } else {
        document.getElementById('idPuesto').value = '';
        document.getElementById('nombre').value = '';
        document.getElementById('salario').value = '';
        document.getElementById('idDepartamento').value = '';
        document.getElementById('activo').checked = false;
        document.getElementById('inactivo').checked = false;
    }
}