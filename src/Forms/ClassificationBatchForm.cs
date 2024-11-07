using CorticalExtract.Processing;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace CorticalExtract.Forms
{
    public partial class ClassificationBatchForm : Form
    {
        public ClassificationBatchForm()
        {
            InitializeComponent();
            controller = new Controller();
            worker.DoWork += worker_DoWork;
        }

        string path = null;
        string pathDest = null;
        bool debugEach = false;
        BackgroundWorker worker = new BackgroundWorker();
        public Controller controller;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Comma-separated values (*.csv)|*.csv";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            path = dlg.FileName;
            lblFileName.Text = Path.GetFileName(path);
        }

        private void btnBrowseDest_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Comma-separated values (*.csv)|*.csv";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            pathDest = dlg.FileName;
            lblDestFile.Text = Path.GetFileName(pathDest);
        }


        void OnProgress(int task, int numTasks, string comment)
        {
            string prog = "Idle.";

            if (numTasks == 0)
            {
                prog = comment;
            }
            else
            {
                prog = string.Format("[{0} of {1}] {2}", task, numTasks, comment);
            }

            Invoke((MethodInvoker)delegate
            {
                lblProgress.Text = prog;
                Application.DoEvents();
            });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            controller.ExecuteClassificationScript(path, pathDest, debugEach, OnProgress);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            debugEach = chkDebugEach.Checked;
            worker.RunWorkerAsync();
        }
    }
}
