using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorticalExtract.Forms
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();            
        }
                
        public List<StackViewItem> Items = new List<StackViewItem>();
        StackViewItem activeItem = null;
        string stackInfo = string.Empty;
        string measureInfo = string.Empty;


        private void DebugForm_Load(object sender, EventArgs e)
        {
            if (Items.Count > 0)
                activeItem = Items[0];

            foreach (StackViewItem svi in Items)
            {
                lstVis.Items.Add(svi.name);
            }

            stackView.MeasurementMessage += MeasurementMessage;
        }

        protected void UpdateView()
        {
            stackView.AdditionalPaint = AdditionalPaint;
            // stackView.Stack = activeItem.stack;
            // stackView.Mask = activeItem.mask;
            stackView.SetItem(activeItem);
            stackInfo = string.Format("{0}x{1}, {2} slices", activeItem.stack.Width, activeItem.stack.Height, activeItem.stack.Slices);
            measureInfo = "";
            UpdateStatus();
        }

        void MeasurementMessage(object sender, string msg)
        {
            measureInfo = msg;
            UpdateStatus();
        }

        void UpdateStatus()
        {
            lblInfo.Text = stackInfo + "\n" + measureInfo;
        }

        void AdditionalPaint(object sender, CorticalExtract.Forms.StackViewer.ApdParams e)
        {
            float sc = e.scale;
            Graphics g = e.g;

            Pen greenOdd = new Pen(Color.DarkRed);
            Pen green = new Pen(Color.Red);
            Pen red = new Pen(Color.LightGreen);

            float width = (float)activeItem.stack.Width * e.scale;
            float height = (float)activeItem.stack.Height * e.scale;
            SizeF center = new SizeF(width / 2, height / 2);

            g.DrawLine(green, new PointF(width / 2, height / 2 - 4), new PointF(width / 2, height / 2 + 4));
            g.DrawLine(green, new PointF(width / 2 - 4, height / 2), new PointF(width / 2 + 4, height / 2));

            if (activeItem.inner != null )
            {
                if (activeItem.inner.GetUpperBound(0) >= e.slice)
                {
                    int n = activeItem.inner.GetUpperBound(1);
                    for (int i = 0; i < n - 1; i++)
                        g.DrawLine(i % 2 == 1 ? red : red, activeItem.inner[e.slice, i].ToPointF(sc) + center, activeItem.inner[e.slice, i + 1].ToPointF(sc) + center);
                    g.DrawLine(green, activeItem.inner[e.slice, n - 1].ToPointF(sc) + center, activeItem.inner[e.slice, 0].ToPointF(sc) + center);
                }
            }

            if (activeItem.outer != null )
            {
                if (activeItem.outer.GetUpperBound(0) >= e.slice)
                {
                    int n = activeItem.outer.GetUpperBound(1);
                    //g.DrawLine(new Pen(Color.Red), activeItem.outer[e.slice, 0].ToPointF(sc) + center, activeItem.outer[e.slice, 1].ToPointF(sc) + center);
                    for (int i = 0; i < n - 1; i++)
                        g.DrawLine(i % 2 == 1 ? green : greenOdd, activeItem.outer[e.slice, i].ToPointF(sc) + center, activeItem.outer[e.slice, i + 1].ToPointF(sc) + center);
                    g.DrawLine(green, activeItem.outer[e.slice, n - 1].ToPointF(sc) + center, activeItem.outer[e.slice, 0].ToPointF(sc) + center);
                                        
                }
            }
        }

        private void lstVis_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstVis.SelectedIndex;
            if (idx < 0) return;

            activeItem = Items[idx];         
            UpdateView();
        }
    }
}
