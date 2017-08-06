using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace ManySyncX
{
    [Serializable]
    public class Inventory
    {
        public string mode = "Watch";
        public bool calculated = false;
        public bool basedSyncAvailable = false;

        // Direct Lists
        public ArrayList totalTargetFiles = new ArrayList();
        public ArrayList folderAdd = new ArrayList();
        public ArrayList folderDel = new ArrayList();
        public ArrayList fileCopyFrom = new ArrayList();
        public ArrayList fileCopyTo = new ArrayList();
        public ArrayList fileDel = new ArrayList();
        public ArrayList fileUpdateFrom = new ArrayList();
        public ArrayList fileUpdateTo = new ArrayList();
        public ArrayList ignored = new ArrayList();

        public ArrayList pathTooLong = new ArrayList();
        public ArrayList otherFailed = new ArrayList();

        public ArrayList tempExclusionList = new ArrayList();

        // Derived Lists
        public ArrayList totalFailed = new ArrayList();
        public ArrayList fileUnchange = new ArrayList();


        
        // Constructors
        public Inventory()
        {
            // empty constructor
        }

        public Inventory(string mode)
        {
            this.mode = mode;
        }



        public void Calculate()
        {
            totalFailed = SummarizeFails();

            fileUnchange = ListSubtraction(totalTargetFiles, fileDel);
            fileUnchange = ListSubtraction(fileUnchange, fileUpdateTo);

            calculated = true;
        }

        private static ArrayList ListSubtraction(ArrayList a, ArrayList b)
        {
            ArrayList result = new ArrayList();
            foreach (string s in a)
            {
                if (!b.Contains(s))
                    result.Add(s);
            }
            return result;
        }

        private ArrayList SummarizeFails()
        {
            ArrayList result = new ArrayList();
            if (pathTooLong.Count != 0)
            {
                result.Add("The following items are not processed because the length of their path exceed the operation limits: \n");
                result.AddRange(pathTooLong);
                result.Add("");
            }
            if (otherFailed.Count != 0)
            {
                result.Add("The following items are not processed due to unknown reason (check whether they are being used): \n");
                result.AddRange(otherFailed);
            }

            return result;
        }
    }

}
