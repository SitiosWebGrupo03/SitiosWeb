document.addEventListener('DOMContentLoaded', (event) => {
    // Obtén los elementos del DOM
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const captureBtn = document.getElementById('captureBtn');
    const photoField = document.getElementById('photoField');
    const form = document.getElementById('captureForm');

    // Configura el video para que capture desde la cámara
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => {
            video.srcObject = stream;
            video.play();
        })
        .catch(error => {
            console.error('Error accessing the camera: ', error);
        });

    captureBtn.addEventListener('click', function (event) {
        event.preventDefault(); // Evita el comportamiento por defecto del botón

        // Configura el canvas con las dimensiones del video
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        // Dibuja la imagen del video en el canvas
        const context = canvas.getContext('2d');
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        // Convierte el contenido del canvas a base64
        const photoBase64 = canvas.toDataURL('image/jpeg');

        // Asigna el valor base64 al campo oculto
        photoField.value = photoBase64;

        // Opcional: Agrega un mensaje de estado
        document.getElementById('status').innerText = 'Imagen capturada y lista para enviar.';

        // Envía el formulario
        form.submit();
    });
});
