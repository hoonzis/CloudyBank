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
using System.Windows.Controls.Primitives;

namespace CloudyBank.Web.Ria.UserControls
{
    public class DiscreteSlider : Slider
    {
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            int val = Convert.ToInt32(Math.Round(newValue));

            Thumb ElementHorizontalThumb = GetTemplateChild("HorizontalThumb") as Thumb;

            double maximum = base.Maximum;
            double minimum = base.Minimum;

            double num3 = val;
            double num4 = 1.0 - ((maximum - num3) / (maximum - minimum));

            RepeatButton ElementHorizontalLargeDecrease = GetTemplateChild("HorizontalTrackLargeChangeDecreaseRepeatButton") as RepeatButton;
            RepeatButton ElementHorizontalLargeIncrease = GetTemplateChild("HorizontalTrackLargeChangeIncreaseRepeatButton") as RepeatButton;

            Grid grid = GetTemplateChild("HorizontalTemplate") as Grid;

            if (grid != null)
            {

                if ((grid.ColumnDefinitions != null) && (grid.ColumnDefinitions.Count == 3))
                {
                    grid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                    grid.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);

                    if (ElementHorizontalLargeDecrease != null)
                    {
                        ElementHorizontalLargeDecrease.SetValue(Grid.ColumnProperty, 0);
                    }
                    if (ElementHorizontalLargeIncrease != null)
                    {
                        ElementHorizontalLargeIncrease.SetValue(Grid.ColumnProperty, 2);
                    }
                }
                if ((ElementHorizontalLargeDecrease != null) && (ElementHorizontalThumb != null))
                {
                    ElementHorizontalLargeDecrease.Width = Math.Max(0.0, num4 * (base.ActualWidth - ElementHorizontalThumb.ActualWidth));
                }
            }
        }
    }
}
