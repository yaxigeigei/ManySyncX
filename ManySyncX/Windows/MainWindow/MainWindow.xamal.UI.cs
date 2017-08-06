using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;

namespace ManySyncX
{
    public partial class MainWindow : Window
    {

        #region USER INTERFACE: STATIC

        // Window control
        private void CloseButton_Click(object sender, EventArgs e)                              // Exit application
        {
            this.Close();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)                     // Minimize window
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void Minimize2TrayButton_Click(object sender, RoutedEventArgs e)                // Minimize to system tray
        {
            this.Hide();
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)     // Save settings on exit
        {
            ni.Visible = false;
            ReadSettings();
            SaveSettings();
            resultWindow.Close();
        }

        // System tray
        private void DefineTrayIcon()
        {
            Stream iconStream = System.Windows.Application.GetResourceStream
                (new Uri("Images\\Icon1.ico", UriKind.Relative)).Stream;
            ni.Icon = new Icon(iconStream);
            ni.Visible = true;
            ni.DoubleClick += TrayShowWindow;
            ni.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            GenerateContextMenu();
        }
        private void GenerateContextMenu()
        {
            ni.ContextMenuStrip.Items.Clear();
            if (allSets.enableWatch)
            {
                Stream watchStream = System.Windows.Application.GetResourceStream
                    (new Uri("Images\\trayWatch.png", UriKind.Relative)).Stream;
                System.Drawing.Image imWatch = System.Drawing.Image.FromStream(watchStream);
                ni.ContextMenuStrip.Items.Add("Auto sync ON", imWatch, WatchOnOff);
            }
            else
            {
                Stream watchStream = System.Windows.Application.GetResourceStream
                    (new Uri("Images\\trayNotWatch.png", UriKind.Relative)).Stream;
                System.Drawing.Image imWatch = System.Drawing.Image.FromStream(watchStream);
                ni.ContextMenuStrip.Items.Add("Auto sync OFF", imWatch, WatchOnOff);
            }

            Stream exitStream = System.Windows.Application.GetResourceStream
                    (new Uri("Images\\trayExit.png", UriKind.Relative)).Stream;
            System.Drawing.Image imExit = System.Drawing.Image.FromStream(exitStream);

            Stream showStream = System.Windows.Application.GetResourceStream
                    (new Uri("Images\\trayWindow.png", UriKind.Relative)).Stream;
            System.Drawing.Image imShow = System.Drawing.Image.FromStream(showStream);

            ni.ContextMenuStrip.Items.Add("Show window", imShow, TrayShowWindow);
            ni.ContextMenuStrip.Items.Add("Exit ManySyncX", imExit, CloseButton_Click);
        }
        private void WatchOnOff(object sender, EventArgs e)
        {
            EnableWatchCheckbox.IsChecked = !EnableWatchCheckbox.IsChecked;
            GenerateContextMenu();
        }
        private void TrayShowWindow(object sender, EventArgs args)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        // Open Folder Dialog
        public string fbDialog()
        {
            FolderBrowserDialog b = new FolderBrowserDialog();
            DialogResult result = b.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            return b.SelectedPath.Trim();
        }

        // Folder selection
        private void BrowseDiretory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = new System.Windows.Controls.TextBox();
            textbox = (System.Windows.Controls.TextBox)sender;
            string path = fbDialog();
            if (path != "") { textbox.Text = path; }
        }

        // Swap paths between source and target
        private void SwapImage1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string sourceTemp = sourceDirText1.Text;
            string targetTemp = targetDirText1.Text;

            if (sourceTemp != "Double click here to select a source" &
                targetTemp != "Double click here to select a target")
            {
                sourceDirText1.Text = targetTemp;
                targetDirText1.Text = sourceTemp;
            }
        }
        private void SwapImage2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string sourceTemp = sourceDirText2.Text;
            string targetTemp = targetDirText2.Text;
            sourceDirText2.Text = targetTemp;
            targetDirText2.Text = sourceTemp;
        }
        private void SwapImage3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string sourceTemp = sourceDirText3.Text;
            string targetTemp = targetDirText3.Text;
            sourceDirText3.Text = targetTemp;
            targetDirText3.Text = sourceTemp;
        }
        private void SwapImage4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string sourceTemp = sourceDirText4.Text;
            string targetTemp = targetDirText4.Text;
            sourceDirText4.Text = targetTemp;
            targetDirText4.Text = sourceTemp;
        }
        private void SwapImage5_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string sourceTemp = sourceDirText5.Text;
            string targetTemp = targetDirText5.Text;
            sourceDirText5.Text = targetTemp;
            targetDirText5.Text = sourceTemp;
        }

        // Show About screen
        private void aboutButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AboutScreen about = new AboutScreen();
            about.ShowDialog();
        }

        // Show Settings
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow setWindow = new SettingWindow();
            setWindow.Owner = this;
            setWindow.ShowDialog();

            if (allSets.ghostMode)
                windowFrame.Background = null;
            else
                windowFrame.Background = System.Windows.Media.Brushes.White;
        }

        #endregion





        # region USER INTERFACE: DYNAMIC

        // Drag to move the window
        private void currentTaskLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch (Exception) { }                                       // Strange error occurs when not catching 
        }

        // Gradually loading MainWindow
        private void GradualEmerge()
        {
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800));
            da.AccelerationRatio = 1;
            this.BeginAnimation(OpacityProperty, da);
        }

        // Breathing arrow effect during PS
        private void ArrowBreathing()
        {
            DoubleAnimation da = new DoubleAnimation(0.3, 0.1, TimeSpan.FromMilliseconds(800));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            arrowSB.Duration = Duration.Forever;
            arrowSB.Children.Add(da);
            Storyboard.SetTarget(da, BackgroundArrow);
            Storyboard.SetTargetProperty(da, new PropertyPath(MainWindow.OpacityProperty));
            Timeline.SetDesiredFrameRate(arrowSB, 20);
            arrowSB.Begin(BackgroundArrow, HandoffBehavior.SnapshotAndReplace, true);
        }

        // Task name animation
        private void TaskNameAnimation()
        {
            Duration t = TimeSpan.FromMilliseconds(500);
            TimeSpan d = TimeSpan.FromMilliseconds(0);
            double start = 96;
            if (startup)
            {
                currentTaskLabel.Opacity = 0;
                t = TimeSpan.FromMilliseconds(2500);
                d = TimeSpan.FromMilliseconds(1000);
                start = 300;
            }

            DoubleAnimation fontSize = new DoubleAnimation(start, 48, t);
            BackEase fsEase = new BackEase();
            fsEase.EasingMode = EasingMode.EaseOut;
            fsEase.Amplitude = 0.15;
            fontSize.EasingFunction = fsEase;

            DoubleAnimation opa = new DoubleAnimation(0, 1, t);
            PowerEase opaEase = new PowerEase();
            opaEase.EasingMode = EasingMode.EaseOut;
            opaEase.Power = 4;
            opa.EasingFunction = opaEase;

            fontSize.BeginTime = d;
            opa.BeginTime = d;
            currentTaskLabel.BeginAnimation(FontSizeProperty, fontSize);
            currentTaskLabel.BeginAnimation(OpacityProperty, opa);
            startup = false;
        }

        // Tab content animation
        private void TabContentFadeEmerge()
        {
            DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            da.Completed += TabContentRefresh;
            o2oTabGrid.BeginAnimation(OpacityProperty, da);
        }
        private void TabContentRefresh(object sender, EventArgs e)
        {
            DisplayDirs();
            o2oTabGrid.Visibility = Visibility.Visible;
            TabContentEmerge();
        }
        private void TabContentEmerge()
        {
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800));
            o2oTabGrid.BeginAnimation(OpacityProperty, da);
        }

        // Expander behavior
        private void expanderTriggered(object sender, RoutedEventArgs e)
        {
            double end = shrinkHeight;
            if (expander1.IsExpanded)
                end = expandHeight;

            DoubleAnimation da = new DoubleAnimation(this.Height, end, 
                TimeSpan.FromMilliseconds(Math.Abs(end - this.Height)*2), FillBehavior.Stop);
            da.DecelerationRatio = 1;
            this.BeginAnimation(HeightProperty, da);
        }

        //Drag and Drop
        private void TextSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)sender;
            DragDrop.DoDragDrop(textbox, textbox.Text, System.Windows.DragDropEffects.Move);
        }
        private void TextTarget_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.Text))
                e.Effects = System.Windows.DragDropEffects.Copy;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }
        private void TextTarget_Drop(object sender, System.Windows.DragEventArgs e)
        {
            ((System.Windows.Controls.TextBox)sender).Text = (string)e.Data.GetData(System.Windows.DataFormats.Text);
        }


        // Do AutoTextBoxes whenever text changes
        private void AutoTextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Read directories for "One to One" mode
            string[] sourceTemp = new string[5];
            string[] targetTemp = new string[5];

            for (int i = 0; i < 5; i++)
            {
                sourceTemp[i] = o2oPathsTextBoxes[i].Text;
                targetTemp[i] = o2oPathsTextBoxes[i + 5].Text;
            }

            // Make everything visible
            o2oStackPanel1.Visibility = Visibility.Visible;
            o2oStackPanel2.Visibility = Visibility.Visible;
            o2oStackPanel3.Visibility = Visibility.Visible;
            o2oStackPanel4.Visibility = Visibility.Visible;
            o2oTabGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            o2oTabGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            o2oTabGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
            o2oTabGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);

            // Selectively make elements invisible
            if (sourceTemp[3] == "" & targetTemp[3] == "" &
                sourceTemp[4] == "" & targetTemp[4] == "")
            {
                o2oStackPanel4.Visibility = Visibility.Collapsed;
                o2oTabGrid.RowDefinitions[4].Height = new GridLength(0, GridUnitType.Star);

                if (sourceTemp[2] == "" & targetTemp[2] == "")
                {
                    o2oStackPanel3.Visibility = Visibility.Collapsed;
                    o2oTabGrid.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);
                    o2oTabGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);

                    if (sourceTemp[1] == "" & targetTemp[1] == "")
                    {
                        o2oStackPanel2.Visibility = Visibility.Collapsed;
                        o2oTabGrid.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Star);
                        o2oTabGrid.RowDefinitions[4].Height = new GridLength(1.5, GridUnitType.Star);

                        if ((sourceTemp[0] == "" & targetTemp[0] == "") |
                            (sourceTemp[0] == "Double click here to select a SOURCE" &
                            targetTemp[0] == "Double click here to select a TARGET"))
                        {
                            o2oStackPanel1.Visibility = Visibility.Collapsed;
                            o2oTabGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                            o2oTabGrid.RowDefinitions[4].Height = new GridLength(1, GridUnitType.Star);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
