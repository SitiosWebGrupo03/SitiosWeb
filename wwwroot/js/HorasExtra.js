function updateDateTime() {
    const now = new Date();
    const date = now.toLocaleDateString('es-ES');
    const time = now.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' }); // Formato de hora en español

    document.getElementById('current-date').textContent = date;
    document.getElementById('current-time').textContent = time;
}


document.addEventListener('DOMContentLoaded', updateDateTime);


setInterval(updateDateTime, 60000);