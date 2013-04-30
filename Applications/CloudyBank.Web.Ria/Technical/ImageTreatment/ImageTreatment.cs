using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Linq;

namespace CloudyBank.Web.Ria.Technical.ImageTreatment
{
    public class ImageTreatment
    {
        public static WriteableBitmap ConvertToWB(byte[] image)
        {

            //var pixels = image.Select(x => x * 0x00010101).ToArray();
            var colors = image.Select(x => Color.FromArgb(255, x, x, x)).Select(x => ToArgb(x)).ToArray();


            WriteableBitmap wb = new WriteableBitmap(80, 80);

            //have to multiply the lenght by 4 - because each int has a length of 4 bytes.
            Buffer.BlockCopy(colors, 0, wb.Pixels, 0, colors.Length * 4);
            //wp.Pixels = pixels.ToArray();

            //wp.FromByteArray(image);
            return wb;
        }

        public static int ToArgb(Color color)
        {
            int argb = color.A << 24;
            argb += color.R << 16;
            argb += color.G << 8;
            argb += color.B;

            return argb;
        }
    }
}
