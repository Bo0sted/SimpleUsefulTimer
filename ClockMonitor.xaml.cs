using System;
using System.Collections.Generic;
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

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for ClockMonitor.xaml
    /// </summary>
    public partial class ClockMonitor : Window
    {
        ClockControl cc;
        Properties.Settings s = Properties.Settings.Default;
        public ClockMonitor(ref ClockControl cc)
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.cc = cc;
            DataContext = cc;
            cc.Clock = "Reading system time...";
            var position = (s.LastKnowClockPosition.IsEmpty ? s.DefaultWindowPosition : s.LastKnowClockPosition);
            this.Top = position.Y;
            this.Left = position.X;

        }

        private void ClockMonitorWindow_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cc.Visibility = ((cc.Visibility is Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
        }

        private void ClockMonitorWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ClockMonitorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int currentLeft = (Int32)this.Left;
            int currentTop = (Int32)this.Top;
            var lastKnownLocation = new System.Drawing.Point(x: currentLeft, y: currentTop);
            s.LastKnowClockPosition = lastKnownLocation;
            s.Save();
            SaveConfig();
        }
        public void SaveConfig()
        {
            var s = Properties.Settings.Default;
            s.Save();
            s.Reload();
        }

    }
}
