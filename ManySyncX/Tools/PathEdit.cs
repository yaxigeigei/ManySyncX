using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ManySyncX
{
    // Edit a given path
    class PathEdit
    {
        // Given a full path, return the file or folder name only
        public static string NameOnly(string path)
        {
            int lastIndex = path.LastIndexOf(Path.DirectorySeparatorChar);
            string name = path.Substring(lastIndex + 1);
            return name;
        }

        // Get the relative path of a folder (relative to source or target root)
        public static string PathMinusRoot(string path, string root)
        {
            string identifier;

            int rootLength = root.Length;
            if (rootLength <= path.Length)
            {
                identifier = path.Substring(rootLength);

                if (!identifier.StartsWith(@"\"))
                    identifier = @"\" + identifier;
            }
            else
                identifier = "hehe";

            return identifier;
        }

        // Path minus Name
        public static string PathMinusName(string path)
        {
            string name = NameOnly(path);
            return path.Substring(0, path.Length - name.Length);
        }

        // Name minus Extention
        public static string NameMinusExtention(string str)
        {
            string extention = Path.GetExtension(str);
            return str.Substring(0, str.Length - extention.Length);
        }


        // 
        public static string EditorOperation(Hashtable editorList, string source, string mirror)
        {
            if (!Screen.ExistInEditorList(source))
            {
                return mirror;
            }
            else{
                ArrayList preAndSuf = (ArrayList)editorList[source];
                string pre = (string)preAndSuf[0];
                string suf = (string)preAndSuf[1];

                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
                int counter = 0;

                DecomposePath dp = new DecomposePath(mirror);
                string sourceFileMirror = dp.dir + dp.nameWithoutExt;                           //...\FileName

                string wildcardName = dp.nameWithoutExt;
                if (pre != "None")
                    wildcardName = "*_" + wildcardName;
                if (suf != "None")
                    wildcardName = wildcardName + "_*";
                wildcardName += dp.extention;

                string[] files = Directory.GetFiles(dp.dir, wildcardName);


                if (NewerThanAll(source, files) && Newer(source, mirror))
                {
                    // Pre part
                    if (pre == "Date & Time")
                    {
                        sourceFileMirror = dp.dir + timeStamp + dp._name;                       // ...\1985-33-22 115599_FileName
                    }
                    else if (pre == "Counting Up")
                    {
                        foreach (string s in files)
                        {
                            string name = NameOnly(s);      // 1_FileName_1985-33-22 115501.txt
                            int _Index = name.IndexOf("_");
                            try
                            {
                                int num = Int32.Parse(name.Substring(0, _Index));
                                if (num > counter)
                                    counter = num;
                            }
                            catch (Exception) { }
                        }
                        counter++;
                        sourceFileMirror = dp.dir + counter + dp._name;                         // ...\2_FileName
                    }

                    // Suf part

                    if (suf == "Date & Time")
                    {
                        sourceFileMirror = sourceFileMirror + "_" + timeStamp + dp.extention;   // ...\123_FileName_1985-33-22 115599
                    }
                    else if (suf == "Counting Up")
                    {
                        foreach (string s in files)
                        {
                            string name = NameOnly(s);      // 1985-33-22 115501_FileName_1.txt
                            int _Index = name.LastIndexOf("_");
                            int dotIndex = name.LastIndexOf(".");
                            try
                            {
                                int num = Int32.Parse(name.Substring(_Index + 1, dotIndex - _Index - 1));
                                if (num > counter)
                                    counter = num;
                            }
                            catch (Exception) { }
                        }
                        counter++;
                        sourceFileMirror = sourceFileMirror + "_" + counter + dp.extention;     // ...\1985-33-22 115599_FileName_456
                    }
                    else
                    {
                        sourceFileMirror += dp.extention;
                    }

                    return sourceFileMirror;
                }
                else
                {
                    return "Pass";
                }
            }
        }

        private static bool NewerThanAll(string source, string[] files)
        {
            bool newerThanAll = true;
            foreach (string s in files)
            {
                if (!Newer(source, s))
                {
                    newerThanAll = false;
                }
            }
            return newerThanAll;
        }

        private static bool Newer(string source, string target)
        {
            bool newer = false;

            DateTime sfLastWriteTime = File.GetLastWriteTime(source);
            DateTime tfLastWriteTime = File.GetLastWriteTime(target);
            if (sfLastWriteTime.CompareTo(tfLastWriteTime) > 0)
                newer = true;

            return newer;
        }

        public static string[] EditorExclusion(Hashtable editorList, string path, string sRoot, string tRoot)
        {
            ArrayList preAndSuf = (ArrayList)editorList[path];
            string pre = (string)preAndSuf[0];
            string suf = (string)preAndSuf[1];

            DecomposePath dp = new DecomposePath(path);
            string wildcardName = "";
            if (pre != "None" & suf != "None")
            {
                wildcardName = "*_" + dp.nameWithoutExt + "_*" + dp.extention;                  // *_FileName_*.txt
            }
            else if (pre != "None")
            {
                wildcardName = "*_" + dp.nameWithExt;                                           // *_FileName.txt
            }
            else if (suf != "None")
            {
                wildcardName = dp.nameWithoutExt + "_*" + dp.extention;                         // FileName_*.txt
            }

            string mirrorDir;
            if (dp.dir.Length >= sRoot.Length)
            {
                mirrorDir = tRoot + PathMinusRoot(dp.dir, sRoot);
                if (Directory.Exists(mirrorDir))
                {
                    string[] list = Directory.GetFiles(mirrorDir, wildcardName);
                    for (int i = 0; i < list.Length; i++)
                    {
                        list[i] = Screen.Str2RegexStr(list[i]);
                    }
                    return list;
                }
                else
                    return new string[0];
            }
            else
            {
                return new string[0];
            }

        }




        // Substitute original folder paths (full path minus file/folder name) with new folder path
        public static string[] MirroringList(string[] sourceList, string mirDir)
        {
            mirDir = mirDir.TrimEnd('\\');
            int length = sourceList.Length;
            string[] mirror = new string[length];
            for (int i = 0; i < length; i++)
            {
                
                string name = PathEdit.NameOnly(sourceList[i]);
                mirror[i] = mirDir + Path.DirectorySeparatorChar + name;
            }
            return mirror;
        }


        // Find the union of two string arrays
        public static string[] CombineLists(string[] list1, string[] list2)
        {
            IEnumerable<string> dirUnion = list1.Union(list2);
            return dirUnion.ToArray();
        }

        // Find elements in the left, but not the right, string array
        public static string[] SubtractLists(string[] left, string[] right)
        {
            IEnumerable<string> dirSubtract = left.Except(right);
            return dirSubtract.ToArray();
        }

    }





    // Decompose a file path into individual parts
    struct DecomposePath
    {
        public string fullPath;
        public string nameWithoutExt;
        public string nameWithExt;
        public string _name;
        public string name_;
        public string extention;
        public string dir;

        public DecomposePath(string path)
        {
            fullPath = path;
            nameWithExt = PathEdit.NameOnly(path);
            nameWithoutExt = PathEdit.NameMinusExtention(nameWithExt);
            _name = "_" + nameWithoutExt;
            name_ = nameWithoutExt + "_";
            extention = Path.GetExtension(path);
            dir = PathEdit.PathMinusName(path);
        }
    }
}
