using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManySyncX
{
    [Serializable]
    class OnePairWPS
    {
        public string sourceRootFolder { get; set; }
        public string targetRootFolder { get; set; }
        


        // Constructor
        public OnePairWPS(string s, string t)
        {
            sourceRootFolder = s;
            targetRootFolder = t;
        }
        


        // Utilities
        public bool IsSourceExist()
        {
            return Directory.Exists(sourceRootFolder);
        }

        public bool IsTargetExist()
        {
            return Directory.Exists(targetRootFolder);
        }

        public bool IsFatherChild()
        {
            bool isFatherChild = false;

            if (sourceRootFolder.Length >= targetRootFolder.Length)
                isFatherChild = sourceRootFolder.Contains(targetRootFolder) && sourceRootFolder != targetRootFolder;
            else
                isFatherChild = targetRootFolder.Contains(sourceRootFolder) && sourceRootFolder != targetRootFolder;

            return isFatherChild;
        }

        public bool IsExecutable()
        {
            return IsSourceExist() && IsTargetExist() && !IsFatherChild();
        }




        // One pair WPS
        public bool Start(OneTaskWPS otInstance)
        {
            bool executable = IsExecutable();

            if (executable)
            {
                string[] sFatherFolders = { sourceRootFolder };
                string[] tFatherFolders = { targetRootFolder };

                // Generate the editor's exclusion list
                if (otInstance.enableEditor)
                    foreach (string path in otInstance.editorList.Keys)
                        otInstance.currentInventory.tempExclusionList.AddRange(PathEdit.EditorExclusion(otInstance.editorList, path, sourceRootFolder, targetRootFolder));


                // Start doing it
                while (sFatherFolders.Length != 0)
                {
                    ArrayList sSubFolders = new ArrayList();
                    ArrayList tSubFolders = new ArrayList();

                    for (int i = 0; i < sFatherFolders.Length; i++)
                    {
                        // Analyze a pair of folders
                        // folders to add and remove; files to add, remove and update
                        OneFolderWPS of = new OneFolderWPS(sFatherFolders[i], tFatherFolders[i]);
                        of.Start(otInstance);

                        // Prepare subfolder pairs for next layer
                        sSubFolders.AddRange(of.sSubDirIntersect);
                        tSubFolders.AddRange(of.tSubDirIntersect);
                    }

                    sFatherFolders = (string[])sSubFolders.ToArray(typeof(string));
                    tFatherFolders = (string[])tSubFolders.ToArray(typeof(string));
                }
            }

            return executable;
        }

    }
}
