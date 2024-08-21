using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using CvSize = OpenCvSharp.Size;

namespace SitiosWeb.ServicesClass
{
    public class FaceIDService
    {
        private readonly CascadeClassifier _faceCascade;
        private readonly string _knownImagesFolderPath = @"wwwroot\imageFaceID\";
        private readonly string _configFile = @"wwwroot\FaceModels\deploy.prototxt";
        private readonly string _modelFile = @"wwwroot\FaceModels\res10_300x300_ssd_iter_140000.caffemodel";

        public FaceIDService()
        {
            string faceCascadeFile = @"wwwroot\FaceModels\haarcascade_frontalface_default.xml";
            _faceCascade = new CascadeClassifier(faceCascadeFile);

            if (_faceCascade.Empty())
            {
                throw new Exception("El modelo de detección de rostros no se cargó correctamente.");
            }
        }

        public string ProcessImage(string base64Image)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                {
                    return "No se recibió una imagen.";
                }

                // Eliminar el encabezado "data:image/jpeg;base64," si está presente
                string base64Data = base64Image.Contains(',') ? base64Image.Split(',')[1] : base64Image;

                byte[] imageBytes = Convert.FromBase64String(base64Data);

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    using (var bitmap = new Bitmap(ms))
                    {
                        Mat image = bitmap.ToMat();

                        if (image.Empty())
                        {
                            return "La imagen no se pudo convertir a Mat.";
                        }

                        // Normalización de la iluminación
                        NormalizeLighting(ref image);

                        // Detectar rostros
                        var faceLocations = DetectFaces(image);

                        if (faceLocations.Length == 0)
                        {
                            return "No se detectaron rostros en la imagen.";
                        }

                        var faceEncodings = faceLocations.Select(faceRect =>
                        {
                            return ExtractFaceFeatures(image, faceRect);
                        }).ToList();

                        if (faceEncodings.Count == 0)
                        {
                            return "No se pudieron extraer características faciales.";
                        }

                        // Comparar encodings faciales con todas las imágenes conocidas
                        const double threshold = 9.99; // Ajusta este valor basado en pruebas
                        foreach (var file in Directory.GetFiles(_knownImagesFolderPath))
                        {
                            var knownEncoding = GetFaceEncoding(file);
                            foreach (var faceEncoding in faceEncodings)
                            {
                                if (faceEncoding.Empty() || faceEncoding.Dims < 2)
                                {
                                    continue; // Saltar encodings inválidos
                                }

                                var distance = CompareFaceFeatures(knownEncoding, faceEncoding);
                                if (distance < threshold)
                                {
                                    return Path.GetFileNameWithoutExtension(file);
                                }
                            }
                        }

                        return "No hay coincidencia de rostros.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        private void NormalizeLighting(ref Mat image)
        {
            Mat grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            // Aplicar CLAHE para mejorar el contraste
            var clahe = Cv2.CreateCLAHE(clipLimit: 2.0, tileGridSize: new OpenCvSharp.Size(8, 8));
            Mat equalizedImage = new Mat();
            clahe.Apply(grayImage, equalizedImage);

            Cv2.CvtColor(equalizedImage, image, ColorConversionCodes.GRAY2BGR);
        }

        private Mat ExtractFaceFeatures(Mat image, Rect faceRect)
        {
            using (Mat face = new Mat(image, faceRect))
            {
                // Alinea el rostro
                Mat alignedFace = AlignFace(face);

                Mat resizedFace = new Mat();
                Cv2.Resize(alignedFace, resizedFace, new CvSize(300, 300)); // Ajusta según el modelo de características faciales

                Mat blob = CvDnn.BlobFromImage(resizedFace, 1.0, new CvSize(300, 300), new Scalar(104, 177, 123), true, false);

                if (blob.Empty())
                {
                    throw new Exception("El blob creado a partir de la imagen redimensionada está vacío.");
                }

                using (var faceNet = CvDnn.ReadNetFromCaffe(_configFile, _modelFile))
                {
                    if (faceNet.Empty())
                    {
                        throw new Exception("El modelo FaceNet no se cargó correctamente.");
                    }

                    faceNet.SetInput(blob);
                    Mat result = faceNet.Forward();

                    if (result.Empty() || result.Dims < 2)
                    {
                        throw new Exception("El resultado del modelo está vacío o tiene dimensiones incorrectas.");
                    }

                    // Ajusta las dimensiones del resultado
                    return result.Reshape(1, 1).Clone();
                }
            }
        }

        private Rect[] DetectFaces(Mat image)
        {
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2RGB);

            Mat resizedImage = new Mat();
            Cv2.Resize(image, resizedImage, new CvSize(300, 300));

            Mat inputBlob = CvDnn.BlobFromImage(resizedImage, 1.0, new CvSize(300, 300), new Scalar(104, 177, 123), true, false);

            if (inputBlob.Empty() || inputBlob.Dims != 4 || inputBlob.Size(1) != 3 || inputBlob.Size(2) != 300 || inputBlob.Size(3) != 300)
            {
                throw new Exception("El blob de entrada tiene dimensiones incorrectas.");
            }

            using (var faceNet = CvDnn.ReadNetFromCaffe(_configFile, _modelFile))
            {
                if (faceNet.Empty())
                {
                    throw new Exception("El modelo FaceNet no se cargó correctamente.");
                }

                faceNet.SetInput(inputBlob);
                Mat detection = faceNet.Forward();

                if (detection.Empty() || detection.Dims < 4)
                {
                    throw new Exception("La salida del modelo de detección está vacía o tiene dimensiones incorrectas.");
                }

                var faces = new List<Rect>();
                int numDetections = detection.Size(2);

                for (int i = 0; i < numDetections; i++)
                {
                    float confidence = detection.At<float>(0, 0, i, 2);
                    if (confidence > 0.7)
                    {
                        float x1 = detection.At<float>(0, 0, i, 3);
                        float y1 = detection.At<float>(0, 0, i, 4);
                        float x2 = detection.At<float>(0, 0, i, 5);
                        float y2 = detection.At<float>(0, 0, i, 6);

                        int x1Pixel = (int)(x1 * image.Cols);
                        int y1Pixel = (int)(y1 * image.Rows);
                        int x2Pixel = (int)(x2 * image.Cols);
                        int y2Pixel = (int)(y2 * image.Rows);

                        Rect rect = new Rect(x1Pixel, y1Pixel, x2Pixel - x1Pixel, y2Pixel - y1Pixel);
                        faces.Add(rect);
                    }
                }

                return faces.ToArray();
            }
        }

        private Mat GetFaceEncoding(string imagePath)
        {
            Mat image = Cv2.ImRead(imagePath);
            if (image.Empty())
            {
                throw new Exception("No se pudo cargar la imagen conocida.");
            }

            Mat resizedImage = new Mat();
            Cv2.Resize(image, resizedImage, new CvSize(300, 300));

            var faceLocations = DetectFaces(resizedImage);
            if (faceLocations.Length == 0)
            {
                throw new Exception("No se detectaron rostros en la imagen conocida");
            }

            return ExtractFaceFeatures(resizedImage, faceLocations[0]);
        }

        private Mat AlignFace(Mat face)
        {
            using (var faceNet = CvDnn.ReadNetFromCaffe(_configFile, _modelFile))
            {
                if (faceNet.Empty())
                {
                    throw new Exception("El modelo FaceNet no se cargó correctamente.");
                }

                // Crear un blob de la imagen en color
                Mat inputBlob = CvDnn.BlobFromImage(face, 1.0, new CvSize(300, 300), new Scalar(104, 177, 123), true, false);
                faceNet.SetInput(inputBlob);

                Mat detection = faceNet.Forward();
                var faces = new List<Rect>();

                for (int i = 0; i < detection.Size(2); i++)
                {
                    float confidence = detection.At<float>(0, 0, i, 2);
                    if (confidence > 0.7)
                    {
                        float x1 = detection.At<float>(0, 0, i, 3) * face.Cols;
                        float y1 = detection.At<float>(0, 0, i, 4) * face.Rows;
                        float x2 = detection.At<float>(0, 0, i, 5) * face.Cols;
                        float y2 = detection.At<float>(0, 0, i, 6) * face.Rows;

                        faces.Add(new Rect((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1)));
                    }
                }

                if (faces.Count == 0)
                {
                    throw new Exception("No se detectaron rostros en la imagen.");
                }

                return face;
            }
        }

        private double CompareFaceFeatures(Mat knownFaceEncoding, Mat faceEncoding)
        {
            // Usar la distancia euclidiana o cualquier otro método para comparar características faciales
            if (knownFaceEncoding.Empty() || faceEncoding.Empty())
            {
                return double.MaxValue;
            }

            return Cv2.Norm(knownFaceEncoding, faceEncoding, NormTypes.L2);
        }
    }
}