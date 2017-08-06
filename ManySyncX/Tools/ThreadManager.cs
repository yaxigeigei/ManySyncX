using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManySyncX
{
    static class ThreadManager
    {
        public static void Check()
        {
            if (MainWindow.MWInstance.pause)
            {
                UItools.PSPlainEntry("Task paused");
                UItools.Paused();
                Thread.CurrentThread.Suspend();
            }

            if (MainWindow.MWInstance.stop)
            {
                try { Thread.CurrentThread.Abort(); }
                catch (Exception) { }
            }
        }
    }
}
