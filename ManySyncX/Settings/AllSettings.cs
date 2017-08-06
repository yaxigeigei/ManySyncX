using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ManySyncX
{

    [Serializable]
    public class AllSettings
    {
        // Mainwindow
        public List<OneTaskWPS> tasksList = new List<OneTaskWPS>();
        public int lastimeTaskIndex { get; set; }
        public bool enableWatch { get; set; }
        public bool ghostMode { get; set; }

        public AllSettings()
        {
            lastimeTaskIndex = 0;
            enableWatch = false;
            ghostMode = false;
        }
    }



    [ValueConversion(typeof(bool), typeof(double))]
    public class EyeBooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return 1;
            else
                return 0.25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value == 1)
                return true;
            else
                return false;
        }
    }


    class SaveAndLoad
    {
        public string settingsFilePath;

        public SaveAndLoad()
        {
            // Create a folder in My Document for saving settings
            string settingsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                Path.DirectorySeparatorChar + "ManySyncX";
            if (!Directory.Exists(settingsFolderPath)) { Directory.CreateDirectory(settingsFolderPath); }

            // Get the path of settings file
            settingsFilePath = settingsFolderPath + Path.DirectorySeparatorChar + "SettingsV32.bin";
        }

        public void SaveAll(AllSettings alset)
        {
            BFormatter.save(settingsFilePath, alset);
        }

        public object LoadAll()
        {
            return BFormatter.load(settingsFilePath);
        }
    }



    class BFormatter
    {
        /// <summary>
        /// BinaryFormatter序列化方式
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="obj"></param>
        public static void save(string filepath, object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream fs = null;
            try
            {
                fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write);
                formatter.Serialize(fs, obj);
            }
            catch (Exception e) { throw e; }
            finally
            { if (fs != null) { fs.Close(); } }
        }


        public static object load(string filepath)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream fs = null;
            try
            {
                fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return formatter.Deserialize(fs);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally { if (fs != null) { fs.Close(); } }
        }
    }
}
