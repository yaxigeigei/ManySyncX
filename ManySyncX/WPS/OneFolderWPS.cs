using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Threading;

namespace ManySyncX
{
    class OneFolderWPS : DirPairAnalyst
    {

        // Constructor
        public OneFolderWPS(string sourceFolder, string targetFolder)
            : base(sourceFolder, targetFolder)
        {
            // anything extra
        }



        // 
        public void Start(OneTaskWPS ot)
        {
            // Delete folders from target which are absent in source
            for (int i = 0; i < dirDelList.Length; i++)
            {
                FindAllChildren fac = new FindAllChildren(sFatherDir, tFatherDir, dirDelList[i], FacMode.REMOVAL);

                foreach (string s in fac.fileList)
                {
                    UItools.DelFile(s);
                    ot.currentInventory.fileDel.Add(s);
                }

                foreach (string s in fac.folderList)
                {
                    UItools.DelFolder(s);
                    ot.currentInventory.folderDel.Add(s);
                }

                UItools.DelFolder(dirDelList[i]);
                ot.currentInventory.folderDel.Add(dirDelList[i]);
            }

            // Copy new folders from source to target
            for (int i = 0; i < dirAddListMirror.Length; i++)
            {
                UItools.AddFolder(dirAddListMirror[i]);
                ot.currentInventory.folderAdd.Add(dirAddListMirror[i]);

                FindAllChildren fac = new FindAllChildren(sFatherDir, tFatherDir, dirAddList[i], FacMode.ADDITION);

                foreach (string s in fac.folderList)
                {
                    UItools.AddFolder(s);
                    ot.currentInventory.folderAdd.Add(s);
                }

                foreach (string s in fac.fileList)
                {
                    UItools.AddFile(s);
                    ot.currentInventory.fileCopyFrom.Add(sFatherDir + PathEdit.PathMinusRoot(s, tFatherDir));
                    ot.currentInventory.fileCopyTo.Add(s);
                }
            }


            // Log total target files
            ot.currentInventory.totalTargetFiles.AddRange(tFiles);

            // Delete files from target which are absent in source
            for (int i = 0; i < tFilesMirror.Length; i++)
            {
                if (!sFiles.Contains(tFilesMirror[i]))
                {
                    UItools.DelFile(tFiles[i]);
                    ot.currentInventory.fileDel.Add(tFiles[i]);
                }
            }

            // Update or copy files from source to target
            for (int i = 0; i < sFilesMirror.Length; i++)
            {
                if (tFiles.Contains(sFilesMirror[i])) // Update files from source to target
                {
                    // Update when the source is newer
                    if (ShouldUpdate(sFiles[i], sFilesMirror[i]))
                    {
                        if (ot.enableEditor)
                        {
                            string sFileMirror = PathEdit.EditorOperation(ot.editorList, sFiles[i], sFilesMirror[i]);

                            // If not in editor's list, normal update (sFileMirror == sFilesMirror[i])
                            // Or in editor's list and newer than any existing versions, incremental update (sFileMirror == pre_sFilesMirror[i]_post.ext)
                            if (sFileMirror != "Pass")
                            {
                                UItools.UpdateFile(sFileMirror);
                                ot.currentInventory.fileUpdateFrom.Add(sFiles[i]);
                                ot.currentInventory.fileUpdateTo.Add(sFileMirror);
                            }
                        }
                        else
                        {
                            UItools.UpdateFile(sFilesMirror[i]);
                            ot.currentInventory.fileUpdateFrom.Add(sFiles[i]);
                            ot.currentInventory.fileUpdateTo.Add(sFilesMirror[i]);
                        }
                    }
                }
                else // Copy new files to target
                {
                    UItools.AddFile(sFilesMirror[i]);
                    ot.currentInventory.fileCopyFrom.Add(sFiles[i]);
                    ot.currentInventory.fileCopyTo.Add(sFilesMirror[i]);
                }
            }
        }


        private bool ShouldUpdate(string source, string target)
        {
            DateTime sfLastWriteTime = File.GetLastWriteTimeUtc(source);
            DateTime sfmLastWriteTime = File.GetLastWriteTimeUtc(target);

            if (MainWindow.MWInstance.selectedOneTask.forceReplace)
            {
                return sfLastWriteTime.CompareTo(sfmLastWriteTime) != 0;
            }
            else
            {
                return sfLastWriteTime.CompareTo(sfmLastWriteTime) > 0;
            }
        }

    }



}