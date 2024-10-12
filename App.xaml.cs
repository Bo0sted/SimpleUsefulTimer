using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {

            MessageBox.Show("Uh oh! The program has suffered a fatal crash. Please view log file in same directory as this Timer app for more information.");
            File.WriteAllText($"{Directory.GetCurrentDirectory()}/crash-{new Random().Next()}.txt", args.Exception.ToString());
            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

    }

}
