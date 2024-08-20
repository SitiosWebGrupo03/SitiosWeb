document.addEventListener("DOMContentLoaded", function () {
    let today = new Date();
    let currentMonth = today.getMonth(); // Mes actual (0-indexado)
    let currentYear = today.getFullYear(); // Año actual
    const daysContainer = document.getElementById('calendarDays');
    const calendar = document.getElementById('calendar');
    const textBlock = document.querySelector('.text-block');
    const solicitar = document.getElementById('Solicitar');
    const inicio = document.getElementById('diaInicio');
    const fin = document.getElementById('diaFinal');

    let start = null;
    let end = null;
    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];

    let startDay = null;
    let startMonth = null;
    let endDay = null;
    let endMonth = null;
    const dates = []
    // Almacenar todos los días seleccionados
    let selectedDays = [];
    function getDatesBetween(startDate, endDate) {
        const currentDate = new Date(startDate);

        while (currentDate <= endDate) {
            const month = String(currentDate.getMonth() + 1); // Mes (01-12)
            const day = String(currentDate.getDate()); // Día (01-31)
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

    function resetSelection() {
        selectedDays = [];
        const selectedElements = document.querySelectorAll('.day.selected');
        selectedElements.forEach(day => day.classList.remove('selected'));
        textBlock.innerHTML = '';
        startDay = null;
        startMonth = null;
        endDay = null;
        endMonth = null;
    }

    daysContainer.addEventListener('click', function (event) {
        if (marcarVC) { 
        const dayCell = event.target;

        if (!dayCell.classList.contains('day') || dayCell.classList.contains('disabled') || dayCell.classList.contains('festivo') || dayCell.classList.contains('configuracion')) return;

        const dayNumber = parseInt(dayCell.textContent.trim());
        const formattedDate = `${currentMonth + 1}/${dayNumber}/${currentYear}`;

        if (startDay === null) {
            resetSelection();
            startDay = dayNumber;
            startMonth = currentMonth;
            dayCell.classList.add('selected');
            selectedDays = [formattedDate];
            calendar.style.marginLeft = ('135px');
            solicitar.style.display = 'none';
            start = `${startMonth + 1}/${startDay}/${currentYear}`;
        } else if (endDay === null) {
            endDay = dayNumber;
            endMonth = currentMonth;

            if (endMonth < startMonth || (endMonth === startMonth && endDay < startDay)) {
                [startDay, endDay] = [endDay, startDay];
                [startMonth, endMonth] = [endMonth, startMonth];
            }
            end = `${endMonth + 1}/${endDay}/${currentYear}`;
            let excludedDays = [];
            let hasCollectiveHolidays = false;

            for (let i = startMonth; i <= endMonth; i++) {
                const daysInMonth = new Date(currentYear, i + 1, 0).getDate();
                const dayStart = (i === startMonth) ? startDay : 1;
                const dayEnd = (i === endMonth) ? endDay : daysInMonth;

                for (let j = dayStart; j <= dayEnd; j++) {
                    const cell = [...daysContainer.children].find(
                        el => el.textContent == j && el.classList.contains('day') && el.classList.contains(`month-${i}`)
                    );
                    if (cell) {
                        if (cell.classList.contains('festivo') || cell.classList.contains('configuracion')) {
                            excludedDays.push(`${j} de ${monthNames[i]}`);
                        } else {
                            cell.classList.add('selected');
                            selectedDays.push(`${i + 1}/${j}/${currentYear}`);
                        }

                        // Verificar si el rango incluye vacaciones colectivas
                        if (cell.classList.contains('selected-vc')) {
                            hasCollectiveHolidays = true;
                        }
                    }
                }
            }

            if (hasCollectiveHolidays) {
                resetSelection();
                return;
            }

            const pElement = document.createElement('p');
            const label = document.createElement('label');
            label.textContent = 'Días a solicitar:';
            label.classList.add('font-semibold');

            if (startMonth === endMonth) {
                pElement.textContent = `Del ${startDay} al ${endDay} de ${monthNames[startMonth]} ${currentYear}`;
            } else {
                pElement.textContent = `Del ${startDay} de ${monthNames[startMonth]} al ${endDay} de ${monthNames[endMonth]} ${currentYear}`;
            }

            const excludedElement = document.createElement('label');
            const p = document.createElement('p');

            excludedElement.classList.add('font-semibold');
            if (excludedDays.length > 0) {
                excludedElement.textContent = `Días excluidos:`;
                p.textContent = excludedDays.join(', ');
            } else {
                excludedElement.textContent = 'No hay días excluidos.';
            }
            calendar.style.marginLeft = ('30px');
            solicitar.style.display = 'block';

            textBlock.appendChild(label);
            textBlock.appendChild(pElement);
            textBlock.appendChild(excludedElement);
            textBlock.appendChild(p);

            startDay = null;
            startMonth = null;
            endDay = null;
            endMonth = null;
            }
        }
        });


    document.getElementById('prevMonth').addEventListener('click', prevMonth);
    document.getElementById('nextMonth').addEventListener('click', nextMonth);

    generateCalendar(currentMonth, currentYear);
    solicitar.addEventListener('click', function () {
        inicio.value = start;
        inicio.textContent = start;
        fin.value = end;
        fin.textContent = end;
        document.getElementById('solicitudVC').submit();
    });
});
