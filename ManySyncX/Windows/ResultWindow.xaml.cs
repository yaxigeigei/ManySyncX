using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace ManySyncX
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        Inventory inventory = new Inventory();

        public ResultWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inventory = MainWindow.MWInstance.selectedOneTask.lastInventory;

            this.Top = (SystemParameters.FullPrimaryScreenHeight - this.Height) / 3.5;          // Screen positioning

            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            WindowBorder.BeginAnimation(OpacityProperty, da);

            int fileCopy = inventory.fileCopyFrom.Count;
            int fileDel = inventory.fileDel.Count;
            int fileUpdate = inventory.fileUpdateTo.Count;
            int failed = inventory.pathTooLong.Count + inventory.otherFailed.Count;
            int ignored = inventory.ignored.Count;

            // Display labels
            if (inventory.mode == "Preview")
            {
                FileAddLabel.Content = fileCopy + " Files to Copy";
                FileDelLabel.Content = fileDel + " Files to Delete";
                FileUpdateLabel.Content = fileUpdate + " Files to Update";
                ItemFailLabel.Content = failed + " Items will Fail";
            }
            else
            {
                FileAddLabel.Content = fileCopy + " Files Copied";
                FileDelLabel.Content = fileDel + " Files Deleted";
                FileUpdateLabel.Content = fileUpdate + " Files Updated";
                ItemFailLabel.Content = failed + " Items Failed";
            }
            ItemSkipLabel.Content = ignored + " Items Ignored";

            // Display colorbar
            ColorBarGrid.ColumnDefinitions[0].Width = new GridLength(inventory.fileCopyFrom.Count, GridUnitType.Star);
            ColorBarGrid.ColumnDefinitions[1].Width = new GridLength(inventory.fileDel.Count, GridUnitType.Star);
            ColorBarGrid.ColumnDefinitions[2].Width = new GridLength(inventory.fileUpdateFrom.Count, GridUnitType.Star);
            ColorBarGrid.ColumnDefinitions[4].Width = new GridLength(inventory.ignored.Count, GridUnitType.Star);
            ColorBarGrid.ColumnDefinitions[5].Width = new GridLength(inventory.pathTooLong.Count + inventory.otherFailed.Count, GridUnitType.Star);
            Thread t = new Thread(new ThreadStart(ColorBarAnimation));
            t.Start();
        }

        private void ColorBarAnimation()
        {
            int u = inventory.fileUnchange.Count;
            if (u < inventory.totalTargetFiles.Count * 0.5 | u == inventory.totalTargetFiles.Count | u < 20)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                    ColorBarGrid.ColumnDefinitions[3].Width =
                        new GridLength(inventory.fileUnchange.Count, GridUnitType.Star);
                    FileUnchangeLabel.Content = inventory.fileUnchange.Count + " Files Unchanged";
                });
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                    ColorBarGrid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Star);
                    FileUnchangeLabel.Content = 0 + " Files Unchanged";
                });
                
                int power = 5;
                int x = 1000;
                double a = (double)inventory.fileUnchange.Count / Math.Pow((double)x, power);

                Thread.Sleep(1500);
                for (int i = 0; i < x; i += 4)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                    {
                        ColorBarGrid.ColumnDefinitions[3].Width = 
                            new GridLength(a * Math.Pow(i, power), GridUnitType.Star);
                        FileUnchangeLabel.Content = Math.Round(a * Math.Pow(i, power)) + " Files Unchanged";
                    });
                    Thread.Sleep(15);
                }
            }
        }


        private void CheckDetails_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            string name = (string)label.Name;
            ArrayList message = new ArrayList();
            switch (name)
            {
                case "FileAddLabel": message = inventory.fileCopyTo; break;
                case "FileDelLabel": message = inventory.fileDel; break;
                case "FileUpdateLabel": message = inventory.fileUpdateTo; break;
                case "ItemFailLabel": message = inventory.totalFailed; break;
                case "ItemSkipLabel": message = inventory.ignored; break;
                case "FileUnchangeLabel": message = inventory.fileUnchange; break;
                default: break;
            }
            DisplayCategory(message);
        }

        private void DisplayCategory(ArrayList message)
        {
            if (this.Height < 220 & message.Count != 0)
            {
                DoubleAnimation da = new DoubleAnimation
                    (this.Height, 500, TimeSpan.FromMilliseconds(500), FillBehavior.Stop);
                da.DecelerationRatio = 1;
                this.BeginAnimation(HeightProperty, da);
            }

            ListBox1.Items.Clear();
            foreach (string path in message)
                ListBox1.Items.Add(path);
        }

        private void ExpRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            string summary = " RESULTS" + Environment.NewLine;
            if (inventory.mode == "Sync")
                summary = "SYNCHRONIZATION" + summary;
            else if (inventory.mode == "Preview")
                summary = "ANALYSIS" + summary;

            if (inventory.fileCopyTo.Count != 0)
            {
                summary += (Environment.NewLine + "New files:" + Environment.NewLine);
                foreach (string s in inventory.fileCopyTo)
                    summary += (s + Environment.NewLine);
            }
            if (inventory.fileDel.Count > 0)
            {
                summary += (Environment.NewLine + "Delete files:" + Environment.NewLine);
                foreach (string s in inventory.fileDel)
                    summary += (s + Environment.NewLine);
            }
            if (inventory.fileUpdateTo.Count > 0)
            {
                summary += (Environment.NewLine + "Update files:" + Environment.NewLine);
                foreach (string s in inventory.fileUpdateTo)
                    summary += (s + Environment.NewLine);
            }
            if (inventory.totalFailed.Count > 0)
            {
                summary += (Environment.NewLine + "Failed Items:" + Environment.NewLine);
                foreach (string s in inventory.totalFailed)
                    summary += (s + Environment.NewLine);
            }
            if (inventory.ignored.Count > 0)
            {
                summary += (Environment.NewLine + "Ignored Items:" + Environment.NewLine);
                foreach (string s in inventory.ignored)
                    summary += (s + Environment.NewLine);
            }
            ExportRecords(summary);
        }

        // Write the information of records list to a .txt file
        private void ExportRecords(string contents)
        {
            string header = DateTime.Now.ToString("yyyy-MM-dd hhmmss");

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                Path.DirectorySeparatorChar + "ManySyncX" + Path.DirectorySeparatorChar + header + " Records.txt";

            File.AppendAllText(filePath, contents);
            Process.Start(filePath);
        }

        private void NumbersGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = (Label)sender;
                label.Background = Brushes.WhiteSmoke;
            }
        }
        private void NumbersGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = (Label)sender;
                label.Background = Brushes.Transparent;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
                // Strange error occurs when not catching
            }
        }




    }
}
