using LibraryApp;
using System;
using System.Windows.Forms;

namespace LibreryApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Замінили MainForm на Form1
        }
    }
}
