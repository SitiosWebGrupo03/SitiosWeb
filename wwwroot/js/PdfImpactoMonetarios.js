async function descargarArchivo() {
    const { jsPDF } = window.jspdf;


    const nombre = document.getElementById('nombre').value;
    const salario = document.getElementById('salario').value;
    const empresa = document.getElementById('empresa').value;
    const ins = document.getElementById('ins').value;


    if (!nombre|| !salario ||!empresa || !ins)
    {
        alert('No se puede crear el PDF porque faltan datos.');
        return;
    }


    const doc = new jsPDF();


    doc.text('Impacto Monetario de la Incapacidad', 10, 10);
    doc.text(Nombre: ${ nombre }, 10, 20);
    doc.text(Salario: ${ salario }, 10, 30);
    doc.text(Porcentaje pagado por la empresa: ${ empresa }, 10, 40);
    doc.text(Porcentaje pagado por la CCSS: ${ ins }, 10, 50);


    doc.save('impacto_monetario.pdf');
}