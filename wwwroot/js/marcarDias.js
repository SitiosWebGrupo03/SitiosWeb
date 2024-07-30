document.addEventListener("DOMContentLoaded", function () {
    const daysContainer = document.getElementById('calendarDays');
    const textBlock = document.querySelector('.text-block');
    const maxSelections = 5; // Maximum number of days that can be selected
    let remainingHours = parseInt(document.getElementById('horasRestantes').textContent.match(/\d+/)[0], 10);
    const selectedDays = []; // Array to store selected days
    
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
                const counterContainer = pElement.querySelector('.counter-container');
                const selectedHours = parseInt(counterContainer.querySelector('input').value, 10);
                textBlock.removeChild(pElement);
            }
        } else {
           
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

            // Create a new <p> element and counter
            const pElement = document.createElement('p');
            pElement.textContent = `${dayNumber} de ${document.getElementById('monthYear').textContent.replace(/\d+/g, '')}`;
            pElement.setAttribute('data-day', dayNumber);

            // Create the counter and append it to the pElement
            createCounter(pElement);

            textBlock.appendChild(pElement);
        }
        if selectedDays.length != 0) {
            this.getElementsByClassName('calendar').
        }
    });
});
