document.addEventListener("DOMContentLoaded", function () {
    let today = new Date();
    let currentMonth = today.getMonth(); // Mes actual (0-indexado)
    let currentYear = today.getFullYear(); // Año actual
    const daysContainer = document.getElementById('calendarDays');
    const acumuladas = document.getElementById('acumuladas');
    const textBlock = document.querySelector('.text-block');
    const solicitar = document.getElementById('Solicitar');
    const solicitudVP = document.getElementById('solicitudVP');

    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];


    const dates = []
    // Almacenar todos los días seleccionados
    let selectedDays = [];
    function getDatesBetween(startDate, endDate) {
        const currentDate = new Date(startDate);

        while (currentDate <= endDate) {
            const month = String(currentDate.getMonth() + 1); // Mes (01-12)
            const day = String(currentDate.getDate()).padStart(2, '0'); // Día (01-31)
            const year = currentDate.getFullYear(); // Año (yyyy)

            dates.push(`${month}/${day}/${year}`);
            currentDate.setDate(currentDate.getDate() + 1);
        }

        return dates;
    }


    function generateCalendar(month, year) {
        daysContainer.innerHTML = '';
        document.getElementById('monthYear').textContent = `${monthNames[month]} ${year}`;

        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const firstDay = new Date(year, month, 1).getDay();
        if (vcInicio !== undefined && vcInicio !== null)
        {
            for (let i = 0; i < vcInicio.length; i++) {
                getDatesBetween(new Date(vcInicio[i]), new Date(vcFin[i]));
            }
        }
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
            dayCell.classList.add(`month-${month}`); // Añadir clase para el mes

            const cellDate = new Date(year, month, day);
            const formattedDate = `${month + 1}/${day}/${year}`;

            if (cellDate < today) {
                dayCell.classList.add('disabled');
            }

            if (typeof dates !== 'undefined' && dates.includes(formattedDate)) {
                dayCell.classList.add('selected-vc');
                dayCell.classList.remove('disabled');
                dayCell.dataset.tooltip = `Vacaciones colectivas`;
            }

            // Lógica para día marcado (supuesto)
            if (typeof diaMarcar !== 'undefined' && diaMarcar.includes(formattedDate)) {
                let index = diaMarcar.indexOf(formattedDate);
                if (index !== -1) {
                    dayCell.classList.remove('disabled');
                    dayCell.classList.remove('selected-vc');
                    if (tipoDia[index] === "1") {
                        dayCell.classList.add('festivo');
                    } else {
                        dayCell.classList.add('configuracion');
                    }
                    dayCell.dataset.tooltip = `${DiaDescripcion[index]}`;
                }
            }
            
            if (diasPasados !== undefined && diasPasados.includes(formattedDate)) {
                dayCell.classList.remove('disabled');
                dayCell.classList.add('selected-vc');
                dayCell.dataset.tooltip = `Dia pendiente de vacaciones`;
            }
            if (selectedDays.includes(formattedDate)) {
                dayCell.classList.add('selected');
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



    daysContainer.addEventListener('click', function (event) {
   
        const dayCell = event.target;
        const acumulado = acumuladas.textContent.match(/\d+(\.\d+)?/)[0]; // Extract the full number, including decimals

        if (!dayCell.classList.contains('day') || dayCell.classList.contains('disabled') || dayCell.classList.contains('selected-vc') || dayCell.classList.contains('festivo') || dayCell.classList.contains('configuracion')) return;

       
        const dayNumber = parseInt(dayCell.textContent.trim());
        const formattedDate = `${currentMonth + 1}/${dayNumber}/${currentYear}`;

        if (selectedDays.includes(formattedDate)) {
            const index = selectedDays.indexOf(formattedDate);
            selectedDays.splice(index, 1);
            dayCell.classList.remove('selected');
            acumuladas.textContent = `Vacaciones acumuladas: ${parseFloat(acumulado, 10) + 1}`;
            const paragraphs = textBlock.getElementsByTagName('p');
            for (let i = 0; i < paragraphs.length; i++) {
                if (paragraphs[i].textContent === `${dayNumber} de ${monthNames[currentMonth + 1]}`) {
                    textBlock.removeChild(paragraphs[i]);
                    break;
                }
            }
        } else {
            if (parseFloat(acumulado, 10) - 1 < 0) {
                alert('No se permite elegir mas dias ');
                return
            };

            dayCell.classList.add('selected');
            selectedDays.push(formattedDate);
            acumuladas.textContent = `Vacaciones acumuladas: ${parseFloat(acumulado, 10) - 1}`;
            const pElement = document.createElement('p');
            pElement.textContent = `${dayNumber} de ${monthNames[currentMonth + 1]}`;
            textBlock.appendChild(pElement);
            solicitudVP.value = selectedDays.join(',');
        }


    });


    document.getElementById('prevMonth').addEventListener('click', prevMonth);
    document.getElementById('nextMonth').addEventListener('click', nextMonth);

    generateCalendar(currentMonth, currentYear);

});
