using System;
using System.Collections;
using System.Collections.Generic;
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

namespace ManySyncX
{

    public partial class MainWindow : Window
    {

        #region PARAMETERS


        // User interface parameters
        public static MainWindow MWInstance;                            // Create a referrence of the MainWindow (MainWindowInstance)
        const double shrinkHeight = 360;                                // Height of window after shrinking
        const double expandHeight = 500;                                // Height of window after expansion
        bool startup = true;                                            // For differentially displaying some animation on startup
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon(); // System tray icon object
        List<TextBox> o2oPathsTextBoxes = new List<TextBox>();          // Enumerable container of 10 text boxes
        ResultWindow resultWindow = new ResultWindow();                 // Only one summary window instance is allowed

        // Settings
        SaveAndLoad saveAndLoad = new SaveAndLoad();                    // SaveAndLoad provides save and load method for AllSetting object
        public AllSettings allSets = new AllSettings();                 // This object stores "everyting" 
        List<OneTaskWPS> tasksList = new List<OneTaskWPS>();            // Stores TaskWPS objects
        public OneTaskWPS selectedOneTask = new OneTaskWPS();               // The current selected OneTaskWPS object
        public OneTaskWPS runningOneTask = new OneTaskWPS();               // The current running OneTaskWPS object

        // Threading control
        DispatcherTimer[] timers = new DispatcherTimer[6];
        ArrayList watchList = new ArrayList();                          // List of auto sync
        public bool isManualPS = false;                                 // Prevent watching while preview or sync manually (double check besides ThreadState)
        Thread PSThread;                                                // The thread executing preview and synchronization job
        public bool pause = false;
        public bool stop = false;

        // Other parameters
        public bool isPreviewed = false;                                // Indicate whether a certain task has been previewed
        Storyboard arrowSB = new Storyboard();

        #endregion



        #region INITIALIZATION

        public MainWindow()
        {
            InitializeComponent();
            // Provide a handle for classes outside MainWindow to make UI change
            // Equivalent to: MainWindowInstance = (MainWindow)App.Current.MainWindow;
            MWInstance = this;
        }

        // Set UI and load settings during startup
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = (SystemParameters.FullPrimaryScreenHeight - this.Height) / 3;        // Screen positioning
            GradualEmerge();                                                                // Window content emerging animation
            LoadAllSets();                                                                  // Load all user settings including all saved tasks and preferences

            DefineTrayIcon();

            o2oPathsTextBoxes.Add(sourceDirText1); o2oPathsTextBoxes.Add(sourceDirText2);
            o2oPathsTextBoxes.Add(sourceDirText3); o2oPathsTextBoxes.Add(sourceDirText4);
            o2oPathsTextBoxes.Add(sourceDirText5); 
            
            o2oPathsTextBoxes.Add(targetDirText1); o2oPathsTextBoxes.Add(targetDirText2);
            o2oPathsTextBoxes.Add(targetDirText3); o2oPathsTextBoxes.Add(targetDirText4);
            o2oPathsTextBoxes.Add(targetDirText5);
        }

        // Load all user settings including saved tasks and preferences
        private void LoadAllSets()
        {
            // When the saved "settingsV*.bin" file exists in "...\我的文档\ManySyncX" folder
            if (File.Exists(saveAndLoad.settingsFilePath))
            {
                allSets = (AllSettings)saveAndLoad.LoadAll();           // Import AllSettings object containing a hashtable and others settings
                tasksList = allSets.tasksList;                          // Assign the task list to the pointer in MainWindow class

                // If the task list contains no existing task, create one and make it the active(current) one
                if (tasksList.Count == 0)
                    tasksList.Add(selectedOneTask);                         // Add this new task into task list
                // If previous task exists, make the first task the active one
                else
                    selectedOneTask = (OneTaskWPS)tasksList[0];             // Using currentName as the key to retrive currentOTS from Hashtable

                foreach (OneTaskWPS ot in tasksList)
                {
                    TasksList.Items.Add(ot);
                }
            }
            // When the saved "settings.bin" file does not exist
            else
            {
                allSets.tasksList = tasksList;
                tasksList.Add(selectedOneTask);
                TasksList.Items.Add(selectedOneTask);
            }

            // Set timers for watching (auto-sync)
            LeftSideStackPanel.DataContext = allSets;                   // EnableWatchCheckbox
            InitializeWatch();

            // Display settings
            TasksList.SelectedIndex = allSets.lastimeTaskIndex;         // display settings via the SelectionChange callback
            if (allSets.ghostMode)
                windowFrame.Background = null;
        }

        #endregion



        #region READ, DISPLAY AND SAVE

        // Read directories from UI to currentOTS
        private void ReadSettings()
        {
            // Read directories
            string[] sourceTemp = new string[5];
            string[] targetTemp = new string[5];

            for (int i = 0; i < 5; i++)
            {
                sourceTemp[i] = o2oPathsTextBoxes[i].Text;
                targetTemp[i] = o2oPathsTextBoxes[i + 5].Text;
            }

            Squeeze sqz = new Squeeze(sourceTemp, targetTemp);                  // Rearrage source-target pairs so that blank pairs in between are removed
            selectedOneTask.SetPairs(sqz.sourceList, sqz.targetList);               // Put rearranged result into currentTask

            // Read selected task index
            allSets.lastimeTaskIndex = TasksList.SelectedIndex;
        }

        // Display the currentTask onto UI with animations
        private void DisplaySettings()
        {
            currentTaskLabel.Content = selectedOneTask.taskName;
            TaskNameAnimation();

            isPreviewed = false;                                                // Reset Preview state
            basedSyncCheckBox.IsChecked = false;
            basedSyncCheckBox.Visibility = Visibility.Hidden;

            TabContentFadeEmerge();
        }

        private void DisplayDirs()
        {
            this.DataContext = selectedOneTask;

            string[] s = selectedOneTask.GetSourceList();
            string[] t = selectedOneTask.GetTargetList();

            for (int i = 0; i < 5; i++)
            {
                o2oPathsTextBoxes[i].Text = s[i];
                o2oPathsTextBoxes[i + 5].Text = t[i];
            }

            TaskListCoverLabel.Visibility = Visibility.Collapsed;
        }


        // Save AllSettings (everything) to .../我的文档/ManySyncX/settings.bin file
        private void SaveSettings()
        {
            // Because currentOTS, tasksSetTable and other preferences are passed by reference
            // and their values share same physical address. Wherever I change any setting, 
            // the value changes physically. Thus, when I can save allSets, everything is saved.
            saveAndLoad.SaveAll(allSets);
        }

        #endregion



        #region TASK MANAGEMENT


        // Change selected task
        private void TasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int taskIndex = TasksList.SelectedIndex;
            if (taskIndex != -1)                                            // Make sure there is a selection in task listbox
            {
                TaskListCoverLabel.Visibility = Visibility.Visible;         // Prevent selection change by mouse

                if (!startup)                                               // No reading and saving in the first display after startup
                {
                    ReadSettings();                                         // Read settings change, if any, from UI
                    SaveSettings();                                         // Save potentially changed settings
                }

                selectedOneTask = (OneTaskWPS)tasksList[taskIndex];             // Make currentOTS the one to which the currentName specifies in Hashtable
                DisplaySettings();
            }
        }

        // Create a new task
        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            // Ask user for a name
            string name;
            TextInputDialog nd = new TextInputDialog();
            nd.Owner = this;
            nd.ShowDialog();                                                // Open the "RenameDialog" window
            name = nd.name;                                                 // Get the input name

            if (name != null)
            {
                tasksList.Add(new OneTaskWPS());                            // Use this name as key to create a new OneTaskSettings object in taskSetTable
                tasksList[tasksList.Count() - 1].taskName = name;
                TasksList.Items.Add(tasksList[tasksList.Count() - 1]);      // Update UI
                TasksList.SelectedIndex = TasksList.Items.Count - 1;        // Select it by selecting the last item in listbox
            }
        }

        // Remove a existing task
        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksList.Items.Count > 1)
            {
                int taskIndex = TasksList.SelectedIndex;
                if (taskIndex != -1)                                            // Make sure there is a selection in task listbox
                {
                    tasksList.RemoveAt(taskIndex);                              // Use this key to search and remove corresponding entry in task Hashtable
                    TasksList.Items.RemoveAt(taskIndex);                        // Update listbox
                    TasksList.SelectedIndex = 0;                                // Select the first task to refresh display
                }
                else
                {
                    MessageBox.Show("Please select the task you want to remove", "No task selected");
                }
            }
            else
            {
                MessageBox.Show("This is the last task you have", "Cannot Remove");
            }
        }

        // Rename task
        private void RenameTask_Click(object sender, RoutedEventArgs e)
        {
            int taskIndex = TasksList.SelectedIndex;
            if (taskIndex != -1)                                            // Make sure there is a selection in task listbox
            {
                string name;
                TextInputDialog rd = new TextInputDialog();
                rd.Owner = this;
                rd.ShowDialog();
                name = rd.name;

                if (name != null)
                {
                    selectedOneTask.taskName = name;
                    TasksList.Items.RemoveAt(taskIndex);
                    TasksList.Items.Insert(taskIndex, selectedOneTask);
                    TasksList.SelectedIndex = taskIndex;
                }
            }
            else
            {
                MessageBox.Show("Please select the task you want to rename", "No task selected");
            }
        }

        #endregion

    }

}
