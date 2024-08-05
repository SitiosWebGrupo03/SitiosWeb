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

    // Acceso a la cámara
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(stream => {
            camera.srcObject = stream;
        })
        .catch(error => {
            console.error('Error al acceder a la cámara:', error);
        });

    // Capturar foto desde la cámara
    captureButton.addEventListener('click', function () {
        canvas.width = camera.videoWidth;
        canvas.height = camera.videoHeight;
        context.drawImage(camera, 0, 0);
        const imageData = canvas.toDataURL('image/png');
        photo.src = imageData;
        photo.style.display = 'block';
        removeButton.style.display = 'inline';
        removeLabel.style.display = 'inline';
        photoData.value = imageData;
    });

    // Botón personalizado para cargar imagen desde el sistema de archivos
    customUploadButton.addEventListener('click', function () {
        uploadInput.click();
    });

    // Cargar imagen desde el sistema de archivos
    uploadInput.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                photo.src = e.target.result;
                photo.style.display = 'block';
                removeButton.style.display = 'inline';
                removeLabel.style.display = 'inline';
                photoData.value = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

    // Eliminar imagen cargada o capturada
    removeButton.addEventListener('click', function () {
        photo.src = '';
        photo.style.display = 'none';
        removeButton.style.display = 'none';
        removeLabel.style.display = 'none';
        photoData.value = '';
        uploadInput.value = '';
    });

    // Enviar formulario y datos de imagen
    crearBtn.addEventListener('click', function (event) {
        event.preventDefault();

        const formData = new FormData(form);
        const data = {
            identificacion: formData.get('identificacion'),
            nombre: formData.get('nombre'),
            apellidos: formData.get('apellidos'),
            fechaNacimiento: formData.get('fecha-nacimiento'),
            fechaContratacion: formData.get('fecha-contratacion'),
            fechaFinContrato: formData.get('fecha-fin-contrato'),
            correo: formData.get('correo'),
            telefono: formData.get('telefono'),
            imagenDto: {
                ImagenBase64: photoData.value
            }
        };

        fetch('/Asignar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(result => {
                if (result.success) {
                    alert('Colaborador agregado exitosamente.');
                } else {
                    alert('Error al agregar el colaborador.');
                }
            });
    });
