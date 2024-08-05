

document.addEventListener('DOMContentLoaded', function () {
    var table = document.getElementById('horarioTable');
    var downloadButton = document.getElementById('downloadButton');

    // Muestra la tabla y el botón de descarga si hay más de una fila en el tbody
    if (table && table.querySelector('tbody').rows.length > 1) {
        table.classList.remove('hidden');
        downloadButton.classList.remove('hidden');
    }

    downloadButton.addEventListener('click', function () {
        var { jsPDF } = window.jspdf;
        var doc = new jsPDF();
        doc.text("Horarios", 10, 10);

        var rows = table.querySelectorAll('tbody tr');
        var x = 10, y = 20;

        rows.forEach(function (row, index) {
            var cells = row.querySelectorAll('td');
            var cellText = Array.from(cells).map(cell => cell.textContent).join(' | ');
            doc.text(cellText, x, y);
            y += 10;

            // Añade una nueva página si es necesario
            if (y > 270) {
                y = 20;
                doc.addPage();
            }
        });

        doc.save('horarios.pdf');
    });

    // Usa la variable 'puestos' según sea necesario
    console.log(puestos);
});