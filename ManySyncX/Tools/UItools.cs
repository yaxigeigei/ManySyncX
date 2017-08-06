using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;

namespace ManySyncX
{
    class UItools
    {
        // Update MainWindow list box
        private static void UpdateRecordListBox(string str)
        {
            MainWindow mw = MainWindow.MWInstance;

            mw.RecordListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate(){
                    mw.RecordListBox.Items.Add(str);
                    mw.RecordListBox.SelectedIndex = mw.RecordListBox.Items.Count - 1;
                    mw.RecordListBox.ScrollIntoView(mw.RecordListBox.SelectedItem);
                });

            Thread.Sleep(3);                                                            // Give MainWindow some time to responce
        }

        // Add an entry into the RecordListBox (with a new thread)
        public static void AddFolder(string content)
        {
            ThreadManager.Check();

            string mode = MainWindow.MWInstance.runningOneTask.currentInventory.mode;
            string prefix = "";

            switch (mode)
            {
                case "Preview":
                    prefix = "Will Create Folder: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                case "bSync":
                    prefix = "Created Folder: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                default:
                    break;
            }
        }

        public static void DelFolder(string content)
        {
            ThreadManager.Check();

            string mode = MainWindow.MWInstance.runningOneTask.currentInventory.mode;
            string prefix = "";

            switch (mode)
            {
                case "Preview":
                    prefix = "Will Delete Folder: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                case "bSync":
                    prefix = "Deleted Folder: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                default:
                    break;
            }
        }

        public static void AddFile(string content)
        {
            ThreadManager.Check();

            string mode = MainWindow.MWInstance.runningOneTask.currentInventory.mode;
            string prefix = "";

            switch (mode)
            {
                case "Preview":
                    prefix = "Will Add File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                case "bSync":
                    prefix = "Copied File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                default:
                    break;
            }
        }

        public static void DelFile(string content)
        {
            ThreadManager.Check();

            string mode = MainWindow.MWInstance.runningOneTask.currentInventory.mode;
            string prefix = "";

            switch (mode)
            {
                case "Preview":
                    prefix = "Will Delete File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                case "bSync":
                    prefix = "Deleted File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                default:
                    break;
            }
        }

        public static void UpdateFile(string content)
        {
            ThreadManager.Check();

            string mode = MainWindow.MWInstance.runningOneTask.currentInventory.mode;
            string prefix = "";

            switch (mode)
            {
                case "Preview":
                    prefix = "Will Update File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                case "bSync":
                    prefix = "Updated File: ";
                    UpdateRecordListBox(prefix + content);
                    break;
                default:
                    break;
            }
        }

        public static void PSPlainEntry(string content)
        {
            if (!MainWindow.MWInstance.runningOneTask.currentInventory.mode.Equals("Watch"))
                UpdateRecordListBox(content);
        }

        public static void WatchEntry(string content)
        {
            if (MainWindow.MWInstance.runningOneTask.currentInventory.mode.Equals("Watch"))
                UpdateRecordListBox(content);
        }



        // Clear record listbox
        public static void ClearRecordList()
        {
            MainWindow.MWInstance.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate() { MainWindow.MWInstance.RecordListBox.Items.Clear(); });
        }

        // UI change when sync or preview paused
        public static void Paused()
        {
            MainWindow.MWInstance.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
            {
                MainWindow.MWInstance.Sync.IsEnabled = true;
                MainWindow.MWInstance.Analyze.IsEnabled = true;
                MainWindow.MWInstance.Analyze.Content = "Resume";
            });
        }

    }

    // List non-empty entry from the start of path list in one to one mode
    class Squeeze
    {
        public string[] sourceList = { "Double click here to select a SOURCE", "", "", "", "" };
        public string[] targetList = { "Double click here to select a TARGET", "", "", "", "" };

        public Squeeze(string[] s, string[] t)
        {
            int j = 0;
            for (int i = 0; i < 5; i++)
            {
                if ((s[i] != "") | (t[i] != ""))
                {
                    sourceList[j] = s[i];
                    targetList[j] = t[i];
                    j++;
                }
            }
        }
    }

}
