using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScreenToHTTP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            int x = int.Parse(textBoxX.Text);
            int y = int.Parse(textBoxY.Text);
            int width = int.Parse(textBoxWidth.Text);
            int height = int.Parse(textBoxHeight.Text);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            g.Dispose();
            pictureBox1.Image = bmp;
        }
    }
}
