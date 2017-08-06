using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;


namespace ManySyncX
{
    [Serializable]
    public class OneTaskWPS
    {
        // Task Information
        public string taskName { get; set; }
        List<OnePairWPS> pairList = new List<OnePairWPS>();
        public DateTime syncDate { get; set; }
        public Inventory currentInventory = new Inventory();
        public Inventory lastInventory = new Inventory();

        // Auto Sync Settings
        public bool o2oWatch { get; set; }
        public string intervalOption { get; set; }

        // Advanced Settings
        public bool copyNew { get; set; }
        public bool replaceOld { get; set; }
        public bool deleteNon { get; set; }
        public bool delToBin { get; set; }
        public bool forceReplace { get; set; }
        public bool enableExclusion { get; set; }
        public bool enableEditor { get; set; }

        // Exclusion and Editor's List
        public ArrayList exclusionList = new ArrayList();
        public Hashtable editorList = new Hashtable();



        // Constructors
        public OneTaskWPS()
        {
            taskName = "ManySynX";
            string[] s = { "Double click here to select a SOURCE", "", "", "", "" };
            string[] t = { "Double click here to select a TARGET", "", "", "", "" };

            for (int i = 0; i < s.Length; i++)
                pairList.Add(new OnePairWPS(s[i], t[i]));

            syncDate = new DateTime();
            o2oWatch = false;
            intervalOption = "Every 30 minutes";

            copyNew = true;
            replaceOld = true;
            deleteNon = true;
            delToBin = false;
            forceReplace = false;
            enableExclusion = false;
            enableEditor = false;
        }



        // Get and set information

        // For displaying task name in listbox
        public override string ToString()
        {
            return taskName;
        }

        // Return a string array of source folders
        public string[] GetSourceList()
        {
            string[] s = new string[5];
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = pairList[i].sourceRootFolder;
            }
            return s;
        }

        // Return a string array of target folders
        public string[] GetTargetList()
        {
            string[] t = new string[5];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = pairList[i].targetRootFolder;
            }
            return t;
        }

        // Use string arrays to set OnePairWPS objects
        public void SetPairs(string[] sourceFolderList, string[] targetFolderList)
        {
            pairList = new List<OnePairWPS>();
            for (int i = 0; i < sourceFolderList.Length; i++)
                pairList.Add(new OnePairWPS(sourceFolderList[i], targetFolderList[i]));
        }

        // Examine how many pairs are valid (exist and not father-child)
        private int NumValidPairs()
        {
            int validPairs = 0;
            for (int i = 0; i < 5; i++)
            {
                if (pairList[i].IsExecutable())
                    validPairs++;
            }
            return validPairs;
        }






        // Start WPS
        public void Start()
        {
            try
            {
                MainWindow mw = MainWindow.MWInstance;

                // Find how many pairs are valid
                int validPairs = NumValidPairs();

                if (validPairs == 0) // Do nothing except for prompting the error (in non-watch mode)
                {
                    if (currentInventory.mode != "Watch")
                    {
                        mw.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                        {
                            mw.AfterWork();
                            System.Windows.MessageBox.Show(mw, "Please specify at least one pair of valid source and target folders");
                        });
                        UItools.ClearRecordList();
                    }
                }
                else // Start WPS
                {
                    // Begining Anouncement
                    if (currentInventory.mode == "Sync" || currentInventory.mode == "bSync")
                    {
                        UItools.PSPlainEntry("Start synchronizing '" + taskName + "'");
                    }
                    else if (currentInventory.mode == "Preview")
                    {
                        UItools.PSPlainEntry("Start previewing '" + taskName + "'");
                    }
                    else if (currentInventory.mode == "Watch")
                    {
                        mw.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                        {
                            mw.RecordListBox.Items.Add
                                ("Task '" + taskName + "' auto-sync at " + DateTime.Now.ToString("dd MMM HH:mm:ss"));
                        });
                    }


                    // Preview step of WPS
                    if (currentInventory.mode != "bSync")
                    {
                        // Previewing each pairs
                        foreach (OnePairWPS p in pairList)
                            p.Start(this);

                        currentInventory.Calculate();
                    }

                    // Check the eligibility of based-sync
                    if (currentInventory.mode == "Preview")
                        currentInventory.basedSyncAvailable = true;
                    else
                        currentInventory.basedSyncAvailable = false;

                    // Sync step of WS
                    if (currentInventory.mode != "Preview")
                    {
                        if (currentInventory.mode != "bSync")
                        {
                            // Tranfer the preview inventory
                            lastInventory = currentInventory;

                            // Reinitialize inventory based on display requirment
                            if (lastInventory.mode == "Watch")
                                currentInventory = new Inventory("Watch");
                            else if (lastInventory.mode == "Sync")
                                currentInventory = new Inventory("bSync");
                        }

                        // Sync based on preview result
                        SyncAsPreviewed();
                        currentInventory.Calculate();
                        syncDate = DateTime.Today;
                    }


                    // Ending Anouncement
                    string message = "";
                    if (currentInventory.mode == "Sync" || currentInventory.mode == "bSync")
                    {
                        message += "Synchronized " + validPairs + " pair(s) of folders.";
                        UItools.PSPlainEntry("Task Finished!\n");
                        UItools.PSPlainEntry(message);
                    }
                    else if (currentInventory.mode == "Preview")
                    {
                        message += "Previewed " + validPairs + " set(s) of synchronization.";
                        mw.isPreviewed = true;
                        UItools.PSPlainEntry("Task Finished!\n");
                        UItools.PSPlainEntry(message);
                    }
                    else if (currentInventory.mode == "Watch")
                    {
                        message = validPairs + " pairs of auto-sync done at " + DateTime.Now.ToString("dd MMM HH:mm:ss");
                        UItools.WatchEntry("    " + message);
                    }

                    // Restore MainWindow UI
                    if (currentInventory.mode != "Watch")
	                {
                        mw.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                        {
                            ResultWindow rw = new ResultWindow();
                            rw.Show();
                            rw.Activate();
                            mw.AfterWork();
                        });
                    }

                    // Transfer and clear current inventory
                    lastInventory = currentInventory;
                    currentInventory = new Inventory();
                }
            }
            catch (ThreadAbortException)
            {
                UItools.PSPlainEntry("Task aborted!");
            }
        }



        private void SyncAsPreviewed()
        {
            currentInventory.totalTargetFiles = CopyArrayList(lastInventory.totalTargetFiles);
            currentInventory.ignored = CopyArrayList(lastInventory.ignored);

            if (deleteNon)
            {
                foreach (string path in lastInventory.fileDel)
                    SmartOperation.DeleteFile(path);

                foreach (string path in lastInventory.folderDel)
                    SmartOperation.SingleFolderDel(path);
            }

            if (copyNew)
            {
                foreach (string path in lastInventory.folderAdd)
                    SmartOperation.CreateFolder(path);

                for (int i = 0; i < lastInventory.fileCopyFrom.Count; i++)
                    SmartOperation.CopyFile((string)lastInventory.fileCopyFrom[i], (string)lastInventory.fileCopyTo[i]);
            }

            if (replaceOld)
            {
                for (int i = 0; i < lastInventory.fileUpdateFrom.Count; i++)
                    SmartOperation.UpdateFile((string)lastInventory.fileUpdateFrom[i], (string)lastInventory.fileUpdateTo[i]);
            }
        }

        private ArrayList CopyArrayList(ArrayList template)
        {
            ArrayList transcript = new ArrayList();

            foreach (string s in template)
                transcript.Add(s);

            return transcript;
        }

    }
}
