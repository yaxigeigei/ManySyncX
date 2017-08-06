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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ManySyncX
{
    /// <summary>
    /// Interaction logic for AboutScreen.xaml
    /// </summary>
    public partial class AboutScreen : Window
    {
        public AboutScreen()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.Top = (SystemParameters.FullPrimaryScreenHeight - this.Height) / 2.5;      // Screen positioning
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            this.BeginAnimation(OpacityProperty, da);
        }

        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

    }
}
