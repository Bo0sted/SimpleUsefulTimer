using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace SimpleUsefulTimer
{

    [ValueConversion(typeof(int), typeof(string))]
    public class Win32KeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int win32Key = (int)value;
            if (win32Key == 0) return "Unset";
            return (KeyInterop.KeyFromVirtualKey(win32Key)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (strValue == "Unset") return 0;
            int resultKey;
            if (Int32.TryParse(strValue, out resultKey))
            {
                return resultKey;
            }
            return DependencyProperty.UnsetValue;
        }

    }
    public partial class Hotkeys : INotifyPropertyChanged
    {
        TimerControl tc;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private int _startStop;
        public int StartStop { get { return _startStop; } set { _startStop = value; OnPropertyChanged("StartStop"); } }
        public int Reset { get { return _reset; } set { _reset = value; OnPropertyChanged("Reset"); } }
        private int _reset;
        public int CurrentKey { get { return _currentKey; } set { _currentKey = value; OnPropertyChanged("CurrentKey"); } }
        private int _currentKey;
        public Hotkeys(ref TimerControl tc)
        {
            CurrentKey = 0;
            this.tc = tc;
            tc.shouldListenForHotkeys = false;
            StartStop = KeyInterop.VirtualKeyFromKey(tc._toggleTimerHotkey);
            Reset = KeyInterop.VirtualKeyFromKey(tc._resetTimerHotkey);
            InitializeComponent();

            if (tc.MainTimerForeground == null)
                SelectedKey.Foreground = TimerControl.ConvertColorToBrush(Properties.Settings.Default.DefaultTimerForeground);
            else
                SelectedKey.Foreground = tc.MainTimerForeground;

        }
        ~Hotkeys()
        {
            tc.shouldListenForHotkeys = true;
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentKey == '\0') { MessageBox.Show("Input a key first", "No hotkey detected", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            switch (((Button)sender).Name)
            {
                case "AssignStartStop":
                    {
                        tc._toggleTimerHotkey = KeyInterop.KeyFromVirtualKey(CurrentKey);
                        StartStop = KeyInterop.VirtualKeyFromKey(tc._toggleTimerHotkey);
                        Properties.Settings.Default.ToggleTimerHotkey = CurrentKey;
                        tc.SaveConfig();
                        break;
                    }
                case "AssignReset": 
                    {
                        tc._resetTimerHotkey = KeyInterop.KeyFromVirtualKey(CurrentKey);
                        Reset = KeyInterop.VirtualKeyFromKey(tc._resetTimerHotkey);
                        Properties.Settings.Default.ResetTimerHotkey = CurrentKey;
                        tc.SaveConfig();
                        break;
                    }
            }
            CurrentKey = 0;
            Dispatcher.Invoke(new Action(() =>
            {
                SelectedKey.Text = "Success!";
            }));
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            SelectedKey.Text = e.Key.ToString();
            CurrentKey = KeyInterop.VirtualKeyFromKey(e.Key);
        }

    }
}
