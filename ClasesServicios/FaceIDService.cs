using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System.Drawing; // Necesario para Bitmap

namespace Sitios_Version_2.servicesclass
{
    public class FaceIDService
    {
        private readonly Net _faceNet;
        private readonly string _knownImagePath = @"C:\Users\amaur\OneDrive\Escritorio\ASPW_2024-master\images\known_image.png";

        public FaceIDService()
        {
            // Usar una carpeta alternativa para evitar conflictos
            string modelDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceModels");
            string modelFile = Path.Combine(modelDirectory, "res10_300x300_ssd_iter_140000.caffemodel");
            string configFile = Path.Combine(modelDirectory, "deploy.prototxt");
            _faceNet = CvDnn.ReadNetFromCaffe(configFile, modelFile);
        }

        public string ProcessImage(JObject data)
        {
            try
            {
                string base64Image = data["image"].ToString().Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    using (var bitmap = new Bitmap(ms))
                    {
                        Mat image = bitmap.ToMat();

                        // Detectar rostros
                        var faceLocations = DetectFaces(image);

                        if (faceLocations.Length == 0)
                        {
                            return "No se detectaron rostros en la imagen";
                        }

                        var faceEncodings = faceLocations.Select(faceRect =>
                        {
                            return ExtractFaceFeatures(image, faceRect);
                        }).ToList();

                        // Obtener el encoding de la imagen conocida
                        var knownFaceEncoding = GetFaceEncoding(_knownImagePath);

                        // Comparar encodings faciales
                        foreach (var faceEncoding in faceEncodings)
                        {
                            var distance = Cv2.Norm(knownFaceEncoding, faceEncoding);
                            if (distance < 0.6)  // Umbral típico para considerar un rostro como coincidente
                            {
                                return "Rostro coincidente encontrado";
                            }
                        }

                        return "No coincide";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        private Mat ExtractFaceFeatures(Mat image, Rect faceRect)
        {
            Mat face = new Mat(image, faceRect);
            Mat blob = CvDnn.BlobFromImage(face, 1.0, new OpenCvSharp.Size(300, 300), new Scalar(104, 177, 123), false, false);
            _faceNet.SetInput(blob);
            Mat result = _faceNet.Forward();

            Mat faceFeatures = new Mat(1, result.Size(1), MatType.CV_32F);
            for (int i = 0; i < result.Size(1); i++)
            {
                faceFeatures.Set<float>(0, i, result.At<float>(0, i));
            }
            return faceFeatures;
        }

        private Rect[] DetectFaces(Mat image)
        {
            Mat inputBlob = CvDnn.BlobFromImage(image, 1.0, new OpenCvSharp.Size(300, 300), new Scalar(104, 177, 123), false, false);
            _faceNet.SetInput(inputBlob);
            Mat detection = _faceNet.Forward();

            var faces = new List<Rect>();

            for (int i = 0; i < detection.Size(2); i++)
            {
                float confidence = detection.At<float>(0, 0, i, 2);
                if (confidence > 0.5)
                {
                    int x1 = (int)(detection.At<float>(0, 0, i, 3) * image.Cols);
                    int y1 = (int)(detection.At<float>(0, 0, i, 4) * image.Rows);
                    int x2 = (int)(detection.At<float>(0, 0, i, 5) * image.Cols);
                    int y2 = (int)(detection.At<float>(0, 0, i, 6) * image.Rows);
                    Rect rect = new Rect(x1, y1, x2 - x1, y2 - y1);
                    faces.Add(rect);
                }
            }

            return faces.ToArray();
        }

        private Mat GetFaceEncoding(string imagePath)
        {
            Mat image = Cv2.ImRead(imagePath);
            var faceLocations = DetectFaces(image);
            if (faceLocations.Length == 0)
            {
                throw new Exception("No se detectaron rostros en la imagen conocida");
            }
            return ExtractFaceFeatures(image, faceLocations[0]);
        }
    }
}
