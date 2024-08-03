using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using CvSize = OpenCvSharp.Size;

namespace SitiosWeb.ServicesClass
{
    public class FaceIDService
    {

        private Net _faceNet;
        private readonly string _knownImagesFolderPath = @"wwwroot\imageFaceID\";
        private Mat _knownFaceEncoding;

        public FaceIDService()
        {
            string configFile = @"wwwroot\FaceModels\deploy.prototxt";
            string modelFile = @"wwwroot\FaceModels\res10_300x300_ssd_iter_140000.caffemodel";
            _faceNet = CvDnn.ReadNetFromCaffe(configFile, modelFile);

            if (_faceNet.Empty())
            {
                throw new Exception("El modelo de detección de rostros no se cargó correctamente.");
            }
        }

        public string ProcessImage(JObject data)
        {
            try
            {
                if (data == null || !data.ContainsKey("image"))
                {
                    return "No se recibió una imagen.";
                }

                string base64Image = data["image"].ToString();
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
                        const double threshold = 0.17; // Ajusta este valor basado en pruebas
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


                _faceNet.SetInput(blob);
                Mat result = _faceNet.Forward();

                if (result.Empty() || result.Dims < 2)
                {
                    throw new Exception("El resultado del modelo está vacío o tiene dimensiones incorrectas.");
                }

                // Ajusta las dimensiones del resultado
                return result.Reshape(1, 1).Clone();
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

            _faceNet.SetInput(inputBlob);
            Mat detection = _faceNet.Forward();

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
            // No convertir a escala de grises aquí
            // Mat grayFace = new Mat();
            // Cv2.CvtColor(face, grayFace, ColorConversionCodes.BGR2GRAY);

            // Usar el modelo DNN directamente con la imagen en color
            string configFile = @"wwwroot\FaceModels\deploy.prototxt";
            string modelFile = @"wwwroot\FaceModels\res10_300x300_ssd_iter_140000.caffemodel";
            var faceNet = CvDnn.ReadNetFromCaffe(configFile, modelFile);

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

                    Rect rect = new Rect((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1));
                    faces.Add(rect);
                }
            }

            if (faces.Count == 0)
            {
                return face.Clone();
            }

            // Tomar el primer rostro detectado
            Rect faceRect = faces[0];

            // Extraer el rostro de la imagen
            Mat alignedFace = new Mat(face, faceRect);

            // Redimensionar el rostro para que tenga un tamaño estándar
            Mat resizedFace = new Mat();
            Cv2.Resize(alignedFace, resizedFace, new CvSize(300, 300));

            return resizedFace;
        }


        private double CompareFaceFeatures(Mat knownEncoding, Mat unknownEncoding)
        {
            if (knownEncoding.Empty() || unknownEncoding.Empty())
            {
                throw new ArgumentException("Los encodings no pueden estar vacíos.");
            }

            if (knownEncoding.Rows != unknownEncoding.Rows || knownEncoding.Cols != unknownEncoding.Cols)
            {
                throw new ArgumentException("Los encodings deben tener el mismo tamaño.");
            }

            // Normalizar los vectores
            Cv2.Normalize(knownEncoding, knownEncoding);
            Cv2.Normalize(unknownEncoding, unknownEncoding);

            // Calcular el producto punto
            Mat dotProduct = new Mat();
            Cv2.Multiply(knownEncoding, unknownEncoding, dotProduct);
            Scalar sumDotProduct = Cv2.Sum(dotProduct);

            // Calcular la magnitud de los vectores
            double normKnown = Cv2.Norm(knownEncoding);
            double normUnknown = Cv2.Norm(unknownEncoding);

            if (normKnown == 0 || normUnknown == 0)
            {
                return double.MaxValue; // Evita la división por cero
            }

            // Calcular la similitud del coseno
            double cosineSimilarity = sumDotProduct.Val0 / (normKnown * normUnknown);

            // Convertir similitud del coseno a distancia
            double cosineDistance = 1 - cosineSimilarity;

            return cosineDistance;
        }



    }
}