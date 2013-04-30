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
using System.IO;

namespace CloudyBank.Web.Ria.Technical
{
    public class FDService : IOService
    {

        public string OpenFileDialog(string defaultPath)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            if (fd.File != null)
            {
                return fd.File.FullName;
            }
            return null;
        }

        public System.IO.Stream OpenFile()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            if (fd.File != null)
            {
                return fd.File.OpenRead();
            }
            return null;
        }

        public FileInfo OpenFileInfo()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            return fd.File;
        }
    }
}
