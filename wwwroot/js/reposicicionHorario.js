// calendarGenerator.js
document.addEventListener("DOMContentLoaded", function () {
    let today = new Date();
    let currentMonth = today.getMonth(); // Mes actual (0-indexed)
    let currentYear = today.getFullYear(); // Año actual
    const daysContainer = document.getElementById('calendarDays');
    const textBlock = document.querySelector('.text-block');
    const maxSelections = 5; // Maximum number of days that can be selected
    const selectedDays = []; // Array to store selected days

    // Inicializa la variable Horas con un valor predeterminado (por ejemplo, 0)

    let total = 0;
    const dates = []
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

    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ]
    const errorLabel = document.querySelector('.error');
    function generateCalendar(month, year) {
        daysContainer.innerHTML = '';
        document.getElementById('monthYear').textContent = `${monthNames[month]} ${year}`;

        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const firstDay = new Date(year, month, 1).getDay();
        if (vcInicio !== undefined && vcInicio !== null) {
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

            const cellDate = new Date(year, month, day);
            const formattedDate = `${month + 1}/${day}/${year}`;
            if (diaMarcar && diaMarcar.includes(formattedDate)) {
                dayCell.classList.add('selected');
            }
            if (typeof dates !== 'undefined' && dates.includes(formattedDate)) {
                dayCell.classList.add('selected-vc');
                dayCell.classList.remove('disabled');
                dayCell.dataset.tooltip = `Vacaciones colectivas`;

            }
            if (typeof diasMarcar !== 'undefined' && diasMarcar.includes(formattedDate)) {
                let index = diasMarcar.indexOf(formattedDate);
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

            if (cellDate < today) {
                dayCell.classList.add('disabled');
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


    function createCounter(pElement) {
        const counterContainer = document.createElement('span');
        counterContainer.className = 'counter-container';

        const decrementButton = document.createElement('button');
        decrementButton.textContent = '-';
        decrementButton.className = 'ContadorButtons';
        decrementButton.disabled = true;
        decrementButton.type = 'button';


        const incrementButton = document.createElement('button');
        incrementButton.textContent = '+';
        incrementButton.className = 'ContadorButtons';
        incrementButton.type = 'button';

        const inputField = document.createElement('input');
        inputField.type = 'text';
        inputField.value = 0;
        inputField.readOnly = true;
        inputField.className = 'contador';

        decrementButton.addEventListener('click', function () {
            const currentValue = parseInt(inputField.value, 10);
            if (currentValue > 0) {
                total = total - 1;
                inputField.value = currentValue - 1;
                incrementButton.disabled = false;

                if (currentValue - 1 === 0) {
                    decrementButton.disabled = true;
                }
            }
        });

        incrementButton.addEventListener('click', function () {
            const currentValue = parseInt(inputField.value, 10);
            if (total < Horas) {
                total = total + 1;
                inputField.value = currentValue + 1;
                if (total === Horas) {
                    incrementButton.disabled = true;
                }
                decrementButton.disabled = false;
            }
        });

        counterContainer.appendChild(decrementButton);
        counterContainer.appendChild(inputField);
        counterContainer.appendChild(incrementButton);

        pElement.appendChild(counterContainer);
    }

    daysContainer.addEventListener('click', function (event) {
        const dayCell = event.target;
        const text = document.getElementById('monthYear').textContent;
        const month = text.replace(/\d+/g, '').trim(); // Extract month name
        const monthIndex = monthNames.indexOf(month); // Get zero-based index
        const year = text.match(/\d+/g)[0]; // Extract year

        if (!dayCell.classList.contains('day') || dayCell.classList.contains('disabled') || dayCell.classList.contains('selected-vc') || dayCell.classList.contains('festivo') || dayCell.classList.contains('configuracion')) return;
        const dayNumber = dayCell.textContent.trim();
        const index = selectedDays.indexOf(dayNumber);
        if (index > -1) {
            // Day is already selected, so deselect it
            dayCell.classList.remove('selected');
            selectedDays.splice(index, 1);
            diaMarcar = diaMarcar.filter(item => item !== `${monthIndex}/${dayNumber}/${year}`);
            const pElement = document.querySelector(`.text-block p[data-day="${dayNumber}"]`);
            if (pElement) {
                const counterValue = parseInt(pElement.querySelector('input.contador').value, 10);
                total -= counterValue; // Adjust the total based on the counter value
                textBlock.removeChild(pElement); // Remove the p element from textBlock
            }
        } else {
            if (!Marcar) { return; }
            // Get the error label element


            if (selectedDays.length >= maxSelections) {

                errorLabelShow('Periodo de 5 dias para realizar la reposición excedido.');
                return;
            } else {
                // Hide the error message
                errorLabel.style.display = 'none';
                errorLabel.classList.remove('shake', 'fadeOut');
            }

            // Select the day
            dayCell.classList.add('selected');
            selectedDays.push(dayNumber);

            diaMarcar.push(`${monthIndex + 1}/${dayNumber}/${year}`);
            // Create a new <p> element and counter
            const pElement = document.createElement('p');
            pElement.textContent = `${dayNumber} de ${document.getElementById('monthYear').textContent.replace(/\d+/g, '')}`;
            pElement.setAttribute('data-day', dayNumber);

            // Create the counter and append it to the pElement
            createCounter(pElement);

            textBlock.appendChild(pElement);
        }
        function errorLabelShow(text) {
            errorLabel.classList.remove('fadeOut');
            errorLabel.textContent = text;
            errorLabel.style.display = 'block';
            errorLabel.classList.add('shake');

            setTimeout(() => {
                errorLabel.classList.remove('shake');
            }, 500); // Duration of the shake animation

            setTimeout(() => {
                errorLabel.classList.add('fadeOut');
                setTimeout(() => {
                    errorLabel.style.display = 'none';
                    errorLabel.classList.remove('fadeOut');
                }, 2000);
            }, 2000);
            return;
        }
        const calendar = document.getElementById('calendar');
        const submitButton = document.getElementById('Solicitar');
        const justificacionSelect = document.getElementById('justificacionSelect');
        const justificacionLbl = document.getElementById('justificacionLbl');
        if (selectedDays.length !== 0) {
            if (calendar) {
                calendar.style.marginLeft = '';
                submitButton.style.display = 'block';
                if (justificacionSelect) {
                    justificacionSelect.style.display = 'block';
                    justificacionLbl.style.display = 'block';
                }
            }
        } else {
            if (calendar) {
                calendar.style.marginLeft = '135px';
                submitButton.style.display = 'none';
                if (justificacionSelect) {
                    justificacionSelect.style.display = 'none';
                    justificacionLbl.style.display = 'none';
                }
            }
        }
        document.getElementById('Solicitar').addEventListener('click', event => {
            // Crear un array para almacenar las horas
            let horasArray = [];

            // Iterar sobre los elementos con la clase 'contador' y agregar sus valores al array
            Array.from(document.getElementsByClassName('contador')).forEach(function (input) {
                horasArray.push(input.value);
            });
            let sumaHoras = horasArray.reduce((total, valor) => total + valor, 0);
            if (sumaHoras == 0) {
                errorLabelShow('Seleccione al menos una hora para realizar la reposición.');
                return;
            }
            // Convertir el array a una cadena separada por comas
            let horasString = horasArray.join(',');

            // Asignar el valor a los campos correspondientes
            document.getElementById('diasReposicion').value = diaMarcar;
            document.getElementById('horasReposicion').value = horasString;

            // Enviar el formulario
            document.getElementById('counterForm').submit();
        });
    });

});
