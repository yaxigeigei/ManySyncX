using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ManySyncX
{
    // For finding similarity and difference between two folders
    // note: no exclusion applied during these operations (should be done beforehand if desired)
    class DirPairAnalyst
    {
        #region Properties

        // Root path of source and target folders
        public string sFatherDir;
        public string tFatherDir;


        // Full path of subfolders
        public string[] sSubDir;
        public string[] tSubDir;

        // Relative folder paths with respect to root path
        string[] sSubDirName;
        string[] tSubDirName;

        // Full path of subfolders shared by source and target
        public string[] sSubDirIntersect;
        public string[] tSubDirIntersect;

        // Full path new folders in source which are absent in target (i.e. to be added)
        public string[] dirAddList;
        public string[] dirAddListMirror;

        // Full path of folders in target which do not exist in source (i.e. to be deleted)
        public string[] dirDelList;


        // Full path (and mirrored path) of files
        public string[] sFiles;
        public string[] tFiles;
        public string[] sFilesMirror;
        public string[] tFilesMirror;

        #endregion


        // Constructor (for assigning above properties)
        public DirPairAnalyst(string sourceFatherDir, string targetFatherDir)
        {
            // Father folders
            sFatherDir = sourceFatherDir;
            tFatherDir = targetFatherDir;



            // Subdirecotries

            // All subdirectories
            sSubDir = SmartSearch.GetDirectories(sFatherDir);
            tSubDir = SmartSearch.GetDirectories(tFatherDir);

            // Exclude invalid subdirectories
            // identify exclusions in source and target, respectively, by matching regular expressions
            string[] sExcludedSubDir = Screen.GetExcludedPaths(sSubDir);
            string[] tExcludedSubDir = Screen.GetExcludedPaths(tSubDir);

            // get mirror versions of both
            string[] sExSubDirMirror = PathEdit.MirroringList(sExcludedSubDir, tFatherDir);
            string[] tExSubDirMirror = PathEdit.MirroringList(tExcludedSubDir, sFatherDir);

            // get unions of them
            string[] sExcludedSubDirUnion = PathEdit.CombineLists(sExcludedSubDir, tExSubDirMirror);
            string[] tExcludedSubDirUnion = PathEdit.CombineLists(tExcludedSubDir, sExSubDirMirror);

            // each side subtracts corresponding union
            sSubDir = PathEdit.SubtractLists(sSubDir, sExcludedSubDirUnion);       // valid subdirectories
            tSubDir = PathEdit.SubtractLists(tSubDir, tExcludedSubDirUnion);       // valid subdirectories
            

            // Get folder name of subdirectories for later use
            sSubDirName = new string[sSubDir.Length];
            tSubDirName = new string[tSubDir.Length];

            for (int i = 0; i < sSubDir.Length; i++)
                sSubDirName[i] = PathEdit.PathMinusRoot(sSubDir[i], sFatherDir);

            for (int i = 0; i < tSubDir.Length; i++)
                tSubDirName[i] = PathEdit.PathMinusRoot(tSubDir[i], tFatherDir);
            // alternatively
            //subDirNameIntersect = (string[])sSubDirName.Intersect(tSubDirName);


            // Find subfolders need to be added and added to
            Addition();

            // Find subfolders need to be removed from target
            Deletion();

            // Find subfolders shared by source and target
            Intersection();



            // Files

            // All files
            sFiles = SmartSearch.GetFiles(sFatherDir);
            tFiles = SmartSearch.GetFiles(tFatherDir);

            // Exclude invalid subdirectories
            // identify exclusions in source and target, respectively, by matching regular expressions
            string[] sExcludedFiles = Screen.GetExcludedPaths(sFiles);
            string[] tExcludedFiles = Screen.GetExcludedPaths(tFiles);

            // Exclude from full file arraies
            sFiles = PathEdit.SubtractLists(sFiles, sExcludedFiles);
            tFiles = PathEdit.SubtractLists(tFiles, tExcludedFiles);

            // Produce an array of file paths with mixed directory and file names
            sFilesMirror = PathEdit.MirroringList(sFiles, tFatherDir);
            tFilesMirror = PathEdit.MirroringList(tFiles, sFatherDir);

        }



        // Find new folders in source which are absent in target
        private void Addition()
        {
            ArrayList newFolders = new ArrayList();

            for (int i = 0; i < sSubDirName.Length; i++)
            {
                bool exist = false;

                for (int j = 0; j < tSubDirName.Length; j++)
                    if (sSubDirName[i] == tSubDirName[j])
                        exist = true;

                if (!exist)
                    newFolders.Add(sSubDir[i]);
            }

            dirAddList = (string[])newFolders.ToArray(typeof(string));
            dirAddListMirror = PathEdit.MirroringList(dirAddList, tFatherDir);
        }



        // Find folders in target which do not exist in source
        private void Deletion()
        {
            ArrayList deadFolders = new ArrayList();

            for (int i = 0; i < tSubDirName.Length; i++)
            {
                bool exist = false;

                for (int j = 0; j < sSubDirName.Length; j++)
                    if (tSubDirName[i] == sSubDirName[j])
                        exist = true;

                if (!exist)
                    deadFolders.Add(tSubDir[i]);
            }

            dirDelList = (string[])deadFolders.ToArray(typeof(string));
        }



        // Find folders shared by source and target
        private void Intersection()
        {
            ArrayList same = new ArrayList();
            ArrayList sameInTarget = new ArrayList();

            for (int i = 0; i < sSubDirName.Length; i++)
            {
                for (int j = 0; j < tSubDirName.Length; j++)
                {
                    if (sSubDirName[i].Equals(tSubDirName[j]))
                    {
                        same.Add(sSubDir[i]);
                        sameInTarget.Add(tSubDir[j]);
                    }
                }
            }

            sSubDirIntersect = (string[])same.ToArray(typeof(string));
            tSubDirIntersect = (string[])sameInTarget.ToArray(typeof(string));
        }
    }





    class FolderFinder
    {
        // Full path of subfolders
        string[] sourceFolders;
        string[] targetFolders;

        // Root path of source and target folders
        string sRoot;
        string tRoot;

        // Relative folder paths with respect to root path
        string[] sfRelaPath;
        string[] tfRelaPath;

        // Full path of subfolders shared by source and target
        public string[] sourceIntersection;
        public string[] targetIntersection;

        // Full path new folders in source which are absent in target (i.e. to be added)
        public string[] addition;

        // Full path of folders in target which do not exist in source (i.e. to be deleted)
        public string[] deletion;



        // Constructor (for assigning above properties)
        public FolderFinder(string[] ssf, string[] tsf, string sr, string tr)
        {
            // Full path of subfolders
            this.sourceFolders = ssf;
            this.targetFolders = tsf;
            
            // Root path of source and target folders
            this.sRoot = sr;
            this.tRoot = tr;

            // Relative folder paths with respect to root path (in a special case could be folder name)
            this.sfRelaPath = new string[sourceFolders.Length];
            this.tfRelaPath = new string[targetFolders.Length];

            for (int i = 0; i < sourceFolders.Length; i++)
                sfRelaPath[i] = PathEdit.PathMinusRoot(sourceFolders[i], sRoot);

            for (int i = 0; i < targetFolders.Length; i++)
                tfRelaPath[i] = PathEdit.PathMinusRoot(targetFolders[i], tRoot);
        }



        // Find folders shared by source and target
        public void Intersection()
        {
            ArrayList same = new ArrayList();
            ArrayList sameInTarget = new ArrayList();

            for (int i = 0; i < sfRelaPath.Length; i++)
            {
                for (int j = 0; j < tfRelaPath.Length; j++)
                {
                    if (sfRelaPath[i].Equals(tfRelaPath[j]))
                    {
                        same.Add(sourceFolders[i]);
                        sameInTarget.Add(targetFolders[j]);
                    }
                }
            }

            this.sourceIntersection = (string[])same.ToArray(typeof(string));
            this.targetIntersection = (string[])sameInTarget.ToArray(typeof(string));
        }

        // Find new folders in source which are absent in target
        public string[] Addition()
        {
            ArrayList newFolders = new ArrayList();

            for (int i = 0; i < sfRelaPath.Length; i++)
            {
                bool exist = false;

                for (int j = 0; j < tfRelaPath.Length; j++)
                    if (sfRelaPath[i] == tfRelaPath[j])
                        exist = true;

                if (!exist)
                    newFolders.Add(sourceFolders[i]);
            }

            this.addition = (string[])newFolders.ToArray(typeof(string));
            return addition;
        }

        // Find folders in target which do not exist in source
        public string[] Deletion()
        {
            ArrayList deadFolders = new ArrayList();

            for (int i = 0; i < tfRelaPath.Length; i++)
            {
                bool exist = false;

                for (int j = 0; j < sfRelaPath.Length; j++)
                    if (tfRelaPath[i] == sfRelaPath[j])
                        exist = true;

                if (!exist)
                    deadFolders.Add(targetFolders[i]);
            }

            this.deletion = (string[])deadFolders.ToArray(typeof(string));
            return deletion;
        }
    }





    enum FacMode { ADDITION, REMOVAL };
    
    class FindAllChildren
    {
        string sRoot;
        string tRoot;

        public string[] folderList;
        public string[] fileList;


        // Constructor
        public FindAllChildren(string sourceRootDir, string targetRootDir, string topFolder, FacMode fm)
        {
            sRoot = sourceRootDir;
            tRoot = targetRootDir;

            switch (fm)
            {
                case FacMode.ADDITION:
                    AdditionMirrorList(topFolder);
                    break;
                case FacMode.REMOVAL:
                    AllChildrenList(topFolder, tRoot);
                    break;
                default:
                    break;
            }
        }


        // Find all children with selective exclusion and mirror their paths
        private void AdditionMirrorList(string topFolder)
        {
            AllChildrenList(topFolder, sRoot);

            for (int i = 0; i < fileList.Length; i++)
                fileList[i] = tRoot + PathEdit.PathMinusRoot(fileList[i], sRoot);

            for (int i = 0; i < folderList.Length; i++)
                folderList[i] = tRoot + PathEdit.PathMinusRoot(folderList[i], sRoot);
        }


        // Find all children with selective exclusion
        private void AllChildrenList(string topFolder, string thisRoot)
        {
            // They will contain the overall result
            ArrayList folderArrayList = new ArrayList();
            ArrayList fileArrayList = new ArrayList();

            // Start gathering all files or folders layer by layer
            ArrayList folderLayer = new ArrayList();
            folderLayer.Add(topFolder);
            while (folderLayer.Count != 0)                                          // Stop when there is no folder layer any more
            {
                // Checkpoint
                ThreadManager.Check();
                
                ArrayList nextFolderLayer = new ArrayList();                        // It will contain all folders in layer N+1 (branching up)

                foreach (string thisFolder in folderLayer)                          // Loop through all members of layer N
                {
                    // Screen out excluded folders
                    string[] validSubFolders = Screen.GetValidPath(SmartSearch.GetDirectories(thisFolder));
                    folderArrayList.AddRange(validSubFolders);                      // Get subfolders of this folder member

                    // Screen out excluded files
                    string[] validFiles = Screen.GetValidPath(SmartSearch.GetFiles(thisFolder));
                    fileArrayList.AddRange(validFiles);                             // Get files of this folder member

                    // Collect valid subfolders for next layer
                    nextFolderLayer.AddRange(validSubFolders);
                }

                // Pass valid subfolders for next layer
                folderLayer = nextFolderLayer;
            }

            // Return the overall result
            folderArrayList.Reverse();                                              // Most children folders first, for a orderly deletion
            folderList = (string[])folderArrayList.ToArray(typeof(string));
            fileList = (string[])fileArrayList.ToArray(typeof(string));
        }

    }
}
