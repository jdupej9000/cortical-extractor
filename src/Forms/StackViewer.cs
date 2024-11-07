using CorticalExtract.DataStructures;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Windows.Forms;

namespace CorticalExtract.Forms
{
    public partial class StackViewer : UserControl
    {
        public StackViewer()
        {
            InitializeComponent();
            picView.Paint += picView_Paint;
            picView.MouseWheel += OnMouseWheel;
            //this.MouseWheel += OnMouseWheel;
            stack = null;
            AdditionalPaint = null;
            MeasurementMessage += (s, m) => { };
        }

        public ImageStack Stack
        {
            get
            {
                return stack;
            }
            set
            {
                stack = value;
                picView.Invalidate();
            }
        }

        public byte[] Mask
        {
            get
            {
                return mask;
            }
            set
            {
                mask = value;
                picView.Invalidate();
            }
        }

        public class ApdParams : EventArgs
        {
            public ApdParams()
            {
                scale = 1;
                g = null;
            }

            public ApdParams(Graphics g, float sc, int sl)
            {
                scale = sc;
                this.g = g;
                slice = sl;
            }

            public Graphics g;
            public float scale;
            public int slice;
        }

        public delegate void MeasurementMessageDelegate(object sender, string msg);
        public MeasurementMessageDelegate MeasurementMessage;

        public delegate void AdditionalPaintDelegate(object sender, ApdParams e);
        public AdditionalPaintDelegate AdditionalPaint;

        protected ImageStack stack;
        protected byte[] mask;
        StackViewItem svi = null;
        string lmAannot = string.Empty, lmBannot = string.Empty;

        bool lmAset = false, lmBset = false;
        Vector3 lmA = Vector3.Zero, lmB = Vector3.Zero;

        public void SetItem(StackViewItem svi)
        {
            this.svi = svi;
            stack = svi.stack;
            mask = svi.mask;
            lmAset = false;
            lmBset = false;
            picView.Invalidate();
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtMin_Click(object sender, EventArgs e)
        {

        }

        private void txtMax_Click(object sender, EventArgs e)
        {

        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (stack == null) return;

            int slice = 0;
            if (!int.TryParse(toolStripTextBox1.Text, out slice)) return;

            if (e.Delta > 0) slice++;
            else slice--;

            if (slice >= 0 & slice < stack.Slices)
            {
                toolStripTextBox1.Text = slice.ToString();
                picView.Invalidate();
            }

        }

        private void MaskToColor(byte[] cls, int idx, byte[] factors)
        {
            if (cls == null)
            {
                factors[0] = 255; factors[1] = 255; factors[2] = 255;
                return;
            }

            switch (cls[idx])
            {
                case 0:
                    factors[0] = 255; factors[1] = 255; factors[2] = 255;
                    break;

                case 1:
                    factors[0] = 255; factors[1] = 0; factors[2] = 0;
                    break;

                case 2:
                    factors[0] = 0; factors[1] = 255; factors[2] = 0;
                    break;

                case 3:
                    factors[0] = 0; factors[1] = 0; factors[2] = 255;
                    break;
            }
        }


        void picView_Paint(object sender, PaintEventArgs e)
        {
            if (stack == null) return;

            float min = -1000;
            float max = 3096;
            int slice = 0;
            float scale = 1;
            byte[] channelFactor = new byte[3] { 255, 255, 255 };

            if (!float.TryParse(txtMin.Text, out min)) return;
            if (!float.TryParse(txtMax.Text, out max)) return;
            if (!float.TryParse(txtScale.Text, out scale)) return;
            if (!int.TryParse(toolStripTextBox1.Text, out slice)) return;

            if (slice >= 0 & slice < stack.Slices)
            {
                Bitmap bmp = new Bitmap(stack.Width, stack.Height, PixelFormat.Format32bppArgb);
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                  System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                                  bmp.PixelFormat);

                unsafe
                {
                    int maskIdx = 0;
                    for (int y = 0; y < bmd.Height; y++)
                    {
                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (int x = 0; x < bmd.Width; x++)
                        {
                            float v = MathF.Min(255.0f, MathF.Max(0, 255.0f * (stack[x, y, slice] - min) / (max - min)));
                            byte b = (byte)v;

                            MaskToColor(mask, maskIdx, channelFactor);

                            row[x * 4] = (byte)(b & channelFactor[2]);
                            row[x * 4 + 1] = (byte)(b & channelFactor[1]);
                            row[x * 4 + 2] = (byte)(b & channelFactor[0]);
                            row[x * 4 + 3] = 255;

                            maskIdx++;
                        }
                    }
                }

                bmp.UnlockBits(bmd);

                e.Graphics.DrawImage(bmp, 0, 0, scale * (float)bmd.Width, scale * (float)bmd.Height);
            }

            if (chkAnnot.Checked)
            {
                if (AdditionalPaint != null) AdditionalPaint(this, new ApdParams(e.Graphics, scale, slice));

                Pen penLm = new Pen(Color.Red);
                Brush brushLm = new SolidBrush(Color.Red);

                if (lmAset)
                {
                    e.Graphics.DrawLine(penLm, lmA.X * scale - 2, lmA.Y * scale, lmA.X * scale + 2, lmA.Y * scale);
                    e.Graphics.DrawLine(penLm, lmA.X * scale, lmA.Y * scale - 1, lmA.X * scale, lmA.Y * scale + 2);
                    e.Graphics.DrawString(string.Format("A {0}", lmA.Z), Font, brushLm, lmA.X * scale + 2, lmA.Y * scale + 2);
                }

                if (lmBset)
                {
                    e.Graphics.DrawLine(penLm, lmB.X * scale - 2, lmB.Y * scale, lmB.X * scale + 2, lmB.Y * scale);
                    e.Graphics.DrawLine(penLm, lmB.X * scale, lmB.Y * scale - 1, lmB.X * scale, lmB.Y * scale + 2);
                    e.Graphics.DrawString(string.Format("B {0}", lmB.Z), Font, brushLm, lmB.X * scale + 2, lmB.Y * scale + 2);
                }

            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            picView.Invalidate();
        }

        private void picView_MouseDown(object sender, MouseEventArgs e)
        {
            bool keepChecked = Control.ModifierKeys == Keys.Shift;

            int slice = 0; ;
            float scale = 1;
            if (!float.TryParse(txtScale.Text, out scale)) return;
            if (!int.TryParse(toolStripTextBox1.Text, out slice)) return;

            if (tsbLmA.Checked)
                lmA = new Vector3((float)e.X / scale, (float)e.Y / scale, slice);
            else if (tsbLmB.Checked)
                lmB = new Vector3((float)e.X / scale, (float)e.Y / scale, slice);

            if (!keepChecked)
            {
                tsbLmA.Checked = false;
                tsbLmB.Checked = false;
            }

            if (lmAset && lmBset)
            {
                Vector3 a = svi.TransformPoint(lmA.XY(), Math.Max(0, (int)lmA.Z));
                Vector3 b = svi.TransformPoint(lmB.XY(), Math.Max(0, (int)lmB.Z));
                float d = VectorUtils.DistanceAniso(a, b, svi.voxDim);

                string msg = string.Format("d ={0:#####0.000}", d);
                MeasurementMessage(this, msg);
            }

            picView.Invalidate();
        }

        private void chkAnnot_Click(object sender, EventArgs e)
        {
            picView.Invalidate();
        }

        private void picView_Click(object sender, EventArgs e)
        {

        }
    }
}
