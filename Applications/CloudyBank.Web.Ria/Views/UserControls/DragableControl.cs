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

namespace CloudyBank.Web.Ria.UserControls
{
    public class DragableControl : UserControl
    {
        private bool _isMouseDown;
        private Point _lastPosition;
        int zIndex = 0;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            BringToFront();
            this.CaptureMouse();
            _lastPosition = e.GetPosition(this.Parent as Canvas);
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point currentPosition = e.GetPosition(this.Parent as Canvas);
                Canvas.SetLeft(this, currentPosition.X);
                Canvas.SetTop(this, currentPosition.Y);
                _lastPosition = currentPosition;
                UpdateLayout();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            zIndex = 0;
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected void BringToFront()
        {
            if (zIndex == 0)
            {
                var oldIndex = this.zIndex;
                var mainCanvas = this.Parent as Canvas;
                if (mainCanvas != null)
                {
                    foreach (FrameworkElement fElement in mainCanvas.Children)
                    {
                        Canvas.SetZIndex(fElement, 0);
                    }
                    Canvas.SetZIndex(this, 2);
                    zIndex = 1;
                }
            }
        }
    }
}
