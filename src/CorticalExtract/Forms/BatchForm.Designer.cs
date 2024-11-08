namespace CorticalExtract.Forms
{
    partial class BatchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchForm));
            this.propExtract = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnLoadBatch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnEngage = new System.Windows.Forms.ToolStripButton();
            this.sblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.prbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnDebugOne = new System.Windows.Forms.ToolStripButton();
            this.lstItems = new System.Windows.Forms.ListView();
            this.chFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chHeight = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWidth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSlices = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chVoxelDim = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propExtract
            // 
            this.propExtract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propExtract.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propExtract.Location = new System.Drawing.Point(0, 0);
            this.propExtract.Name = "propExtract";
            this.propExtract.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propExtract.Size = new System.Drawing.Size(345, 702);
            this.propExtract.TabIndex = 8;
            this.propExtract.ToolbarVisible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadBatch,
            this.toolStripSeparator1,
            this.btnEngage,
            this.btnDebugOne});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 5, 1, 5);
            this.toolStrip1.Size = new System.Drawing.Size(1289, 37);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sblStatus,
            this.prbProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 739);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1289, 25);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnLoadBatch
            // 
            this.btnLoadBatch.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadBatch.Image")));
            this.btnLoadBatch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadBatch.Name = "btnLoadBatch";
            this.btnLoadBatch.Size = new System.Drawing.Size(116, 24);
            this.btnLoadBatch.Text = "Load batch...";
            this.btnLoadBatch.Click += new System.EventHandler(this.btnLoadBatch_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnEngage
            // 
            this.btnEngage.Image = ((System.Drawing.Image)(resources.GetObject("btnEngage.Image")));
            this.btnEngage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEngage.Name = "btnEngage";
            this.btnEngage.Size = new System.Drawing.Size(87, 24);
            this.btnEngage.Text = "Engage!";
            this.btnEngage.Click += new System.EventHandler(this.btnEngage_Click);
            // 
            // sblStatus
            // 
            this.sblStatus.Name = "sblStatus";
            this.sblStatus.Size = new System.Drawing.Size(871, 20);
            this.sblStatus.Spring = true;
            this.sblStatus.Text = "Ready.";
            this.sblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // prbProgress
            // 
            this.prbProgress.Name = "prbProgress";
            this.prbProgress.Size = new System.Drawing.Size(200, 19);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 37);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstItems);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propExtract);
            this.splitContainer1.Size = new System.Drawing.Size(1289, 702);
            this.splitContainer1.SplitterDistance = 940;
            this.splitContainer1.TabIndex = 11;
            // 
            // btnDebugOne
            // 
            this.btnDebugOne.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDebugOne.Image = ((System.Drawing.Image)(resources.GetObject("btnDebugOne.Image")));
            this.btnDebugOne.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDebugOne.Name = "btnDebugOne";
            this.btnDebugOne.Size = new System.Drawing.Size(24, 24);
            this.btnDebugOne.Text = "toolStripButton1";
            this.btnDebugOne.ToolTipText = "Debugs one file.";
            this.btnDebugOne.Click += new System.EventHandler(this.btnDebugOne_Click);
            // 
            // lstItems
            // 
            this.lstItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFileName,
            this.chHeight,
            this.chWidth,
            this.chSlices,
            this.chFormat,
            this.chVoxelDim});
            this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstItems.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstItems.FullRowSelect = true;
            this.lstItems.Location = new System.Drawing.Point(0, 0);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(940, 702);
            this.lstItems.TabIndex = 0;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = System.Windows.Forms.View.Details;
            // 
            // chFileName
            // 
            this.chFileName.Text = "File";
            this.chFileName.Width = 400;
            // 
            // chHeight
            // 
            this.chHeight.Text = "Height";
            // 
            // chWidth
            // 
            this.chWidth.Text = "Width";
            // 
            // chSlices
            // 
            this.chSlices.Text = "Slices";
            // 
            // chFormat
            // 
            this.chFormat.Text = "Format";
            this.chFormat.Width = 100;
            // 
            // chVoxelDim
            // 
            this.chVoxelDim.Text = "Voxel dim";
            this.chVoxelDim.Width = 200;
            // 
            // BatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1289, 764);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BatchForm";
            this.Text = "Cortical extractor";
            this.Load += new System.EventHandler(this.BatchForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propExtract;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoadBatch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnEngage;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel sblStatus;
        private System.Windows.Forms.ToolStripProgressBar prbProgress;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton btnDebugOne;
        private System.Windows.Forms.ListView lstItems;
        private System.Windows.Forms.ColumnHeader chFileName;
        private System.Windows.Forms.ColumnHeader chHeight;
        private System.Windows.Forms.ColumnHeader chWidth;
        private System.Windows.Forms.ColumnHeader chSlices;
        private System.Windows.Forms.ColumnHeader chFormat;
        private System.Windows.Forms.ColumnHeader chVoxelDim;
    }
}