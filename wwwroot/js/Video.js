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

    dateDiv.innerText = `Fecha: ${dateString}`;
    entryTimeDiv.innerText = timeString;
    exitTimeDiv.innerText = timeString; // Actualiza con la misma hora por defecto
}

function isValidBase64(base64String) {
    // Verifica si la cadena tiene el formato que se ocupa
    const base64Regex = /^data:image\/(jpeg|png);base64,[a-zA-Z0-9+/=]+$/;
    return base64Regex.test(base64String);
}

captureBtn.addEventListener('click', () => {
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const context = canvas.getContext('2d');
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    const dataURL = canvas.toDataURL('image/jpeg');
    const imageParts = dataURL.split(',');

    // verifica que la cadena base64 tenga el formato que se ocupa
    if (imageParts.length !== 2 || !isValidBase64(dataURL)) {
        statusDiv.innerText = 'La cadena base64 de la imagen no es válida.';
        return;
    }

    const base64Data = imageParts[1]; // Extraer solo los datos base64

    fetch('/processImage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ image: base64Data })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Server Response:', data);
            statusDiv.innerText = data.message;
        })
        .catch(err => {
            console.error('Error al procesar la imagen: ', err);
            statusDiv.innerText = 'Error al procesar la imagen: ' + err.message;
        });
});

document.addEventListener('DOMContentLoaded', () => {
    initCamera();
    updateTime();
});