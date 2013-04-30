using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CloudyBank.Web.Ria.Components
{
    /// <summary>
    /// Control usefull for taking images and visualizing using web camera. Contains ImageCaptured event,
    /// which is fired everytime a picture is taken.
    /// </summary>
    public partial class ImageTaker : UserControl
    {
        CaptureSource _captureSource;
        public event EventHandler<ImageCapturedArgs> ImageCaptured;

        
        public ImageTaker()
        {
            InitializeComponent();
        }

        void _captureSource_CaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
        {
            //local copy of the event handler - in case someone decides to unsubscribe just after the null check
            var eventCopy = ImageCaptured;
            
            if (eventCopy!= null)
            {
                eventCopy(this, new ImageCapturedArgs { Bitmap = e.Result });
            }
        }

        private void TakeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_captureSource != null && _captureSource.State == CaptureState.Started)
            {
                _captureSource.CaptureImageAsync();
            }
        }

        public void ReleaseCaptureSource()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (_captureSource != null && _captureSource.State == CaptureState.Started)
                {
                    _captureSource.Stop();
                }
            });
        }

        public void StartWebCam()
        {
            _captureSource = new CaptureSource();
            _captureSource.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(_captureSource_CaptureImageCompleted);
            _captureSource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

            try
            {
                // Start capturing
                if (_captureSource.State != CaptureState.Started)
                {
                    // Create video brush and fill the WebcamVideo rectangle with it
                    var vidBrush = new VideoBrush();
                    vidBrush.Stretch = Stretch.Uniform;
                    vidBrush.SetSource(_captureSource);
                    WebcamVideo.Fill = vidBrush;
                    
                    // Ask user for permission and start the capturing
                    if (CaptureDeviceConfiguration.RequestDeviceAccess())
                    {
                        _captureSource.Start();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                InfoTextBox.Text = "Web Cam already started - if not, I can't find it...";
            }
            catch (Exception)
            {
                InfoTextBox.Text = "Could not start web cam, do you have one?";
            }
        }

        private void StartCameraButton_Click(object sender, RoutedEventArgs e)
        {
            StartWebCam();
        }

    }
}
