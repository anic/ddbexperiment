using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DistDBMS.UserInterface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmApp frmApp = new FrmApp();
            frmApp.NeedWizzard = !((args.Length > 0) && args[0] == "auto");
            Application.Run(frmApp);
        }
    }
}
