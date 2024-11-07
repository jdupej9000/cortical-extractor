using CorticalExtract.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace CorticalExtract.Forms
{
    public partial class BatchForm : Form
    {
        public BatchForm()
        {
            InitializeComponent();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            controller = new Controller();
        }


        string path = null;
        bool debugEach = false;
        bool debugMode = false;
        BackgroundWorker worker = new BackgroundWorker();
        public Controller controller;
        List<ExtractorSetup> batchItems = new List<ExtractorSetup>();



        private void btnBrowse_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

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
                sblStatus.Text = prog;
                prbProgress.Value = task - 1;
                Application.DoEvents();
            });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            controller.ExecuteScript(path, batchItems, debugEach | debugMode, OnProgress, debugMode);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                sblStatus.Text = "Ready.";
                prbProgress.Visible = false;
            });
        }

        private void LoadBatch(string path)
        {
            lstItems.Items.Clear();

            StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                ExtractorSetup es = new ExtractorSetup(line);
                ListViewItem lvi = lstItems.Items.Add(es.pathRaw);
                lvi.SubItems.Add(es.width.ToString());
                lvi.SubItems.Add(es.height.ToString());
                lvi.SubItems.Add(es.slices.ToString());
                lvi.SubItems.Add(es.format.ToString());
                lvi.SubItems.Add(string.Format("{0:##0.000}x{1:##0.000}x{2:##0.000}", es.voxDim[0], es.voxDim[1], es.voxDim[2]));
                lvi.Tag = es;
            }

            lstItems.Update();
        }

        private void Start()
        {
            if (batchItems.Count == 0)
            {
                MessageBox.Show("There are no items loaded.");
                return;
            }

            prbProgress.Visible = true;
            prbProgress.Minimum = 0;
            prbProgress.Maximum = batchItems.Count + 1;
            prbProgress.Value = 0;

            worker.RunWorkerAsync();
        }

        private void BatchForm_Load(object sender, EventArgs e)
        {
            propExtract.SelectedObject = controller.Config;
            prbProgress.Visible = false;

            lstItems.View = View.Details;
        }

        private void btnLoadBatch_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Comma-separated values (*.csv)|*.csv";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            path = dlg.FileName;
            LoadBatch(dlg.FileName);
            sblStatus.Text = "Loaded batch file " + Path.GetFileName(path);
        }

        private void btnEngage_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("A batch is already running.");
                return;
            }

            batchItems.Clear();
            foreach (ListViewItem lvi in lstItems.Items)
                batchItems.Add(lvi.Tag as ExtractorSetup);

            debugMode = false;
            Start();
        }

        private void btnDebugOne_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("A batch is already running.");
                return;
            }

            batchItems.Clear();
            foreach (ListViewItem lvi in lstItems.SelectedItems)
                batchItems.Add(lvi.Tag as ExtractorSetup);

            debugMode = true;
            Start();
        }
    }
}
