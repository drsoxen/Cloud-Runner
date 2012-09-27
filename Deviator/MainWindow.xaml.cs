using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Deviator
{
    public partial class MainWindow : Window
    {
        Host host = new Host();
        bool firstline = true;

        void LogToBox(string str)
        {   
            this.logblock.Text += (!firstline ? "\n" : "") + str;
            firstline = false;
            this.logblock.ScrollToEnd();
        }

        public MainWindow()
        {
            InitializeComponent();
            Log.Logables.Add(LogToBox);

            host.Initialize("../server.txt");
            host.Upload("../server.txt", "Code/Haxorz.txt");
            byte[] hi = host.Download("Code/Haxorz.txt");
            
            
        }

    }
}
