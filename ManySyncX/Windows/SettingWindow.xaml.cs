using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ManySyncX
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {

        MainWindow mw = MainWindow.MWInstance;
        AllSettings mwAllSets = MainWindow.MWInstance.allSets;
        OneTaskWPS mwOT = MainWindow.MWInstance.selectedOneTask;
        ArrayList excludeList = new ArrayList();
        Hashtable editorUnitList = new Hashtable();

        
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            // Window
            GradualEmerge();
            
            // Preferences
            copyNewCheckBox.IsChecked = mwOT.copyNew;
            replaceOldCheckBox.IsChecked = mwOT.replaceOld;
            deleteNonCheckBox.IsChecked = mwOT.deleteNon;
            recycleCheckBox.IsChecked = mwOT.delToBin;

            enableEditorCheckbox.IsChecked = mwOT.enableEditor;
            enableExcludeCheckbox.IsChecked = mwOT.enableExclusion;

            forceReplaceCheckBox.IsChecked = mwOT.forceReplace;
            
            ghostCheckBox.IsChecked = mwAllSets.ghostMode;

            // Exclusion List
            foreach (string s in mwOT.exclusionList)
            {
                excludeList.Add(s);
                excludeListBox.Items.Add(s);
            }

            // Editor's List
            foreach (string s in mwOT.editorList.Keys)
            {
                editorUnitList.Add(s, mwOT.editorList[s]);
                editorListBox.Items.Add(s);
            }

            // Automatic Sync
            FreqComboBox.Items.Add("Every 10 seconds");
            FreqComboBox.Items.Add("Every 30 seconds");
            FreqComboBox.Items.Add("Every 3 minutes");
            FreqComboBox.Items.Add("Every 10 minutes");
            FreqComboBox.Items.Add("Every 30 minutes");
            FreqComboBox.Items.Add("Every 3 hours");
            FreqComboBox.SelectedItem = mwOT.intervalOption;

            // Calendar
            Calendar.SelectedDate = mwOT.syncDate;
        }

        private void Save()
        {
            // Task
            // Preferences
            mwOT.copyNew = (bool)copyNewCheckBox.IsChecked;
            mwOT.replaceOld = (bool)replaceOldCheckBox.IsChecked;
            mwOT.deleteNon = (bool)deleteNonCheckBox.IsChecked;
            mwOT.delToBin = (bool)recycleCheckBox.IsChecked;

            mwOT.enableEditor = (bool)enableEditorCheckbox.IsChecked;
            mwOT.enableExclusion = (bool)enableExcludeCheckbox.IsChecked;

            mwOT.forceReplace = (bool)forceReplaceCheckBox.IsChecked;

            // Exclusion List
            mwOT.exclusionList = excludeList;

            // Editor's List
            mwOT.editorList = editorUnitList;

            // Automatic Sync
            mwOT.intervalOption = (string)FreqComboBox.SelectedItem;
            

            // App
            mwAllSets.ghostMode = (bool)ghostCheckBox.IsChecked;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            this.Close();
        }



        private void prefDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            copyNewCheckBox.IsChecked = true;
            replaceOldCheckBox.IsChecked = true;
            deleteNonCheckBox.IsChecked = true;
            recycleCheckBox.IsChecked = false;

            enableEditorCheckbox.IsChecked = false;
            enableExcludeCheckbox.IsChecked = false;

            ghostCheckBox.IsChecked = false;

            FreqComboBox.SelectedItem = "Every 30 minutes";
        }




        #region EXCLUSION LIST

        private void excludeFileTextBox_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Multiselect = false;
            opf.Title = "Select a file or folder";
            bool? ok = opf.ShowDialog(this);

            if (ok == true)
            {
                excludeFileTextBox.Text = opf.FileName;
            }
        }

        private void excludeFolderTextBox_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            string path = fbDialog();
            if (path != "") { excludeFolderTextBox.Text = path; }
        }

        public string fbDialog()
        {
            System.Windows.Forms.FolderBrowserDialog b = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = b.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
                return "";

            return b.SelectedPath.Trim();
        }

        private void excludeAddButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the user input string
            Button b = (Button)sender;
            string path;

            if (b.Name == "excludeAddFileButton")
                path = excludeFileTextBox.Text;
            else
                path = excludeFolderTextBox.Text;

            if (!String.IsNullOrEmpty(path))
            {
                // Test if it is a regular expression, otherwise convert it to RE
                if (!Screen.IsRegex(path))
                    path = Screen.Str2RegexStr(path);

                // Check for duplication
                bool duplicated = false;

                foreach (string s in excludeList)
                    if (path == s)
                        duplicated = true;

                if (!duplicated)
                {
                    excludeList.Add(path);
                    excludeListBox.Items.Add(path);
                }
                else
                    System.Windows.MessageBox.Show("It has already existed in the list", "Conflict");
            }
        }

        private void excludeRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (excludeListBox.SelectedIndex != -1)
            {
                excludeList.Remove((string)excludeListBox.SelectedItem);
                excludeListBox.Items.Remove(excludeListBox.SelectedItem);
            }
        }

        #endregion






        #region EDITOR'S LIST

        private void editorPathTextBox_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Multiselect = false;
            opf.Title = "Select a file";
            bool? ok = opf.ShowDialog(this);

            if (ok == true)
            {
                editorPathTextBox.Text = opf.FileName;
            }
        }


        private void editorAddButton_Click(object sender, RoutedEventArgs e)
        {
            string path = editorPathTextBox.Text;

            if (File.Exists(path))
            {
                bool duplicated = false;

                foreach (string s in editorUnitList.Keys)
                    if (path == s)
                        duplicated = true;

                if (!duplicated)
                {
                    string prefix = (string)prefixComboBox.SelectionBoxItem;
                    string suffix = (string)suffixComboBox.SelectionBoxItem;

                    ArrayList preAndSuf = new ArrayList();
                    preAndSuf.Add(prefix);
                    preAndSuf.Add(suffix);

                    if (prefix != "None" | suffix != "None")
                    {
                        editorUnitList.Add(path, preAndSuf);
                        editorListBox.Items.Add(path);
                    }
                    else
                        System.Windows.MessageBox.Show
                            ("Please specify at least one tag, prefix or suffix.", "Missing");
                }
                else
                    System.Windows.MessageBox.Show("This file has already existed in the list", "Conflict");
            }
            else
            {
                System.Windows.MessageBox.Show("This is not a valid file path", "Invalid File");
            }
        }

        private void editorRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editorListBox.SelectedIndex != -1)
            {
                editorUnitList.Remove((string)editorListBox.SelectedItem);
                editorListBox.Items.Remove(editorListBox.SelectedItem);
            }
        }

        private void editorListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (editorListBox.SelectedIndex != -1)
            {
                string key = (string)editorListBox.SelectedItem;
                DecomposePath dp = new DecomposePath(key);
                string fileName = dp.nameWithoutExt;

                ArrayList preAndSuf = (ArrayList)editorUnitList[key];
                string pre = (string)preAndSuf[0] + "__";
                string suf = "__" + (string)preAndSuf[1];
                
                if (pre == "None__")
                    pre = "";

                if (suf == "__None")
                    suf = "";

                PreviewLabel.Content = pre + fileName + suf + dp.extention;
            }
        }


        #endregion






        private void GradualEmerge()
        {
            DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
            da.AccelerationRatio = 1;
            this.BeginAnimation(OpacityProperty, da);
        }

        private void Window_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch (Exception) { }                                       // Strange error occurs when not catching 
        }




    }
}
