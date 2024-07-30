// calendarGenerator.js
document.addEventListener("DOMContentLoaded", function () {
    let today = new Date();
    let currentMonth = today.getMonth(); // Mes actual (0-indexed)
    let currentYear = today.getFullYear(); // Año actual
    const daysContainer = document.getElementById('calendarDays');
    const textBlock = document.querySelector('.text-block');
    const maxSelections = 5; // Maximum number of days that can be selected
    const selectedDays = []; // Array to store selected days
    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ]
    function generateCalendar(month, year) {
        daysContainer.innerHTML = '';

        ;
        document.getElementById('monthYear').textContent = `${monthNames[month]} ${year}`;

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

            const cellDate = new Date(year, month, day);
            const formattedDate = `${month}/${day}/${year}`;
            if (diaMarcar && diaMarcar.includes(formattedDate)) {
                dayCell.classList.add('selected');
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

        const incrementButton = document.createElement('button');
        incrementButton.textContent = '+';
        incrementButton.className = 'ContadorButtons';

        const inputField = document.createElement('input');
        inputField.type = 'text';
        inputField.value = 0;
        inputField.readOnly = true;
        inputField.className = 'contador';

        decrementButton.addEventListener('click', function () {
            const currentValue = parseInt(inputField.value, 10);
            if (currentValue > 0) {
                inputField.value = currentValue - 1;
                if (currentValue - 1 === 0) {
                    decrementButton.disabled = true;
                }
                incrementButton.disabled = false;
            }
        });

        incrementButton.addEventListener('click', function () {
            const currentValue = parseInt(inputField.value, 10);
            inputField.value = currentValue + 1;
            decrementButton.disabled = false;
        });

        counterContainer.appendChild(decrementButton);
        counterContainer.appendChild(inputField);
        counterContainer.appendChild(incrementButton);

        pElement.appendChild(counterContainer);
    }

    daysContainer.addEventListener('click', function (event) {
        const dayCell = event.target;

        if (!dayCell.classList.contains('day') || dayCell.classList.contains('disabled')) return;
        const dayNumber = dayCell.textContent.trim();
        const index = selectedDays.indexOf(dayNumber);

        if (index > -1) {
            // Day is already selected, so deselect it
            dayCell.classList.remove('selected');
            selectedDays.splice(index, 1);

            // Remove the corresponding <p> and counter from the text block
            const pElement = document.querySelector(`.text-block p[data-day="${dayNumber}"]`);
            if (pElement) {
                textBlock.removeChild(pElement);
            }
        } else {
            if (!Marcar) { return; }
            // Get the error label element
            const errorLabel = document.querySelector('.error');

            if (selectedDays.length >= maxSelections) {
                errorLabel.classList.remove('fadeOut'); // Remove fadeOut class if present
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
            } else {
                // Hide the error message
                errorLabel.style.display = 'none';
                errorLabel.classList.remove('shake', 'fadeOut');
            }

            // Select the day
            dayCell.classList.add('selected');
            selectedDays.push(dayNumber);
            const text = document.getElementById('monthYear').textContent;
            const month = text.replace(/\d+/g, '').trim(); // Extract month name
            const monthIndex = monthNames.indexOf(month); // Get zero-based index
            const year = text.match(/\d+/g)[0]; // Extract year
            diaMarcar.push(`${monthIndex}/${dayNumber}/${year}`);

            // Create a new <p> element and counter
            const pElement = document.createElement('p');
            pElement.textContent = `${dayNumber} de ${document.getElementById('monthYear').textContent.replace(/\d+/g, '')}`;
            pElement.setAttribute('data-day', dayNumber);

            // Create the counter and append it to the pElement
            createCounter(pElement);

            textBlock.appendChild(pElement);
        }

        const calendar = document.getElementById('calendar');
        const submitButton = document.getElementById('Solicitar');
        if (selectedDays.length !== 0) {
            if (calendar) {
                calendar.style.marginLeft = '';
                submitButton.style.display = 'block';
            }
        } else {
            if (calendar) {
                calendar.style.marginLeft = '135px';
                submitButton.style.display = 'none';
            }
        }
    });

});
