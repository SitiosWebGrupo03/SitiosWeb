using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
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
                        const double threshold = 10.0; // Ajusta este valor basado en pruebas
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


            Mat filteredImage = new Mat();
            Cv2.BilateralFilter(grayImage, filteredImage, 15, 75, 75);


            Mat equalizedImage = new Mat();
            Cv2.EqualizeHist(filteredImage, equalizedImage);

            Cv2.CvtColor(equalizedImage, image, ColorConversionCodes.GRAY2BGR);
        }

        private Mat ExtractFaceFeatures(Mat image, Rect faceRect)
        {
            using (Mat face = new Mat(image, faceRect))
            {
                Mat alignedFace = AlignFace(face);

                // Redimensionar la imagen
                Mat resizedFace = new Mat();
                Cv2.Resize(alignedFace, resizedFace, new CvSize(300, 300));

                // Crear el blob
                Mat blob = CvDnn.BlobFromImage(resizedFace, 1.0, new CvSize(300, 300), new Scalar(104, 177, 123), true, false);

                if (blob.Empty() || blob.Total() == 0)
                {
                    throw new Exception("El blob de la cara está vacío o tiene tamaño total cero.");
                }

                _faceNet.SetInput(blob);
                Mat result = _faceNet.Forward();

                if (result.Empty() || result.Dims < 2)
                {
                    throw new Exception("La salida del modelo está vacía o tiene dimensiones incorrectas.");
                }

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
            Mat grayFace = new Mat();
            Cv2.CvtColor(face, grayFace, ColorConversionCodes.BGR2GRAY);

            string eyesCascadeFile = @"wwwroot\FaceModels\haarcascade_eye.xml";
            var eyesCascade = new CascadeClassifier(eyesCascadeFile);

            var eyes = eyesCascade.DetectMultiScale(grayFace, scaleFactor: 1.1, minNeighbors: 5, minSize: new CvSize(30, 30));

            if (eyes.Length < 2)
            {
                return face.Clone();
            }

            Rect leftEye = eyes[0];
            Rect rightEye = eyes[1];

            OpenCvSharp.Point leftEyeCenter = new OpenCvSharp.Point(leftEye.X + leftEye.Width / 2, leftEye.Y + leftEye.Height / 2);
            OpenCvSharp.Point rightEyeCenter = new OpenCvSharp.Point(rightEye.X + rightEye.Width / 2, rightEye.Y + rightEye.Height / 2);

            double deltaX = rightEyeCenter.X - leftEyeCenter.X;
            double deltaY = rightEyeCenter.Y - leftEyeCenter.Y;
            double angle = Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI;

            Mat rotationMatrix = Cv2.GetRotationMatrix2D(leftEyeCenter, angle, 1.0);

            Mat rotatedFace = new Mat();
            Cv2.WarpAffine(face, rotatedFace, rotationMatrix, face.Size(), InterpolationFlags.Linear, BorderTypes.Reflect101);

            return rotatedFace;
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

            Mat diff = knownEncoding - unknownEncoding;
            double distance = Cv2.Norm(diff, NormTypes.L2);
            return distance;
        }
    }
}
