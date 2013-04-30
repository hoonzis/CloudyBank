using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Emgu.CV.CvEnum;

namespace CloudyBank.Services.ImageProcessing
{
    public class ImageProcessingService
    {
        public static Image<Gray, byte> EqualizeHist(Image<Gray, byte> input)
        {
            Image<Gray, byte> output = new Image<Gray, byte>(input.Width, input.Height);

            CvInvoke.cvEqualizeHist(input.Ptr, output.Ptr);

            return output;
        }

        private static Bitmap ConvertToBitmap(int[] pixels, int size)
        {
            Bitmap bitmap = new Bitmap(size, size);

            for (int i = 0; i < pixels.Length; i++)
            {
                int y = i / size;
                int x = i % size;
                bitmap.SetPixel(x, y, Color.FromArgb(pixels[i]));

            }

            //bitmap.Save(@"e:\test\bitmap.jpg");
            return bitmap;
        }

        public static Image<Gray, byte> DetectAndTrimFace(int[] pixels, int initialSize, int outputSize)
        {
            var inBitmap = ConvertToBitmap(pixels, initialSize);
            //inBitmap.Save(@"E:\data\phototest\received.bmp");
            var grayframe = new Image<Gray, byte>(inBitmap);

            var haar = new HaarCascade(GetHaarCascade());
            var faces = haar.Detect(grayframe,
                1.2,
                3,
                HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(30, 30));


            if (faces.Count() != 1)
            {
                return null;
            }
            var face = faces[0];

            var returnImage = grayframe.Copy(face.rect).Resize(outputSize, outputSize, INTER.CV_INTER_CUBIC);

            return returnImage;
        }

        private static byte[] ConvertBitmapToByteArray(Bitmap imageToConvert, ImageFormat formatOfImage)
        {
            byte[] Ret;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageToConvert.Save(ms, formatOfImage);
                    Ret = ms.ToArray();
                }
            }
            catch (Exception) { throw; }
            return Ret;
        }

        private static Image<Gray, byte> ConvertToImage(byte[] pixels, int size)
        {
            MemoryStream stream = new MemoryStream(pixels, 0, pixels.Length);
            Bitmap bitmap = new Bitmap(stream);
            Image<Gray, byte> image = new Image<Gray, byte>(size, size);
            image.Bytes = pixels;

            return image;
        }

        private static String GetHaarCascade()
        {
            throw new NotImplementedException();
        }
    }
}
