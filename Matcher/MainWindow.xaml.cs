using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.IO;

namespace Matcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PlayersList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void UploadButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a text file";
            op.Filter = "Text file (*.txt)|*.txt;";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK) PlayersList = File.ReadAllText(op.FileName);

            //StringBuilder sb;
            //StringReader sr = new StringReader(PlayersList);


        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClose(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
