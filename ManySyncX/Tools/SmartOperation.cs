using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

namespace ManySyncX
{
    // Change file or folder attribute from ReadOnly to Archive
    class ChangeAttribute
    {
        public static void RO2Arc(string path)
        {
            if ((File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReadOnly))
                == (FileAttributes.Directory & FileAttributes.ReadOnly))
            {
                File.SetAttributes(path, FileAttributes.Archive | FileAttributes.Directory);
            }
            else if ((File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(path, FileAttributes.Archive);
            }
        }
    }

    class SmartOperation
    {
        private static void ComboFileDel(string path)
        {
            if (MainWindow.MWInstance.runningOneTask.delToBin)
            {
                File.Move(path, path);
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
                File.Delete(path);
        }

        // Delete a file carefree
        public static void DeleteFile(string path)
        {
            try
            {
                try
                {
                    ComboFileDel(path);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileDel.Add(path);
                    UItools.DelFile(path);
                }
                catch (UnauthorizedAccessException)
                {
                    ChangeAttribute.RO2Arc(path);
                    ComboFileDel(path);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileDel.Add(path);
                    UItools.DelFile(path);
                }
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
            }
            catch (Exception)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
            }
        }

        // Delete a folder tree carefree
        public static void DeleteFolder(string topFolder)
        {
            //// Delete all files inside
            //string[] fileRemovalList = FindAllChildren.RemovalList(true, topFolder);
            //foreach (string path in fileRemovalList)
            //{
            //    DeleteFile(path);
            //}

            //// Delete all folders (if possible)
            //DeleteEmptyTree(topFolder);
        }

        // Delete a directory tree layer by layer
        private static void DeleteEmptyTree(string tf)
        {
            string[] topFolder = { tf };

            string[] pathLayerN = topFolder;
            int numLayers = 0;

            while (true)
            {
                ArrayList pathLayerNplus1 = new ArrayList();

                foreach (string member in pathLayerN)                                   // Get all subfolders of layer N folders
                    pathLayerNplus1.AddRange(Directory.GetDirectories(member));

                if (pathLayerNplus1.Count != 0)                                         // If layer N is the bottom
                {
                    numLayers++;
                    pathLayerN = (string[])pathLayerNplus1.ToArray(typeof(string));
                }
                else break;
            }

            for (int i = numLayers; i >= 0; i--)
            {
                pathLayerN = topFolder;
                for (int j = i; j >= 0; j--)
                {
                    if (j != 0)
                    {
                        ArrayList pathLayerNplus1 = new ArrayList();
                        foreach (string member in pathLayerN)                           // Get all subfolders of layer N folders
                        {
                            pathLayerNplus1.AddRange(Directory.GetDirectories(member));
                        }
                        pathLayerN = (string[])pathLayerNplus1.ToArray(typeof(string));
                    }
                    else
                    {
                        foreach (string member in pathLayerN)
                            SingleFolderDel(member);                                    // Delete layer N folder per se
                    }
                }
            }
        }

        // Delete a single folder carefree (non-empty folder will be skiped)
        public static void SingleFolderDel(string path)
        {
            if ((SmartSearch.GetFiles(path).Length) == 0 & (SmartSearch.GetFiles(path).Length == 0))
            {
                try
                {
                    try
                    {
                        Directory.Delete(path);
                        MainWindow.MWInstance.runningOneTask.currentInventory.folderDel.Add(path);
                        UItools.DelFolder(path);
                    }
                    catch (IOException)
                    {
                        try
                        {
                            ChangeAttribute.RO2Arc(path);
                            Directory.Delete(path);
                            MainWindow.MWInstance.runningOneTask.currentInventory.folderDel.Add(path);
                            UItools.DelFolder(path);
                        }
                        catch (IOException)
                        {
                            if (Directory.GetDirectories(path).Equals(string.Empty))
                                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
                        }
                    }
                }
                catch (PathTooLongException)
                {
                    MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
                }
                catch (Exception)
                {
                    MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
                }
            }
        }


        // Copy a file carefree
        public static void CopyFile(string source, string sourceMirror)
        {
            try
            {
                File.Copy(source, sourceMirror);
                MainWindow.MWInstance.runningOneTask.currentInventory.fileCopyFrom.Add(source);
                MainWindow.MWInstance.runningOneTask.currentInventory.fileCopyTo.Add(sourceMirror);
                UItools.AddFile(sourceMirror);
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(source);
            }
            catch (Exception)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(source);
            }
        }


        // Update a file carefree
        public static void UpdateFile(string source, string sourceMirror)
        {
            try
            {
                try
                {
                    ComboFileDel(sourceMirror);
                    File.Copy(source, sourceMirror);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileUpdateFrom.Add(source);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileUpdateTo.Add(sourceMirror);
                    UItools.UpdateFile(sourceMirror);
                }
                catch (UnauthorizedAccessException)
                {
                    ChangeAttribute.RO2Arc(sourceMirror);
                    ComboFileDel(sourceMirror);
                    File.Copy(source, sourceMirror);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileUpdateFrom.Add(source);
                    MainWindow.MWInstance.runningOneTask.currentInventory.fileUpdateTo.Add(sourceMirror);
                    UItools.UpdateFile(sourceMirror);
                }
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(source);
            }
            catch (IOException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(sourceMirror);
            }
            catch (Exception)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(source);
            }
        }


        // Create folder carefree
        public static void CreateFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                MainWindow.MWInstance.runningOneTask.currentInventory.folderAdd.Add(path);
                UItools.AddFolder(path);
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
            }
            catch (Exception)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
            }
        }

    }





    static class SmartSearch
    {
        public static string[] GetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
                return new string[0];
            }
            catch (UnauthorizedAccessException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
                return new string[0];
            }
        }

        public static string[] GetFiles(string path)
        {
            try
            {
                //return Directory.GetFiles(path);
                ArrayList temp = new ArrayList();
                foreach (FileData f in FastDirectoryEnumerator.EnumerateFiles(path))
                {
                    temp.Add(f.Path);
                }
                return (string[])temp.ToArray(typeof(string));
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
                return new string[0];
            }
            catch (UnauthorizedAccessException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
                return new string[0];
            }
        }

        public static Hashtable GetFiles2(string path)
        {
            try
            {
                Hashtable table = new Hashtable();

                foreach (FileData f in FastDirectoryEnumerator.EnumerateFiles(path))
                    table.Add(f.Path, f);

                return table;
            }
            catch (PathTooLongException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.pathTooLong.Add(path);
                return new Hashtable();
            }
            catch (UnauthorizedAccessException)
            {
                MainWindow.MWInstance.runningOneTask.currentInventory.otherFailed.Add(path);
                return new Hashtable();
            }
        }

    }
}
