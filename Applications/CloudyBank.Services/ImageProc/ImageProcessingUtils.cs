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
using System.Reflection;

namespace CloudyBank.Services.ImageProc
{
    public static class ImageProcessingUtils
    {
        public static Image<Gray, byte> EqualizeHist(Image<Gray, byte> input)
        {
            Image<Gray, byte> output = new Image<Gray, byte>(input.Width, input.Height);

            CvInvoke.cvEqualizeHist(input.Ptr, output.Ptr);

            return output;
        }



        public static Bitmap ConvertToBitmap(int[] pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < pixels.Length; i++)
            {
                int y = i / width;
                int x = i % height;
                bitmap.SetPixel(x, y, Color.FromArgb(pixels[i]));

            }

            return bitmap;
        }

        public static byte[] ConvertToByte(int[] pixels)
        {
            int len = pixels.Length * 4;
            byte[] result = new byte[len]; // ARGB
            Buffer.BlockCopy(pixels, 0, result, 0, len);
            return result;
        }


        public static Image<Gray, byte> DetectAndTrimFace(int[] pixels, Size initialSize, Size outputSize, String haarcascadePath)
        {
            var inBitmap = ConvertToBitmap(pixels, initialSize.Width, initialSize.Width);
            
            //for testing purposes I can the picture to a folder
            //inBitmap.Save(@"E:\data\phototest\received.bmp");

            var grayframe = new Image<Gray, byte>(inBitmap);

            var haar = new HaarCascade(haarcascadePath);
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

            var returnImage = grayframe.Copy(face.rect).Resize(outputSize.Width, outputSize.Height, INTER.CV_INTER_CUBIC);

            //cleanup managed resources
            haar.Dispose();
            grayframe.Dispose();

            return returnImage;
        }

        public static byte[] ConvertBitmapToByteArray(Bitmap imageToConvert, ImageFormat formatOfImage)
        {
            byte[] returnArray;
            using (MemoryStream ms = new MemoryStream())
            {
                imageToConvert.Save(ms, formatOfImage);
                returnArray = ms.ToArray();
            }
            return returnArray;
        }

        private static Image<Gray, byte> ConvertToImage(byte[] pixels, int size)
        {
            using (MemoryStream stream = new MemoryStream(pixels, 0, pixels.Length))
            {
                Bitmap bitmap = new Bitmap(stream);
            }
            Image<Gray, byte> image = new Image<Gray, byte>(size, size);
            image.Bytes = pixels;
            return image;
        }

        private static String GetHaarCascadePath()
        {
            var dllLocation = Assembly.GetExecutingAssembly().Location;
            //var dllLocation = System.Reflection.Assembly.GetAssembly(typeof(ImageProcessingUtils)).Location;
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //var rinfo = assembly.GetManifestResourceInfo("haarcascade_forntalface_alt.xaml");
            return dllLocation + "\\ImageProc\\" +  "haarcascade_frontalface_alt.xml";
            
        }


        
    }
}
