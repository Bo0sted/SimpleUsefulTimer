using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

using System.Configuration;
using Wpf.Ui.Controls;
using System.Security.Cryptography;

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : Window
    {
        private TimerControl timerControl;
        private Properties.Settings s = Properties.Settings.Default;
        public TimerView(ref TimerControl tc)
        {
            InitializeComponent();
            timerControl = tc;

            timerControl.TimerFontSize = (s.TimerFontSize == 0 ? s.DefaultTimerFontSize : s.TimerFontSize);
            var position = (s.LastTimerWindowPosition.IsEmpty ? s.DefaultWindowPosition:s.LastTimerWindowPosition);
            this.Top = position.Y;
            this.Left = position.X;

            var background = ((s.TimerBackground.IsEmpty) ? s.DefaultTimerBackground:s.TimerBackground);
            timerControl.MainTimerBackground = TimerControl.ConvertColorToBrush(background);

            if (background == s.DefaultTimerBackground)
                timerControl.IsBackgroundDefault = true;
            else if (background == System.Drawing.Color.Transparent)
                timerControl.IsBackgroundTransparent = true;
            else
            {
                timerControl.IsBackgroundTransparent = false;
                timerControl.IsBackgroundDefault = false;
                timerControl.BackgroundHexColorInput.Text = TimerControl.ColorToHex(background).TrimStart('#');
            }

            System.Drawing.Color foreground;

            if (s.UseCustomTimerForeground)
            {
                timerControl.UseCustomForeground = true;
                foreground = s.TimerForeground;
            }
            else
            {
                timerControl.UseCustomForeground = false;
                foreground = s.DefaultTimerForeground;
            }

            timerControl.ForegroundHexColorInput.Text = TimerControl.ColorToHex(foreground).TrimStart('#');

            if ( s.UseTimerForegroundGradient)
            {
                timerControl.UseCustomForegroundGradientCheck.IsChecked = true;
                timerControl.ForegroundGradient = TimerControl.ConvertColorToBrush(s.TimerForegroundGradient);
                timerControl.GradientHexColorInput.Text = TimerControl.ColorToHex(s.TimerForegroundGradient);
                timerControl.MainTimerForeground = (s.UseTimerForegroundGradient ? TimerControl.GetGradientFrom(s.TimerForeground, s.TimerForegroundGradient): TimerControl.ConvertColorToBrush(s.DefaultTimerForeground));
                return;
            }

            timerControl.MainTimerForeground = TimerControl.ConvertColorToBrush(foreground);
            this.UpdateLayout();
        }

        public void SaveConfig()
        {
            var s = Properties.Settings.Default;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            int currentLeft = (Int32)this.Left;
            int currentTop = (Int32)this.Top;
            var lastKnownLocation = new System.Drawing.Point(x: currentLeft, y: currentTop);
            s.LastTimerWindowPosition = lastKnownLocation;
            SaveConfig();
            Application.Current.Shutdown(0);
        }

        private void Preview_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
                return;

            timerControl.Visibility = ((timerControl.Visibility is Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) { 
                this.DragMove();
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (s.DidReadWelcomeMessage)
                return;

            var w = new Welcome();
            w.ShowDialog();
        }
    }
}
