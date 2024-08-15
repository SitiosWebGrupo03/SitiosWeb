// calendarGenerator.js
document.addEventListener("DOMContentLoaded", function () {
    let today = new Date();
    let currentMonth = today.getMonth(); // Mes actual (0-indexed)
    let currentYear = today.getFullYear(); // Año actual
    const daysContainer = document.getElementById('calendarDays');

    let total = 0;

    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ]
    const errorLabel = document.querySelector('.error');
    function generateCalendar(month, year) {
        daysContainer.innerHTML = '';
        month = month + 1;
        document.getElementById('monthYear').textContent = `${monthNames[month - 1]} ${year}`;

        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const firstDay = new Date(year, month, 1).getDay();

        const dayLabels = ['D', 'L', 'M', 'M', 'J', 'V', 'S'];
        dayLabels.forEach(label => {
            const dayLabel = document.createElement('div');
            dayLabel.classList.add('gray-text');
            dayLabel.textContent = label;
            daysContainer.appendChild(dayLabel);
        });

        for (let i = 0; i < firstDay; i++) {
            const emptyCell = document.createElement('div');
            emptyCell.classList.add('gray-text');
            daysContainer.appendChild(emptyCell);
        }

        for (let day = 1; day <= daysInMonth; day++) {
            const dayCell = document.createElement('div');
            dayCell.textContent = day;
            dayCell.classList.add('day');

            const formattedDate = `${month}/${day}/${year}`;
            if (diaMarcar && diaMarcar.includes(formattedDate)) {
                let index = diaMarcar.indexOf(formattedDate);
                if (index !== -1) {
                    if (tipoDia[index] == "1") {
                        dayCell.classList.add('festivo');
                    } else {
                        dayCell.classList.add('configuracion');
                    }
                }

            }
            daysContainer.appendChild(dayCell);
        }
    }

    function prevMonth() {
        if (currentMonth === 0) {
            currentMonth = 11;
            currentYear--;
        } else {
            currentMonth--;
        }
        generateCalendar(currentMonth, currentYear);
    }

    function nextMonth() {
        if (currentMonth === 11) {
            currentMonth = 0;
            currentYear++;
        } else {
            currentMonth++;
        }
        generateCalendar(currentMonth, currentYear);
    }

    document.getElementById('prevMonth').addEventListener('click', prevMonth);
    document.getElementById('nextMonth').addEventListener('click', nextMonth);

    generateCalendar(currentMonth, currentYear);

    const config = document.getElementById('bloquear');
    const festivo = document.getElementById('festivo');
    const editar = document.getElementById('editar');
    const eliminar = document.getElementById('eliminar');
    const descripcion = document.getElementById('descripcion');
    const idDia = document.getElementById('idDia');
    const tipo = document.getElementById('tipo');
        const dia = document.getElementById('dia');


    function ocultarControles(opc)
    {
        switch (opc)
        {
            case 1:
                config.style.display = 'block';
                festivo.style.display = 'block';
                editar.style.display = 'none';
                eliminar.style.display = 'none';
                break;
            case 2:
                config.style.display = 'none';
                festivo.style.display = 'none';
                editar.style.display = 'block';
                eliminar.style.display = 'block';
                break;
        }
    }
    daysContainer.addEventListener('click', function (event) {
        const dayCell = event.target;
        const text = document.getElementById('monthYear').textContent;
        const month = text.replace(/\d+/g, '').trim(); 
        const monthIndex = monthNames.indexOf(month)+1; 
        const year = text.match(/\d+/g)[0]; 
        const day = dayCell.textContent.trim();
        const formattedDate = `${monthIndex}/${day}/${year}`;
        dia.value = formattedDate;
        if (dayCell.classList.contains('festivo') || dayCell.classList.contains('configuracion')) {
            if (diaMarcar && diaMarcar.includes(formattedDate)) {
                let index = diaMarcar.indexOf(formattedDate);
                idDia.value = idsDia[index];
                descripcion.textContent = DiaDescripcion[index];
            }
            ocultarControles(2)
        } else {
            idDia.textContent = '';
            descripcion.textContent = '';
            ocultarControles(1)
        }
       
        showModal();

    });
    config.addEventListener('click', event => {

        // Asignar el valor a los campos correspondientes
        tipo.value = 0;
        // Enviar el formulario
        document.getElementById('modalForm').submit();
    });
    festivo.addEventListener('click', event => {

        // Asignar el valor a los campos correspondientes
        tipo.value = 1;
        // Enviar el formulario
        document.getElementById('modalForm').submit();
    });
    editar.addEventListener('click', event => {

        // Asignar el valor a los campos correspondientes
        tipo.value = 2;
        // Enviar el formulario
        document.getElementById('modalForm').submit();
    });
    eliminar.addEventListener('click', event => {

        // Asignar el valor a los campos correspondientes
        tipo.value = 3;
        // Enviar el formulario
        document.getElementById('modalForm').submit();
    });
});
