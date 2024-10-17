﻿using SimpleUsefulTimer.Properties;
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
using System.Drawing.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SimpleUsefulTimer
{
    /// <summary>
    /// Interaction logic for FontPicker.xaml
    /// </summary>
    public partial class FontPicker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        readonly private TimerControl tc;
        readonly private Properties.Settings s = Properties.Settings.Default;
        readonly public string defaultTimerFont;
        private string _selectedFont = "";
        public string SelectedFont { get { return _selectedFont; } set { _selectedFont = value; OnPropertyChanged("SelectedFont"); } }
        private ObservableCollection<FontFamily> _userFonts = new();
        public ObservableCollection<FontFamily> UserFonts { get { return _userFonts; } set { _userFonts = value; OnPropertyChanged("UserFonts"); } }
        public FontPicker(ref TimerControl tc)
        {
            DataContext = this;
            InitializeComponent();
            defaultTimerFont = s.DefaultTimerFont;
            this.tc = tc;
            SelectedFont = tc.TimerCustomFont;

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                UserFonts.Add(fontFamily);
            }
            UserFonts = new ObservableCollection<FontFamily>(UserFonts.OrderBy(c=>c.Source));


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedFont = defaultTimerFont;
            tc.TimerCustomFont = SelectedFont;
            s.TimerFont = "";
            s.Save();
            s.Reload();
        }

        private void SaveCustomFont_Click(object sender, RoutedEventArgs e)
        {
            tc.TimerCustomFont = SelectedFont;
            s.TimerFont = SelectedFont;
            s.Save();
            s.Reload();
        }
    }
}