using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ManySyncX
{

    // Screen file or folder list
    class Screen
    {

        private static string[] forceExclusionList = {
                                                         @"\b?\:\\System\sVolume\sInformation",
                                                         @"\b?\:\\$RECYCLE\.BIN",
                                                         @".*\.ini"
                                                     };


        // Test whether the string is a regular expression
        public static bool IsRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        // Convert a normal string to an regular expression
        public static string Str2RegexStr(string pattern)
        {
            // No need to convert < > | " / +

            // First, convert literal elements
            pattern = pattern.Replace(@"\", @"\\");     // \
            pattern = pattern.Replace(".", @"\.");      // .
            pattern = pattern.Replace(":", @"\:");      // :
            pattern = pattern.Replace(" ", @"\s");      // whitespace
            pattern = pattern.Replace("$", @"\$");      // $
            pattern = pattern.Replace("^", @"\^");      // ^
            pattern = pattern.Replace("%", @"\%:");     // %
            pattern = pattern.Replace("&", @"\&");      // &
            pattern = pattern.Replace("(", @"\(");      // (
            pattern = pattern.Replace(")", @"\)");      // )
            pattern = pattern.Replace("[", @"\[");      // [
            pattern = pattern.Replace("]", @"\]");      // ]
            pattern = pattern.Replace("{", @"\{");      // {
            pattern = pattern.Replace("}", @"\}");      // }
            pattern = pattern.Replace("'", @"\'");      // '
            pattern = pattern.Replace(";", @"\;");      // ;
            pattern = pattern.Replace(",", @"\,");      // ,
            pattern = pattern.Replace("#", @"\#");      // #
            pattern = pattern.Replace("=", @"\=");      // =

            // Second, convert wildcard elements
            pattern = pattern.Replace("?", ".");
            pattern = pattern.Replace("*", ".*");

            // Third, cap two ends
            pattern = @"^" + pattern + @"$";

            return pattern;
        }


        public static bool ExistInEditorList(string path)
        {
            bool exist = false;

            foreach (string s in MainWindow.MWInstance.runningOneTask.editorList.Keys)
                if (path == s)
                    exist = true;

            return exist;
        }



        internal static string[] GetExcludedPaths(string[] paths)
        {
            // Make a full exclusion list of regular expressions
            List<Regex> exclusionList = new List<Regex>();

            // Collect entries of forced exclusion
            foreach (string pattern in forceExclusionList)
                exclusionList.Add(new Regex(pattern));

            // Collect entries of custom exclusion
            if (MainWindow.MWInstance.runningOneTask.enableExclusion)
                foreach (string pattern in MainWindow.MWInstance.runningOneTask.exclusionList)
                    exclusionList.Add(new Regex(pattern));

            // Collect entries of exclusion due to Editor's List
            if (MainWindow.MWInstance.runningOneTask.enableEditor)
                foreach (string pattern in MainWindow.MWInstance.runningOneTask.currentInventory.tempExclusionList)
                    exclusionList.Add(new Regex(pattern));


            // Find excluded paths with full exclusion list by matching regular expressions
            ArrayList excludedPaths = new ArrayList();

            foreach (Regex re in exclusionList)
                foreach (string p in paths)
                    if (re.IsMatch(p))
                    {
                        excludedPaths.Add(p);
                        MainWindow.MWInstance.runningOneTask.currentInventory.ignored.Add(p);
                    }

            return (string[])excludedPaths.ToArray(typeof(string));
        }



        internal static string[] GetValidPath(string[] paths)
        {
            string[] paths2exclude = GetExcludedPaths(paths);
            return PathEdit.SubtractLists(paths, paths2exclude);
        }
    }
}
