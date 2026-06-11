using System;
using System.Windows.Forms;

namespace WinFormsApp5
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppPaths.EnsureImagesFolder();
            Application.Run(new LoginForm());
        }
    }
}
