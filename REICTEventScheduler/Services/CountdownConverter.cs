using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace REICTEventScheduler.Services
{
    /// <summary>
    /// Converts countdown seconds double value to string "HH : MM : SS"
    /// </summary>
    public class CountdownConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Double dbl = 0;
            try
            {
                dbl = Double.Parse(value.ToString());
            }
            catch (System.Exception) { }

            try
            {
                DateTime dateTime = DateTime.Parse(value.ToString());
                TimeSpan ts = dateTime - DateTime.Now;
                dbl = ts.TotalSeconds;
            }
            catch (System.Exception) { }

            var timespan = TimeSpan.FromSeconds(dbl);
            
            if (timespan.TotalSeconds < 1.0)
            {
                return "-- : --";
            }
            else if (timespan.TotalSeconds > 3600 * 24)
            {
                return string.Format("{0:D2} : {1:D2} : {2:D2} : {3:D2}",
                    timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
            }

            return string.Format("{0:D2} : {1:D2} : {2:D2}",
                timespan.Hours, timespan.Minutes, timespan.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (targetType == typeof(DateTime))
            {
                return DateTime.Now;
            }
            else if (targetType == typeof(Double))
            {
                return (double)value;
            }
            else
                return value;
        }
        #endregion
    }

    /// <summary>
    /// Converts countdown seconds double value to string "HH : MM : SS"
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string colorcode = "#6B8CAF";
            Color color = Color.FromHex(colorcode);

            if ((Color)value == Color.Red)
                return color;
            else
                return Color.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Countdown timer with periodical ticks.
    /// </summary>
    public class Countdown : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the start date time.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        /// <summary>
        /// Gets the remain time in seconds.
        /// </summary>
        public double RemainTime
        {
            get { return remainTime; }

            private set
            {
                remainTime = value;
                OnPropertyChanged();
            }
        }

        //public Color WarningBackColor
        //{
        //    get { return warningBackColor; }

        //    set
        //    {
        //        string colorcode = "#6B8CAF";
        //        Color color = Color.FromHex(colorcode);

        //        if (warningBackColor == Color.Red)
        //            warningBackColor = color;
        //        else
        //            warningBackColor = Color.Red;
        //        OnPropertyChanged();
        //    }
        //}

        public Color WarningTextColor
        {
            get { return warningTextColor; }

            set
            {
                Color color = Color.Black;

                if (warningTextColor == Color.White)
                    warningTextColor = color;
                else
                    warningTextColor = Color.White;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Occurs when completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// Occurs when ticked.
        /// </summary>
        public event Action Ticked;

        /// <summary>
        /// The timer.
        /// </summary>
        Timer timer;

        /// <summary>
        /// The remain time.
        /// </summary>
        double remainTime;

        //Color warningBackColor;
        Color warningTextColor;

        /// <summary>
        /// The remain time total.
        /// </summary>
        double remainTimeTotal;

        /// <summary>
        /// Starts the updating with specified period, total time and period are specified in seconds.
        /// </summary>
        public void StartUpdating(double total, double period = 1.0)
        {
            if (timer != null)
            {
                StopUpdating();
            }

            remainTimeTotal = total;
            RemainTime = total;

            StartDateTime = DateTime.Now;

            timer = new Timer(period * 1000);
            timer = new Timer();
            timer.Elapsed += (sender, e) => Tick();
            timer.Enabled = true;
        }

        /// <summary>
        /// Stops the updating.
        /// </summary>
        public void StopUpdating()
        {
            RemainTime = 0;
            remainTimeTotal = 0;

            if (timer != null)
            {
                timer.Enabled = false;
                timer = null;
            }
        }

        /// <summary>
        /// Updates the time remain.
        /// </summary>
        public void Tick()
        {
            var delta = (DateTime.Now - StartDateTime).TotalSeconds;

            if (delta < remainTimeTotal)
            {
                RemainTime = remainTimeTotal - delta;

                var ticked = Ticked;
                if (ticked != null)
                {
                    ticked();
                }
            }
            else
            {
                RemainTime = 0;

                var completed = Completed;
                if (completed != null)
                {
                    completed();
                }
            }
        }

        #region INotifyPropertyChanged implementation

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

}




//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Timers;
//using Xamarin.Forms;

//namespace REICTEventScheduler.Services
//{
//    /// <summary>
//    /// Converts countdown seconds double value to string "HH : MM : SS"
//    /// </summary>
//    public class CountdownConverter : IValueConverter
//    {
//        #region IValueConverter implementation

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            DateTime dateTime;
//            try
//            {
//                dateTime = DateTime.Parse(value.ToString());
//            }
//            catch (System.Exception ex)
//            {
//                dateTime = DateTime.Now;
//            }

//            var timespan = dateTime - DateTime.Now;

//            if (timespan.TotalSeconds < 1.0)
//            {
//                return "--:--";
//            }
//            //            else if (timespan.TotalSeconds < 3600)
//            //            {
//            //                return string.Format("{0:D2} : {1:D2}",
//            //                    timespan.Minutes, timespan.Seconds);
//            //            }
//            else if (timespan.TotalSeconds > 3600 * 24)
//            {
//                //return "24 : 00 : 00";
//                return string.Format("{0:D2} : {1:D2} : {2:D2} : {3:D2}",
//                    timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
//            }

//            return string.Format("{0:D2} : {1:D2} : {2:D2}",
//                timespan.Hours, timespan.Minutes, timespan.Seconds);
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            try
//            {
//                return DateTime.Parse(value.ToString());
//            }
//            catch (System.Exception ex) { return "--:--"; }
//        }

//        #endregion
//    }

//    /// <summary>
//    /// Converts countdown seconds double value to string "HH : MM : SS"
//    /// </summary>
//    public class ColorConverter : IValueConverter
//    {
//        #region IValueConverter implementation

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            string colorcode = "#6B8CAF";
//            Color color = Color.FromHex(colorcode);

//            if ((Color)value == Color.Red)
//                return color;
//            else
//                return Color.Red;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

//    /// <summary>
//    /// Countdown timer with periodical ticks.
//    /// </summary>
//    public class Countdown : INotifyPropertyChanged
//    {
//        /// <summary>
//        /// Gets the start date time.
//        /// </summary>
//        public DateTime StartDateTime { get; private set; }

//        /// <summary>
//        /// Gets the remain time in seconds.
//        /// </summary>
//        public double RemainTime
//        {
//            get { return remainTime; }

//            private set
//            {
//                remainTime = value;
//                OnPropertyChanged();
//            }
//        }

//        //public Color WarningBackColor
//        //{
//        //    get { return warningBackColor; }

//        //    set
//        //    {
//        //        string colorcode = "#6B8CAF";
//        //        Color color = Color.FromHex(colorcode);

//        //        if (warningBackColor == Color.Red)
//        //            warningBackColor = color;
//        //        else
//        //            warningBackColor = Color.Red;
//        //        OnPropertyChanged();
//        //    }
//        //}

//        public Color WarningTextColor
//        {
//            get { return warningTextColor; }

//            set
//            {
//                Color color = Color.Black;

//                if (warningTextColor == Color.White)
//                    warningTextColor = color;
//                else
//                    warningTextColor = Color.White;
//                OnPropertyChanged();
//            }
//        }

//        /// <summary>
//        /// Occurs when completed.
//        /// </summary>
//        public event Action Completed;

//        /// <summary>
//        /// Occurs when ticked.
//        /// </summary>
//        public event Action Ticked;

//        /// <summary>
//        /// The timer.
//        /// </summary>
//        Timer timer;

//        /// <summary>
//        /// The remain time.
//        /// </summary>
//        double remainTime;

//        //Color warningBackColor;
//        Color warningTextColor;

//        /// <summary>
//        /// The remain time total.
//        /// </summary>
//        double remainTimeTotal;

//        /// <summary>
//        /// Starts the updating with specified period, total time and period are specified in seconds.
//        /// </summary>
//        public void StartUpdating(double total, double period = 1.0)
//        {
//            if (timer != null)
//            {
//                StopUpdating();
//            }

//            remainTimeTotal = total;
//            RemainTime = total;

//            StartDateTime = DateTime.Now;

//            timer = new Timer(period * 1000);
//            timer = new Timer();
//            timer.Elapsed += (sender, e) => Tick();
//            timer.Enabled = true;
//        }

//        /// <summary>
//        /// Stops the updating.
//        /// </summary>
//        public void StopUpdating()
//        {
//            RemainTime = 0;
//            remainTimeTotal = 0;

//            if (timer != null)
//            {
//                timer.Enabled = false;
//                timer = null;
//            }
//        }

//        /// <summary>
//        /// Updates the time remain.
//        /// </summary>
//        public void Tick()
//        {
//            var delta = (DateTime.Now - StartDateTime).TotalSeconds;

//            if (delta < remainTimeTotal)
//            {
//                RemainTime = remainTimeTotal - delta;

//                var ticked = Ticked;
//                if (ticked != null)
//                {
//                    ticked();
//                }
//            }
//            else
//            {
//                RemainTime = 0;

//                var completed = Completed;
//                if (completed != null)
//                {
//                    completed();
//                }
//            }
//        }

//        #region INotifyPropertyChanged implementation

//        /// <summary>
//        /// Occurs when property changed.
//        /// </summary>
//        public event PropertyChangedEventHandler PropertyChanged;

//        /// <summary>
//        /// Raises the property changed event.
//        /// </summary>
//        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        #endregion
//    }

//}