const video = document.getElementById('video');
const captureBtn = document.getElementById('captureBtn');
const statusDiv = document.getElementById('status');
const dateDiv = document.querySelector('.date h3');
const entryTimeDiv = document.querySelector('.entry-time p');
const exitTimeDiv = document.querySelector('.exit-time p');

async function initCamera() {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        video.srcObject = stream;
    } catch (err) {
        console.error('Error al acceder a la cámara: ', err);
        statusDiv.innerText = 'Error al acceder a la cámara: ' + err.message;
    }
}

function updateTime() {
    const now = new Date();
    const dateString = now.toLocaleDateString('es-ES');
    const timeString = now.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });

    dateDiv.innerText = Fecha: ${ dateString };
    entryTimeDiv.innerText = timeString;
    exitTimeDiv.innerText = timeString; // Actualiza con la misma hora por defecto
}

captureBtn.addEventListener('click', () => {
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const context = canvas.getContext('2d');
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    const dataURL = canvas.toDataURL('image/jpeg');
    fetch('/processImage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ image: dataURL })
    })
        .then(response => response.json())
        .then(data => {
            statusDiv.innerText = data.message;
        })
        .catch(err => {
            console.error('Error al procesar la imagen: ', err);
            statusDiv.innerText = 'Error al procesar la imagen.';
        });
});

// Inicializar la cámara y actualizar la hora al cargar el documento
document.addEventListener('DOMContentLoaded', () => {
    initCamera();
    updateTime();
});