using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;
using System.Xml;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using System.Drawing;
using Color = System.Drawing.Color;
using ColorConverter = System.Drawing.ColorConverter;
using System.Windows.Automation.Peers;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Globalization;


namespace SimpleUsefulTimer
{
    public partial class TimerControl : INotifyPropertyChanged
    {
        public static TimerControl Self;
        public static TimerView timerView;
        private Properties.Settings s = Properties.Settings.Default;
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public static Stopwatch stopWatch;
        private bool _is_background_default, _is_background_transparent = false;
        public bool IsBackgroundDefault { get { return _is_background_default; } set { _is_background_default = value; OnPropertyChanged("IsBackgroundDefault"); } }
        public bool IsBackgroundTransparent { get { return _is_background_transparent; } set { _is_background_transparent = value; OnPropertyChanged("IsBackgroundTransparent"); } }
        public bool IsBackgroundCustom { get { if (IsBackgroundDefault == false && IsBackgroundTransparent == false) return true; else return false; } }
        private bool _useCustomForeground = false;
        public bool UseCustomForeground { get { return _useCustomForeground; }set { _useCustomForeground = value;OnPropertyChanged("UseCustomForeground"); } }
        private Brush _foreground= Brushes.Transparent;
        public Brush MainTimerForeground { get { return _foreground; } set { _foreground = value; OnPropertyChanged("MainTimerForeground"); } }

        private bool _useForegroundGradient = false;
        public bool UseForegroundGradient { get { return _useForegroundGradient; } set { _useForegroundGradient = value; OnPropertyChanged("UseForegroundGradient"); } }
        private Brush _foregroundGradient = Brushes.Transparent;
        public Brush ForegroundGradient { get { return _foregroundGradient; } set { _foregroundGradient = value; OnPropertyChanged("ForegroundGradient"); } }

        private Boolean _hex_color_input = false;
        private bool didWarnUserAboutConfigWindow = false;

        private int _msFieldAlignment;
        public int MsFieldAlignment { get { return _msFieldAlignment; } set { _msFieldAlignment = value; OnPropertyChanged("MsFieldAlignment"); } }
        public Boolean HexColorInput
        {
            get { return _hex_color_input; }
            set
            {
                _hex_color_input = value;
                OnPropertyChanged("HexColorInput");
            }
        }
        private string _timer = "";
        public string Timer
        {
            get
            {
                return _timer;
            }
            set
            {
                _timer = value;
                OnPropertyChanged("Timer");
            }
        }
        private string _timerMs = "";
        public string TimerMs
        {
            get
            {
                return _timerMs;
            }
            set
            {
                _timerMs = value;
                OnPropertyChanged("TimerMs");
            }
        }
        private int _fontSize;
        public int TimerFontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                OnPropertyChanged("TimerFontSize");
                OnPropertyChanged("AdjustedTimerFontSize");
            }
        }

        public int AdjustedTimerFontSize
        {
            get
            {
                return TimerFontSize - 15;
            }
        }

        private string _customFont = "";
        public string TimerCustomFont
        {
            get
            {
                return _customFont;
            }
            set
            {
                _customFont = value;
                OnPropertyChanged("TimerCustomFont");
            }
        }
        private Brush _background = Brushes.White;
        public Brush MainTimerBackground
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged("MainTimerBackground");
            }
        }

        //https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=windowsdesktop-8.0
        public Key _toggleTimerHotkey;
        public Key _resetTimerHotkey;
        public bool respondToHotkeys = true;
        public HotkeyResponderLoop hotkeyLoop;
        public List<string> KeyList
        {
            get { return Enum.GetNames(typeof(Key)).ToList(); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void SaveConfig()
        {
            var s = Properties.Settings.Default;
            s.Save();
            s.Reload();
        }
        public TimerControl()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Self = this;
            timerView = new TimerView(ref Self);
            stopWatch = new Stopwatch();
            //stopWatch.Start();
            dispatcherTimer.Start();
            Timer = "Initializing...";
            HexColorInput = false;
            timerView.Show();

            _resetTimerHotkey = (Key)(s.ResetTimerHotkey == 0 ? s.DefaultResetTimerHotkey : s.ResetTimerHotkey);
            _toggleTimerHotkey = (Key)(s.ToggleTimerHotkey == 0 ? s.DefaultToggleTimerHotkey : s.ToggleTimerHotkey);
            hotkeyLoop = new HotkeyResponderLoop(ref Self);
        }


        public static void SetWindowEnabledOrNot(bool enableWindow)
        {
            Self.IsEnabled = enableWindow;
        }
        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            switch (stopWatch.Elapsed.TotalMinutes)
            {
                case var expression when (stopWatch.Elapsed.TotalMinutes < 1):
                    Timer = stopWatch.Elapsed.ToString("ss").TrimStart('0');
                    if (stopWatch.Elapsed.TotalMilliseconds < 1000)
                        Timer = Timer.Insert(0,"0");
                    break;
                case var expression when (stopWatch.Elapsed.TotalMinutes >= 1):
                    Timer = stopWatch.Elapsed.ToString(@"mm\:ss").TrimStart('0');
                    break;
                case var expression when (stopWatch.Elapsed.TotalMinutes > 60):
                    Timer = stopWatch.Elapsed.ToString(@"hh\:mm\:ss\\").TrimStart('0');
                    break;
                default:
                    break;

            }

            TimerMs = stopWatch.Elapsed.ToString(@"\.ff");
                
        }

        private void BackgroundHexColorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BackgroundHexColorInput.Text.Length != 6)
            {
                return;
            }
            var clean = "";
            if (BackgroundHexColorInput.Text.Length == 6)
                clean = $"#{BackgroundHexColorInput.Text}";
            else
                clean = $"{BackgroundHexColorInput.Text}";

            try
            {
                var color = ColorTranslator.FromHtml(clean);
                MainTimerBackground = ConvertColorToBrush(color);
                s.TimerBackground = color;
                SaveConfig();
                return;
            }
            catch (Exception)
            {
                MainTimerBackground = ConvertColorToBrush(s.DefaultTimerBackground);
                return;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton)
            {
                RadioButton radio = (RadioButton)e.Source;

                if (radio.GroupName == "background")
                {
                    switch (radio.Content)
                    {
                        case "Default":
                            {
                                ResetTimerBackground();
                                s.TimerBackground = s.DefaultTimerBackground;
                                break;
                            }
                        case "Transparent":
                            {
                                MainTimerBackground = Brushes.Transparent;
                                s.TimerBackground = Color.Transparent;
                                break;
                            }
                        case "Hex":
                            {
                                // Textbox update event will save hex code only if its valid
                                break;
                            }
                        default:
                            break;
                    }
                }
                SaveConfig();

            }
        }

        public static string ColorToHex(System.Drawing.Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Brush ConvertColorToBrush(Color color)
        {
           return (new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B)));
        }
        public static System.Windows.Media.Color ConvertDrawingColorToWinMediaColor(System.Drawing.Color color)
        {
            var winMediaColor = new System.Windows.Media.Color();
            
            winMediaColor.R = color.R;
            winMediaColor.G = color.G;
            winMediaColor.B = color.B;
            winMediaColor.A = color.A;

            return winMediaColor;
        }

        public static void ToggleTimer()
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
            }
            else if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
            }
        }
        public static void ResetTimer()
        {
            stopWatch.Reset();
        }
        private void ResetTimerBackground()
        {
            MainTimerBackground = ConvertColorToBrush(s.DefaultTimerBackground);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
    }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            s.Save();
            Visibility = Visibility.Hidden;
            UpdateLayout();
        }

        private void EnableCustomForeground_Checked(object sender, RoutedEventArgs e)
        {
            if (ForegroundHexColorInput.Text == null)
                return;
            ForegroundHexColorInput.Text = (s.TimerForeground.IsEmpty ? ColorToHex(s.DefaultTimerForeground) : ColorToHex(s.TimerForeground));
            MainTimerForeground = ConvertColorToBrush(s.TimerForeground);
            s.UseCustomTimerForeground = true;
            s.Save();
        }
        private void UseCustomForegroundCheck_Unchecked(object sender, RoutedEventArgs e)
        {

            MainTimerForeground = ConvertColorToBrush(s.DefaultTimerForeground);
            UseCustomForegroundGradientCheck.IsChecked = false;
            s.UseCustomTimerForeground = false;
            s.Save();
        }

        private void ForegroundHexColorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ForegroundHexColorInput.Text.Length != 6)
            {
                return;
            }
            var clean = "";
            if (ForegroundHexColorInput.Text.Length == 6)
                clean = $"#{ForegroundHexColorInput.Text}";
            else
                clean = $"{ForegroundHexColorInput.Text}";

            try
            {
                var color = ColorTranslator.FromHtml(clean);
                MainTimerForeground = ConvertColorToBrush(color);
                s.TimerForeground = color;
                UseForegroundGradient = false;
                SaveConfig();
                return;
            }
            catch (Exception)
            {
                MainTimerForeground = ConvertColorToBrush(s.DefaultTimerForeground);
                return;
            }
        }

        private void UseCustomForegroundGradientCheck_Checked(object sender, RoutedEventArgs e)
        {
            MainTimerForeground = (s.TimerForeground.IsEmpty ? ConvertColorToBrush(s.DefaultTimerForeground) : GetGradientFrom(s.TimerForeground, s.TimerForegroundGradient));
            GradientHexColorInput.Text = (s.TimerForegroundGradient.IsEmpty ? ColorToHex( s.DefaultForegroundGradient): ColorToHex(s.TimerForegroundGradient));
            UseForegroundGradient = true;
            s.UseTimerForegroundGradient = true;
            SaveConfig();
        }
        private void UseCustomForegroundGradientCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            MainTimerForeground = (s.UseCustomTimerForeground ? ConvertColorToBrush(s.TimerForeground): ConvertColorToBrush(s.DefaultTimerForeground));
            UseForegroundGradient = false;
            s.UseTimerForegroundGradient = false;
            SaveConfig();
        }
        private void ForegroundGradientHexColorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GradientHexColorInput.Text.Length != 6)
            {
                return;
            }
            if (UseCustomForegroundGradientCheck.IsChecked == false)
            {
                return;
            }
            var clean = "";
            if (GradientHexColorInput.Text.Length == 6)
                clean = $"#{GradientHexColorInput.Text}";
            else
                clean = $"{GradientHexColorInput.Text}";

            try
            {
                var color = ColorTranslator.FromHtml(clean);
                var gradient = GetGradientFrom(s.TimerForeground, color);

                MainTimerForeground = gradient;
                s.TimerForegroundGradient = color;
                SaveConfig();
                return;
            }
            catch (Exception)
            {
                MainTimerForeground = ConvertColorToBrush(s.DefaultTimerForeground);
                return;
            }
        }

        public static LinearGradientBrush GetGradientFrom(Color color1, Color color2)
        {
            ColorConverter cc = new ColorConverter();
            LinearGradientBrush gradient = new LinearGradientBrush();
            GradientStopCollection gradientStops = new GradientStopCollection();
            GradientStop gradientStopOne = new GradientStop();
            GradientStop gradientStopTwo = new GradientStop();

            gradientStopOne.Color = ConvertDrawingColorToWinMediaColor(color1);
            gradientStopOne.Offset = 0.4;
            gradientStopTwo.Color = ConvertDrawingColorToWinMediaColor(color2);
            gradientStopTwo.Offset = 0.6;
            gradientStops.Add(gradientStopOne);
            gradientStops.Add(gradientStopTwo);
            gradient.GradientStops = gradientStops;

            return gradient;
        }

        private void ResetWelcomeMessage_Click(object sender, RoutedEventArgs e)
        {
            s.DidReadWelcomeMessage = false;
            s.Save();
        }

        private void FontChangeDialogueOpener_Click(object sender, RoutedEventArgs e)
        {
            var fontDialogue = new FontPicker(ref Self);
            fontDialogue.ShowDialog();
        }


        private void MsFieldControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MsFieldAlignment = MsFieldControl.SelectedIndex;
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            timerView.Close();
        }

        private void NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            s.TimerFontSize = (FontSizeNumberBox.Text != "" ? Int32.Parse(FontSizeNumberBox.Text): s.DefaultTimerFontSize);
            s.Save();
        }

        

    }
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int) return Visibility.Hidden;
            if ((int)value == 3) return Visibility.Hidden;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    public class IntToVerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 3: break;
                case 0: return VerticalAlignment.Top;
                case 1: return VerticalAlignment.Center;
                case 2: return VerticalAlignment.Bottom;
            }
            return VerticalAlignment.Stretch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((VerticalAlignment)value)
            {
                case VerticalAlignment.Top: return 0;
                case VerticalAlignment.Center: return 1;
                case VerticalAlignment.Bottom: return 2;
                case VerticalAlignment.Stretch: break;
            }
            return 3;
        }
    }

}