using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
using static System.Net.WebRequestMethods;

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
            WarmWelcome.Text = $"Welcome, {Environment.UserName}";
            var assembly = Assembly.GetAssembly(MethodBase.GetCurrentMethod().DeclaringType).GetName();
            VersionNumber.Text = $"{assembly.Version}";
            TimerControl.SetWindowEnabledOrNot(false);
            GithubLink.Foreground = TimerControl.ConvertColorToBrush( Properties.Settings.Default.DefaultTimerForeground);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HardCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DidReadWelcomeMessage = true;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Bo0sted/SimpleUsefulTimer") { UseShellExecute = true });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TimerControl.SetWindowEnabledOrNot(true);
        }
    }
}
