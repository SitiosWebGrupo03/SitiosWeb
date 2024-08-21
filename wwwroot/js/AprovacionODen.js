function enviarDatos(estado) {
    const selectedRows = document.querySelectorAll('input.select-row:checked');


    if (selectedRows.length === 0) {
        alert('No se han seleccionado filas.');
        return;
    }

    // Obtener los campos ocultos
    const hiddenIdEmpleado = document.getElementById('hiddenIdEmpleado');
    const hiddenDiasHorasFuera = document.getElementById('hiddenDiasHorasFuera');
    const hiddenFechaInicio = document.getElementById('hiddenFechaInicio');
    const hiddenFechaFin = document.getElementById('hiddenFechaFin');
    const hiddenPuestoLaboral = document.getElementById('hiddenPuestoLaboral');
    const hiddenIdTipoPermiso = document.getElementById('hiddenIdTipoPermiso');
    const hiddenEstado = document.getElementById('hiddenEstado');


    hiddenIdEmpleado.value = '';
    hiddenDiasHorasFuera.value = '';
    hiddenFechaInicio.value = '';
    hiddenFechaFin.value = '';
    hiddenPuestoLaboral.value = '';
    hiddenIdTipoPermiso.value = '';
    hiddenEstado.value = estado;

    selectedRows.forEach((checkbox, index) => {
        const id = checkbox.getAttribute('data-id');
        const diasHorasFuera = checkbox.getAttribute('data-diasHorasFuera');
        const idTipoPermiso = checkbox.getAttribute('data-idTipoPermiso');
        const puestoLaboral = checkbox.getAttribute('data-puestoLaboral');
        const fechaInicio = checkbox.getAttribute('data-fechaInicio');
        const fechaFin = checkbox.getAttribute('data-fechaFin');

        // Agregar valores a los campos ocultos
        hiddenIdEmpleado.value += id + (index < selectedRows.length - 1 ? ',' : '');
        hiddenDiasHorasFuera.value += diasHorasFuera + (index < selectedRows.length - 1 ? ',' : '');
        hiddenFechaInicio.value += fechaInicio + (index < selectedRows.length - 1 ? ',' : '');
        hiddenFechaFin.value += fechaFin + (index < selectedRows.length - 1 ? ',' : '');
        hiddenPuestoLaboral.value += puestoLaboral + (index < selectedRows.length - 1 ? ',' : '');
        hiddenIdTipoPermiso.value += idTipoPermiso + (index < selectedRows.length - 1 ? ',' : '');
    });

    // Enviar el formulario
    document.getElementById('formAprobacion').submit();
}