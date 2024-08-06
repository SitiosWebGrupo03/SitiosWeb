document.addEventListener('DOMContentLoaded', function () {
    const camera = document.getElementById('camera');
    const photo = document.getElementById('photo');
    const captureButton = document.getElementById('capturePhoto');
    const uploadInput = document.getElementById('uploadPhoto');
    const customUploadButton = document.getElementById('customUploadButton');
    const removeButton = document.getElementById('removePhoto');
    const removeLabel = document.getElementById('removepho');
    const photoData = document.getElementById('photoData');
    const form = document.getElementById('colaboradorForm');
    const btnGuardar = document.getElementById('btnGuardar');

    if (!camera || !photo || !captureButton || !uploadInput || !customUploadButton || !removeButton || !removeLabel || !photoData || !form || !btnGuardar) {
        console.error('Uno o más elementos DOM no se encuentran.');
        return;
    }

    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(stream => {
                camera.srcObject = stream;
            })
            .catch(error => {
                console.error('Error al acceder a la cámara:', error);
                alert('No se puede acceder a la cámara. Por favor, verifica los permisos.');
            });
    } else {
        console.warn('getUserMedia no es compatible con este navegador.');
        alert('Tu navegador no soporta acceso a la cámara.');
    }

    captureButton.addEventListener('click', function () {
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');
        canvas.width = camera.videoWidth;
        canvas.height = camera.videoHeight;
        context.drawImage(camera, 0, 0);

        const dataUrl = canvas.toDataURL('image/jpeg'); // Cambia a 'image/jpeg'
        photo.src = dataUrl;
        photo.style.display = 'block';
        removeButton.style.display = 'inline';
        removeLabel.style.display = 'inline';
        if (photoData) {
            photoData.value = dataUrl;
        }
    });

    customUploadButton.addEventListener('click', function () {
        uploadInput.click();
    });

    uploadInput.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const dataUrl = e.target.result;
                photo.src = dataUrl;
                photo.style.display = 'block';
                removeButton.style.display = 'inline';
                removeLabel.style.display = 'inline';
                if (photoData) {
                    photoData.value = dataUrl;
                }
            };
            reader.readAsDataURL(file);
        }
    });

    removeButton.addEventListener('click', function () {
        photo.src = '';
        photo.style.display = 'none';
        removeButton.style.display = 'none';
        removeLabel.style.display = 'none';
        if (photoData) {
            photoData.value = '';
        }
        uploadInput.value = '';
    });

    btnGuardar.addEventListener('click', function (event) {
        event.preventDefault();

        const formData = new FormData(form);
        const photoBase64 = photoData ? photoData.value : '';

        if (photoBase64) {
            formData.append('photoBase64', photoBase64);
        }

        fetch('/Asignar', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert(data.message);
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    });
});
