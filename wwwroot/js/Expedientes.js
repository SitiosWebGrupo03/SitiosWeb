document.addEventListener('DOMContentLoaded', function () {
    const camera = document.getElementById('camera');
    const photo = document.getElementById('photo');
    const captureButton = document.getElementById('capturePhoto');
    const uploadInput = document.getElementById('uploadPhoto');
    const customUploadButton = document.getElementById('customUploadButton');
    const removeButton = document.getElementById('removePhoto');
    const removeLabel = document.getElementById('removepho');
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    const photoData = document.getElementById('photoData');
    const form = document.getElementById('colaboradorForm');
    const crearBtn = document.getElementById('btnCrear');

    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => {
            camera.srcObject = stream;
        })
        .catch(error => {
            console.error('Error al acceder a la cámara:', error);
        });

    captureButton.addEventListener('click', function () {
        canvas.width = camera.videoWidth;
        canvas.height = camera.videoHeight;
        context.drawImage(camera, 0, 0);
        const imageData = canvas.toDataURL('image/png');
        photo.src = imageData;
        photo.style.display = 'block';
        removeButton.style.display = 'inline';
        removeLabel.style.display = 'inline';  // Mostrar el label para eliminar la foto
        photoData.value = imageData;
    });

    // Al hacer clic en el botón personalizado, se activa el input de archivo
    customUploadButton.addEventListener('click', function () {
        uploadInput.click();
    });

    uploadInput.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                photo.src = e.target.result;
                photo.style.display = 'block';
                removeButton.style.display = 'inline';
                removeLabel.style.display = 'inline';  // Mostrar el label para eliminar la foto
                photoData.value = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

    removeButton.addEventListener('click', function () {
        photo.src = '';
        photo.style.display = 'none';
        removeButton.style.display = 'none';
        removeLabel.style.display = 'none';  // Ocultar el label de eliminar foto
        photoData.value = '';
        uploadInput.value = '';  // Limpiar el contenido del input de archivo
    });

    crearBtn.addEventListener('click', function (event) {
        event.preventDefault();

        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());

        fetch('/Asignar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Error en la solicitud');
            })
            .then(result => {
                console.log('Éxito:', result);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    });
});