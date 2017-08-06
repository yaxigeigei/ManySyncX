using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManySyncX
{

    public class StartUp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceApplicationWrapper wrapper = new SingleInstanceApplicationWrapper();
            wrapper.Run(args);
        }
    }
    
    public class SingleInstanceApplicationWrapper :
    Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        private WpfApp app;

        public SingleInstanceApplicationWrapper()
        {
            this.IsSingleInstance = true;                       // Enable single instance mode
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            app = new WpfApp();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run();

            return false;
        }

        protected override void OnStartupNextInstance
            (Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e)
        {
            MainWindow.MWInstance.Show();
            MainWindow.MWInstance.WindowState = WindowState.Normal;
        }
    }

    public class WpfApp : Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load the main window
            MainWindow mw = new MainWindow();
            this.MainWindow = mw;
            mw.Show();
        }
    }


}
