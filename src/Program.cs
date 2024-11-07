using CorticalExtract.Forms;
using CorticalExtract.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorticalExtract
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Controller c = new Controller();
            //c.SimpleTest();

            /*ClassificationBatchForm dlg = new ClassificationBatchForm();
            dlg.controller = c;
            Application.Run(dlg);*/

            BatchForm dlg = new BatchForm();
            dlg.controller = c;

            Application.Run(dlg);
        }
    }
}
