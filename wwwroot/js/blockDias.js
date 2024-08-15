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


    

    daysContainer.addEventListener('click', function (event) {
        const dayCell = event.target;
        const text = document.getElementById('monthYear').textContent;
        const month = text.replace(/\d+/g, '').trim(); // Extract month name
        const monthIndex = monthNames.indexOf(month); // Get zero-based index
        const year = text.match(/\d+/g)[0]; // Extract year
        const dayNumber = dayCell.textContent.trim();
        if (dayCell.classList.contains('festivo')) {
            return;
        }
        if (dayCell.classList.contains('configuracion')) {
                return;
        }
        showModal();
        
       
        
        



    });

});
