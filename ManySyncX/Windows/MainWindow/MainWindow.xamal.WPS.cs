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

namespace ManySyncX
{
    public partial class MainWindow : Window
    {

        #region WATCHING AND AUTO-SYNC

        // Setting up timers (during app startup)
        private void InitializeWatch()
        {
            timers[0] = new DispatcherTimer(TimeSpan.FromSeconds(10),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
            timers[1] = new DispatcherTimer(TimeSpan.FromSeconds(30),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
            timers[2] = new DispatcherTimer(TimeSpan.FromMinutes(3),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
            timers[3] = new DispatcherTimer(TimeSpan.FromMinutes(10),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
            timers[4] = new DispatcherTimer(TimeSpan.FromMinutes(30),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
            timers[5] = new DispatcherTimer(TimeSpan.FromHours(3),
                DispatcherPriority.SystemIdle, WatchingDispatcher, this.Dispatcher);
        }

        // Dispatch watching to other threads
        // The sender is a specific timer
        private void WatchingDispatcher(object sender, EventArgs e)
        {
            RefreshWatchList();
            Thread t = new Thread(new ParameterizedThreadStart(Watching));
            t.Start(sender);
        }

        // Lists different intervals, each contains tasks having the same interval setting
        // Should be called whenever task settings are changed
        // Calling it will not affect the timers
        public void RefreshWatchList()
        {
            watchList.Clear();
            watchList.Add(SortInterval("Every 10 seconds"));
            watchList.Add(SortInterval("Every 30 seconds"));
            watchList.Add(SortInterval("Every 3 minutes"));
            watchList.Add(SortInterval("Every 10 minutes"));
            watchList.Add(SortInterval("Every 30 minutes"));
            watchList.Add(SortInterval("Every 3 hours"));
        }

        // Find tasks with the specified interval setting
        private ArrayList SortInterval(string interval)
        {
            ArrayList result = new ArrayList();
            foreach (OneTaskWPS t in tasksList)
            {
                if (t.o2oWatch && (t.intervalOption == interval))
                    result.Add(t);
            }
            return result;
        }

        // Dispatched
        private void Watching(object sender)
        {
            if (allSets.enableWatch && !isManualPS)
            {
                DispatcherTimer dt = (DispatcherTimer)sender;

                ArrayList tasks2run = new ArrayList();
                switch (dt.Interval.Seconds)
                {
                    case 10: tasks2run = (ArrayList)watchList[0]; break;
                    case 30: tasks2run = (ArrayList)watchList[1]; break;
                    default: break;
                }
                switch (dt.Interval.Minutes)
                {
                    case 3: tasks2run = (ArrayList)watchList[2]; break;
                    case 10: tasks2run = (ArrayList)watchList[3]; break;
                    case 30: tasks2run = (ArrayList)watchList[4]; break;
                    default: break;
                }
                if (dt.Interval.Hours == 3)
                    tasks2run = (ArrayList)watchList[5];

                if (tasks2run.Count != 0)
                {
                    foreach (OneTaskWPS t in tasks2run)
                    {
                        if (PSThread != null)
                            while (PSThread.ThreadState == ThreadState.Running)
                                Thread.Sleep(100);

                        pause = false; stop = false;                                                // Restore pause and stop flags
                        t.currentInventory.mode = "Watch";                                          // Set "UItools" flag
                        runningOneTask = t;

                        PSThread = new Thread(new ParameterizedThreadStart(ExecuteWPS));
                        PSThread.Start(runningOneTask);
                    }
                }
            }
        }

        // Auto-sync switch of a particular task
        private void Eye_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Eye.Opacity == 0.25)
                Eye.Opacity = 1;
            else
                Eye.Opacity = 0.25;
        }

        // Refresh tray context menu
        private void EnableWatchCheckbox_CheckChanged(object sender, RoutedEventArgs e)
        {
            GenerateContextMenu();
        }



        #endregion



        #region PREVIEW AND SYNCHRONIZATION


        // Common preparation before sync and preview
        private void PrepSyncPrev()
        {
            // Prevent auto-sync
            isManualPS = true;

            // Restore pause and stop flags
            pause = false; stop = false;

            // Read currentOTS for this sync or preview
            ReadSettings();

            // Corresponding UI changes
            Sync.Content = "Stop";
            Analyze.Content = "Pause";
            ExpRecord.IsEnabled = false;
            RecordListBox.Items.Clear();
            if (!expander1.IsExpanded)
                expander1.IsExpanded = true;
            ArrowBreathing();
        }

        // Re-enable Sync, Preview (Analysis) and Export buttons on UI
        public void AfterWork()
        {
            // Corresponding UI changes
            Sync.Content = "Synchronize";
            Analyze.Content = "Preview";
            Sync.IsEnabled = true;
            Analyze.IsEnabled = true;
            ExpRecord.IsEnabled = true;
            arrowSB.Stop(BackgroundArrow);

            if (runningOneTask.lastInventory.basedSyncAvailable)
            {
                basedSyncCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                basedSyncCheckBox.IsChecked = false;
                basedSyncCheckBox.Visibility = Visibility.Hidden;
            }

            // Restore the auto-sync flag
            isManualPS = false;
        }

        // Common portal of WPS
        static void ExecuteWPS(Object oneTask)
        {
            OneTaskWPS currentOT = (OneTaskWPS)oneTask;
            currentOT.Start();
        }

        // Start Preview
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            string buttonState = (string)Analyze.Content;
            if (buttonState == "Preview")
            {
                bool problem = false;
                if (PSThread != null)
                    if (PSThread.ThreadState == ThreadState.Running)
                    {
                        problem = true;
                        System.Windows.MessageBox.Show("Please waiting for the current background auto-sync to finish\n",
                            "Auto-sync running");
                    }

                if (!problem)
                {
                    PrepSyncPrev();
                    selectedOneTask.currentInventory.mode = "Preview";
                    runningOneTask = selectedOneTask;
                    PSThread = new Thread(new ParameterizedThreadStart(ExecuteWPS));
                    PSThread.Start(runningOneTask);
                }
            }
            else if (buttonState == "Pause")
            {
                pause = true;
                Sync.IsEnabled = false;
                Analyze.IsEnabled = false;
                AddListBoxEntry("Waiting for the task to pause ......");
            }
            else if (buttonState == "Resume")
            {
                pause = false;
                PSThread.Resume();
                Analyze.Content = "Pause";
                AddListBoxEntry("Task resumed");
            }
        }

        // Start Synchronization
        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            string buttonState = (string)Sync.Content;
            if (buttonState == "Synchronize")
            {
                bool problem = false;
                if (PSThread != null)
                    if (PSThread.ThreadState == ThreadState.Running)
                    {
                        problem = true;
                        System.Windows.MessageBox.Show("Please waiting for the current background auto-sync to finish\n",
                            "Auto-sync running");
                    }

                if (!problem)
                {
                    PrepSyncPrev();

                    if ((bool)basedSyncCheckBox.IsChecked)
                        selectedOneTask.currentInventory.mode = "bSync";
                    else
                        selectedOneTask.currentInventory.mode = "Sync";

                    if (!isPreviewed)
                    {
                        MessageBoxResult ans = MessageBox.Show("Don't you want to have a PREVIEW before sync?", "Consider this",MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
                        if (ans == MessageBoxResult.Yes)
                            selectedOneTask.currentInventory.mode = "Preview";
                    }

                    runningOneTask = selectedOneTask;
                    PSThread = new Thread(new ParameterizedThreadStart(ExecuteWPS));
                    PSThread.Start(runningOneTask);
                }
            }
            else if (buttonState == "Stop")
            {
                Sync.IsEnabled = false;
                Analyze.IsEnabled = false;
                AddListBoxEntry("Waiting for the task to terminate ......");
                stop = true;
                if (PSThread.ThreadState == ThreadState.Suspended)
                    PSThread.Resume();
                AfterWork();
            }
        }

        // See Summary
        private void Result_Click(object sender, RoutedEventArgs e)
        {
            if (resultWindow.IsLoaded)
                resultWindow.Close();

            resultWindow = new ResultWindow();
            resultWindow.Show();
        }

        // Add an entry into list box
        private void AddListBoxEntry(string str)
        {
            RecordListBox.Items.Add(str);
            RecordListBox.SelectedIndex = RecordListBox.Items.Count - 1;
            RecordListBox.ScrollIntoView(RecordListBox.SelectedItem);
        }

        // Clear Records
        private void ClearRecords_Click(object sender, RoutedEventArgs e)
        {
            RecordListBox.Items.Clear();
        }


        #endregion


    }
}
