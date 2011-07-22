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

        delegate void Action();

        private void OnRequest(HttpListenerContext context)
        {
            HttpListenerRequest req = context.Request;
            HttpListenerResponse res = context.Response;

            if (req.Url.AbsolutePath == "/")
            {
                WritePage(res);
            }
            else
            {
                res.ContentType = "image/png";
                Invoke((Action)(() => pictureBox1.Image.Save(res.OutputStream, ImageFormat.Png)));
                res.Close();
            }
        }

        private void WritePage(HttpListenerResponse res)
        {
            res.ContentEncoding = Encoding.UTF8;
            res.ContentType = "text/html";

            using (StreamWriter writer = new StreamWriter(res.OutputStream, Encoding.UTF8))
            {
                writer.Write(@"
                    <!DOCTYPE html>
                    <html>
                      <head>
                        <title>ScreenToHttp</title>
                        <style>
                          html { overflow: hidden; }
                          body { margin: 0; overflow: hidden; }
                        </style>
                        <script>
                            function update(){
                                var img = document.getElementById('screen');
                                img.src = '/screen.png?' + (new Date()).getTime();
                            }
                        </script>
                      </head>
                      <body onload='setInterval(update, 1000)'>
                        <img id='screen' src='/screen.png' alt='Screen Image'>
                      </body>
                    </html>
                ");
            }
            res.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:10158/");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            int width = 512;
            int height = 512;

            int.TryParse(textBoxX.Text, out x);
            int.TryParse(textBoxY.Text, out y);
            int.TryParse(textBoxWidth.Text, out width);
            int.TryParse(textBoxHeight.Text, out height);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            g.Dispose();
            pictureBox1.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
