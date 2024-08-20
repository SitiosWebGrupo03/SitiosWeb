document.addEventListener('DOMContentLoaded', () => {
    const { jsPDF } = window.jspdf;

    document.querySelector('.download-btn').addEventListener('click', () => {
        const selectedRows = document.querySelectorAll('.select-row:checked');

        if (selectedRows.length === 0) {

            downloadFullTable();
        } else {

            selectedRows.forEach(row => {
                const rowId = row.getAttribute('data-id');
                const rowElement = row.closest('tr');
                downloadSingleRow(rowElement);
            });
        }
    });

    function downloadFullTable() {
        html2canvas(document.querySelector('.records-table')).then(canvas => {
            const pdf = new jsPDF();
            const imgData = canvas.toDataURL('image/png');
            pdf.addImage(imgData, 'PNG', 10, 10);
            pdf.save('reporteincapacidades.pdf');
        });
    }

    function downloadSingleRow(rowElement) {
        html2canvas(rowElement).then(canvas => {
            const pdf = new jsPDF();
            const imgData = canvas.toDataURL('image/png');
            pdf.addImage(imgData, 'PNG', 10, 10);
            pdf.save(`incapacidad${rowElement.querySelector('td').textContent}.pdf`);
        });
    }
});
