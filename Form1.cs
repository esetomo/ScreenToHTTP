using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace ScreenToHTTP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:10158/");
            listener.Start();

            listener.BeginGetContext(OnGetContext, listener);
        }

        private void OnGetContext(IAsyncResult ar)
        {
            HttpListener listener = (HttpListener)ar.AsyncState;
            HttpListenerContext context = listener.EndGetContext(ar);
            OnRequest(context);
            listener.BeginGetContext(OnGetContext, listener);
        }

        private void OnRequest(HttpListenerContext context)
        {
            HttpListenerRequest req = context.Request;
            HttpListenerResponse res = context.Response;

            res.ContentType = "image/png";
            pictureBox1.Image.Save(res.OutputStream, ImageFormat.Png);
            res.Close();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:10158/");
        }
    }
}
