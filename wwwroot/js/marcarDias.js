document.addEventListener("DOMContentLoaded", function() {
    const daysContainer = document.getElementById('calendarDays');
    const textBlock = document.querySelector('.text-block');
    const maxSelections = 5; // Maximum number of days that can be selected
    let remainingHours = parseInt(document.getElementById('horasRestantes').textContent.match(/\d+/)[0], 10);
    const selectedDays = []; // Array to store selected days

    function updateDropdownOptions() {
        // Update the dropdown options based on remaining hours
        const selects = textBlock.querySelectorAll('select');
        selects.forEach(select => {
            const currentValue = select.value;
            // Build the options based on remaining hours
            const options = Array.from({ length: remainingHours }, (_, i) => i + 1).map(hour =>
                `<option value="${hour}" ${hour == currentValue ? 'selected' : ''}>${hour} hora${hour > 1 ? 's' : ''}</option>`
            ).join('');
            
            // If no hours are remaining, show a single option indicating no options available
            if (remainingHours <= 0)
                return;
            
            // Update the select element
            select.innerHTML = `
                <option value="" disabled selected hidden>Cant. Horas</option>
                ${options}
            `;
            
        });
    }

    daysContainer.addEventListener('click', function(event) {
        const dayCell = event.target;

        if (!dayCell.classList.contains('day') || dayCell.classList.contains('disabled')) return;
        if (remainingHours <= 0) {alert('No hay horas disponibles para seleccionar.'); return;}
        const dayNumber = dayCell.textContent.trim();
        const index = selectedDays.indexOf(dayNumber);

        if (index > -1) {
            // Day is already selected, so deselect it
            dayCell.classList.remove('selected');
            selectedDays.splice(index, 1);

            // Remove the corresponding <p> and dropdown from the text block
            const pElement = document.querySelector(`.text-block p[data-day="${dayNumber}"]`);
            if (pElement) {
                const selectElement = pElement.querySelector('select');
                const selectedHours = parseInt(selectElement.value, 10);
                textBlock.removeChild(pElement);

                // Update remaining hours
                remainingHours += selectedHours;
                updateDropdownOptions();
            }
        } else {
            // Day is not selected, so check if the maximum number of selections is reached
            if (selectedDays.length >= maxSelections) {
                alert(`El máximo de días para reposiciones son ${maxSelections} días.`);
                return;
            }

            // Select the day
            dayCell.classList.add('selected');
            selectedDays.push(dayNumber);

            // Create a new <p> element and dropdown
            const pElement = document.createElement('p');
            pElement.textContent = `${dayNumber} de ${document.getElementById('monthYear').textContent.replace(/\d+/g, '')}`;
            pElement.setAttribute('data-day', dayNumber);

            // Create a dropdown for hours
            const selectElement = document.createElement('select');
            selectElement.innerHTML = `
                <option value="" disabled selected hidden>Cant. Horas</option>
                ${Array.from({ length: remainingHours }, (_, i) => i + 1).map(hour =>
                    `<option value="${hour}">${hour} hora${hour > 1 ? 's' : ''}</option>`
                ).join('')}
            `;
            selectElement.addEventListener('change', function() {
                const selectedHours = parseInt(this.value, 10);

                // Update remaining hours
                remainingHours -= selectedHours;

                // Disable the select after an option is chosen
                this.disabled = true;

                // Update dropdown options for all selects
                updateDropdownOptions();
            });

            pElement.appendChild(selectElement);
            textBlock.appendChild(pElement);
        }
    });

    // Update dropdown options on page load
    updateDropdownOptions();
});
