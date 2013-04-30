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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CloudyBank.Web.Ria.Components
{
    /// <summary>
    /// Bindable event calendar. Provides a possibility to any IEnumerable containing events.
    /// A property which specifies the date of the item has to be set, in order to allow to control to lay out 
    /// the events.
    /// </summary>
    public partial class EventCalendar : UserControl
    {
        #region Events Exposed by component

        public event EventHandler<CalendarEventArgs> EventClick;
        public event EventHandler<CalendarEventArgs> DayClick;

        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty SelectedEventProperty = DependencyProperty.Register("SelectedEvent", typeof(Object), typeof(EventCalendar), null);
        public Object SelectedEvent
        {
            get { return (Object)GetValue(SelectedEventProperty); }
            set { SetValue(SelectedEventProperty, value); }
        }

        public static readonly DependencyProperty CalendarEventButtonStyleProperty = DependencyProperty.Register("CalendarEventButtonStyle", typeof(Style), typeof(EventCalendar), null);
        public Style CalendarEventButtonStyle
        {
            get { return (Style)GetValue(CalendarEventButtonStyleProperty); }
            set { SetValue(CalendarEventButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty DatePropertyNameProperty = DependencyProperty.Register("DatePropertyName", typeof(String), typeof(EventCalendar), null);
        public String DatePropertyName
        {
            get { return (String)GetValue(DatePropertyNameProperty); }
            set { SetValue(DatePropertyNameProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(EventCalendar),
            new PropertyMetadata(ItemsSourcePropertyChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        private List<CalendarDayButton> calendarButtons = new List<CalendarDayButton>();

        public EventCalendar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clear all the items in the Calendar
        /// </summary>
        public void ClearCallendar()
        {
            foreach (CalendarDayButton button in calendarButtons)
            {

                var panel = button.Parent as StackPanel;
                int nbControls = panel.Children.Count;

                for (int i = nbControls - 1; i > 0; i--)
                {
                    panel.Children.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Method called when the ItemsSource property is changed - when new list of items is assigned to be displayed in the calendar
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as EventCalendar;
            if (e.NewValue == null)
            {
                owner.ClearCallendar();
                return;
            }

            IEnumerable rawItems = (IEnumerable)e.NewValue;
            PropertyInfo property = null;

            var enumerator = rawItems.GetEnumerator();
            if(!enumerator.MoveNext()){
                owner.ClearCallendar();
                return;
            }

            Object o = enumerator.Current;
            Type type = o.GetType();
            
            property = type.GetProperty(owner.DatePropertyName);
            
            if (property != null)
            {
                IEnumerable<Object> items = Enumerable.Cast<Object>((IEnumerable)e.NewValue);
                if (items != null)
                {
                    var parDate = items
                                .GroupBy(x => GetDateValue(x, property))
                                .ToDictionary(x => x.Key, x => x.ToList());
                    owner.ItemsSourceDictionary = parDate;
                    owner.FillCalendar();
                }
            }
        }

        public static DateTime GetDateValue (Object x, PropertyInfo property)
        {
            return ((DateTime)property.GetValue(x,null)).Date;
        }

        private Dictionary<DateTime,List<Object>> _itemsSourceDictionary;
        public Dictionary<DateTime, List<Object>> ItemsSourceDictionary
        {
            get
            {
                return _itemsSourceDictionary;
            }

            set
            {
                _itemsSourceDictionary = value;
            }
        }

        private DateTime GetFirstCalendarDate()
        {
            return new DateTime(InnerCalendar.DisplayDate.Year, InnerCalendar.DisplayDate.Month, 1);
        }

        private void FillCalendar()
        {
            FillCalendar(GetFirstCalendarDate());
        }

        public DateTime GetDate(DateTime firstDate, CalendarDayButton button)
        {
            int weekDay = (int)firstDate.DayOfWeek;
            if (weekDay == 0) weekDay = 7;
            if (weekDay == 1) weekDay = 8;

            for (int counter = 0; counter < calendarButtons.Count; counter++)
            {
                if(button == calendarButtons[counter])
                {
                    return firstDate.AddDays(counter).AddDays(-weekDay);
                }
            }
            return DateTime.MinValue;
        }

        private void FillCalendar(DateTime firstDate)
        {
            if (ItemsSourceDictionary!=null && ItemsSourceDictionary.Count >0)
            {                
                DateTime currentDay;

                int weekDay = (int)firstDate.DayOfWeek;
                if (weekDay == 0) weekDay = 7;
                if (weekDay == 1) weekDay = 8;

                for (int counter = 0; counter < calendarButtons.Count;counter++)
                {
                    var button = calendarButtons[counter];
                    var panel = button.Parent as StackPanel;


                    int nbControls = panel.Children.Count;
                    for (int i = nbControls - 1; i > 0; i--)
                    {
                        panel.Children.RemoveAt(i);
                    }

                    currentDay = firstDate.AddDays(counter).AddDays(-weekDay);

                    if (ItemsSourceDictionary.ContainsKey(currentDay))
                    {
                        var events = ItemsSourceDictionary[currentDay];
                        foreach (Object calendarEvent in events)
                        {
                            Button btn = new Button();
                            btn.DataContext = calendarEvent;
                            btn.Style = CalendarEventButtonStyle;
                            panel.Children.Add(btn);
                            btn.Click += new RoutedEventHandler(EventButton_Click);
                        }
                    }
                }
            }
        }

        void EventButton_Click(object sender, RoutedEventArgs e)
        {
            object eventClicked = (sender as Button).DataContext as object;
            
            //set the selected event
            SelectedEvent = eventClicked;

            //just pass the click event to the hosting envirenment of the component
            if (EventClick != null)
            {
                EventClick(sender, new CalendarEventArgs(eventClicked));
            }
        }

        private void CalendarDayButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as CalendarDayButton;
            calendarButtons.Add(button);

            //Resizing the buttons is the only way to change the dimensions of the calendar
            button.Width = this.ActualWidth / 9;
            button.Height = this.ActualHeight / 8;

            if (calendarButtons.Count == 42)
            {
                FillCalendar();
            }
        }

        private void InnerCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            FillCalendar();
        }

        private void CalendarDayButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarDayButton button = sender as CalendarDayButton;
            DateTime date = GetDate(GetFirstCalendarDate(),button);

            if(date!=DateTime.MinValue && DayClick!=null)
            {
                DayClick(sender,new CalendarEventArgs(date));
            }
        }

    }
}
