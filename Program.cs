using System;
using System.Windows.Forms;

namespace CubiscanSerialDriver
{
    /// <summary>
    /// Application entry point. Bootstraps Windows Forms and shows the main window.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
