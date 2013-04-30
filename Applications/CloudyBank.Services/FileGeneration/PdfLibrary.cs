using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CloudyBank.CoreDomain.Bank;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using CloudyBank.Services.Strings;

namespace CloudyBank.Services.FileGeneration
{
   public static class PdfLibrary
    {
        public static byte[] GetClientAccountList(ICollection<Account> accounts, String name, String code)
        {
            // Create a new PDF document
            using (PdfDocument document = new PdfDocument())
            {

                document.Info.Title = "Created with PDFsharp";

                // Create an empty page
                PdfPage page = document.AddPage();

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Create a font
                XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);


                gfx.DrawString(String.Format("Account list for: {0} ({1})", name, code), font, XBrushes.Black,
                      new XRect(20, 20, page.Width, page.Height),
                      XStringFormats.TopLeft);

                // Create a font
                XFont font2 = new XFont("Verdana", 12, XFontStyle.BoldItalic);
                int i = 0;
                foreach (var account in accounts)
                {
                    // Draw the text
                    gfx.DrawString(account.Name + " : " + account.Balance, font2, XBrushes.Black,
                      new XRect(20, i * 20 + 100, page.Width, page.Height),
                      XStringFormats.TopLeft);
                    i++;
                }

                byte[] data = ConvertPDFToBytes(document);
                return data;
            }

        }

        private static byte[] ConvertPDFToBytes(PdfDocument document)
        {
            using (Stream stream = new MemoryStream())
            {
                //false says do not close stream - i will use it
                document.Save(stream, false);
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
        }
    }
}
