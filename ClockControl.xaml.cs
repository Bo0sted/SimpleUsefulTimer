using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    public partial class ClockControl : INotifyPropertyChanged
    {
        public TimerControl timerControl;
        public static DispatcherTimer dispatcherClock = new DispatcherTimer();
        public Brush ClockBackground { get { return _clockBackground; } set { _clockBackground = value; OnPropertyChanged("ClockBackground"); } }
        public string ClockFont { get { return _clockFont; } set { _clockFont = value; OnPropertyChanged("ClockFont"); } }
        public string ClockFormat { get { return _clockFormat; } set { _clockFormat = value; OnPropertyChanged("ClockFormat"); } }
        public int ClockFontSize { get { return _clockFontSize; } set { _clockFontSize = value; OnPropertyChanged("ClockFontSize"); } }
        public int ClockBackgroundSetting { get { return _clockBackgroundSetting; } set { _clockBackgroundSetting = value; OnPropertyChanged("ClockBackgroundSetting"); } }
        public int ClockForegroundSetting { get { return _clockForegroundSetting; } set { _clockForegroundSetting = value; OnPropertyChanged("ClockForegroundSetting"); } }
        public Brush ClockForeground { get { return _clockForeground; } set { _clockForeground = value; OnPropertyChanged("ClockForeground"); } }
        public string Clock { get { return _clock; } set { _clock = value; OnPropertyChanged("Clock"); } }
        Brush ?_clockBackground = null;
        string ?_clockFont="";
        int _clockFontSize =0;
        Brush ?_clockForeground = null;
        string ?_clockFormat;
        int _clockBackgroundSetting=0, _clockForegroundSetting=0;
        string _clock="Initializing...";
        static ClockMonitor clockInstance;
        Properties.Settings s = Properties.Settings.Default;
        ClockControl Self;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ClockControl(ref TimerControl tc)
        {
            Self = this;
            timerControl = tc;
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            ClockFontSize = s.ClockFontSize;
            ClockFontSizeInput.Text = ClockFontSize.ToString();
            ClockFont = s.ClockFont;
            ClockForeground = TimerControl.ConvertColorToBrush(s.ClockForeground);
            ClockFormat = s.ClockFormat;
            ClockBackgroundSetting = s.ClockBackgroundSetting;
            ClockForegroundSetting = s.ClockForegroundSetting;
            dispatcherClock.Tick += new EventHandler(dispatcherClock_Tick);
            dispatcherClock.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dispatcherClock.Start();
            DataContext = this;
            UpdateClockBackgroundSettings(ClockBackgroundSetting, TimerControl.ColorToHex(s.ClockBackground), true);
            UpdateClockForegroundSettings(ClockForegroundSetting, TimerControl.ColorToHex(s.ClockForeground), true);
            clockInstance = new ClockMonitor(ref Self);
            clockInstance.Show();
        }
        public static ClockMonitor getClockInstance()
        {
            return clockInstance;
        }
        private void ClockBackgroundInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ClockBackgroundSetting != 2) return; //custom setting should be ignored if not selected

            UpdateClockBackgroundSettings(2, ClockBackgroundInput.Text);
        }

        private void dispatcherClock_Tick(object? sender, EventArgs e)
        {
            Clock = DateTime.Now.ToString(ClockFormat);//");

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            s.Save();
            Visibility = Visibility.Hidden;
            UpdateLayout();
        }

        public void UpdateClockBackgroundSettings(int index, string color="", bool init=false)
        {
            ClockBackgroundSettingCombo.SelectedIndex = index;
            UpdateLayout();
            if (index == 0)
            {
                ClockBackground = timerControl.MainTimerBackground;
                s.ClockBackgroundSetting = 0;
            }
            else if (index == 1)
            {
                ClockBackground = Brushes.Transparent;
                s.ClockBackgroundSetting = 1;
            }
            else if (index == 2)
            {
                if (ClockBackgroundInput.Text.Length != 5 && ClockBackgroundInput.Text.Length != 6)
                {
                    if (!init)
                        return;
                }
                if (color == "" && !init)
                {
                    color = ClockBackgroundInput.Text;
                }
                try
                {
                    var translated_color = ColorTranslator.FromHtml($"#{color}");
                    ClockBackground = TimerControl.ConvertColorToBrush(translated_color);
                    s.ClockBackground = translated_color;
                    s.ClockBackgroundSetting = 2;
                    ClockBackground = TimerControl.ConvertColorToBrush(translated_color);
                    if (init) ClockBackgroundInput.Text = color;
                    UpdateLayout();
                    SaveConfig();
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }

            SaveConfig();
        }

        private void ClockBackgroundSettingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ClockBackgroundSettingCombo.SelectedIndex;
            UpdateClockBackgroundSettings(index);
        }

        public void UpdateClockForegroundSettings(int index, string color = "", bool init = false)
        {
            ClockForegroundCombo.SelectedIndex = index;
            UpdateLayout();
            if (index == 0)
            {
                ClockForeground = timerControl.MainTimerForeground;
                s.ClockForegroundSetting = 0;
            }
            else if (index == 1)
            {
                if (ClockForegroundInput.Text.Length != 5 && ClockForegroundInput.Text.Length != 6)
                {
                    if (!init)
                        return;
                }
                if (color == "" && !init)
                {
                    color = ClockForegroundInput.Text;
                }
                try
                {
                    var translated_color = ColorTranslator.FromHtml($"#{color}");
                    ClockForeground = TimerControl.ConvertColorToBrush(translated_color);
                    s.ClockForeground = translated_color;
                    s.ClockForegroundSetting = 1;
                    ClockForeground = TimerControl.ConvertColorToBrush(translated_color);
                    if (init) ClockForegroundInput.Text = color;
                    UpdateLayout();
                    SaveConfig();
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }

            SaveConfig();
        }

        private void ClockForegroundCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ClockForegroundCombo.SelectedIndex;
            UpdateClockForegroundSettings(index);
        }

        private void ClockForegroundInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ClockForegroundSetting != 1) return; //custom setting should be ignored if not selected

            UpdateClockForegroundSettings(1, ClockForegroundInput.Text);
        }

        private void ClockFontSizeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            s.ClockFontSize = (ClockFontSizeInput.Text != "" ? Int32.Parse(ClockFontSizeInput.Text) : s.DefaultTimerFontSize);
            ClockFontSize = (ClockFontSizeInput.Text != "" ? Int32.Parse(ClockFontSizeInput.Text) : s.DefaultTimerFontSize);
            SaveConfig();
        }

        private void ClockControlFontChanger_Click(object sender, RoutedEventArgs e)
        {
            var fontDialogue = new FontPicker(ref Self);
            fontDialogue.ShowDialog();
        }

        public void SaveConfig()
        {
            var s = Properties.Settings.Default;
            s.Save();
            s.Reload();
        }
    }
}
