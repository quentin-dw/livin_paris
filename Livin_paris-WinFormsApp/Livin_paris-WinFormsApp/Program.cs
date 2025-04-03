using System;
using System.IO;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Livin_paris_WinFormsApp
{


    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AllocConsole(); // Ouvre la console
            Console.WriteLine("Console attachée !");

            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            new Graphe<int>(noeuds, arcs);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1("../../../../../noeuds.csv", "../../../../../arcs.csv"));
        }
    }
}