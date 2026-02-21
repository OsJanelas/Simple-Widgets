using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Slideshow
{
    public class SlideshowWidget : Form
    {
        private List<string> caminhosImagens = new List<string>();
        private int indiceAtual = 0;
        private Image imagemExibida;
        private System.Windows.Forms.Timer timerTroca;

        [STAThread]
        static void Main() => Application.Run(new SlideshowWidget());

        public SlideshowWidget()
        {
            // SETTINGS
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(300, 200);
            this.BackColor = Color.Black;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;

            // IMAGES
            string pastaFotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (Directory.Exists(pastaFotos))
            {
                caminhosImagens.AddRange(Directory.GetFiles(pastaFotos, "*.jpg"));
                caminhosImagens.AddRange(Directory.GetFiles(pastaFotos, "*.png"));
            }

            // TIMER
            timerTroca = new System.Windows.Forms.Timer { Interval = 5000 };
            timerTroca.Tick += (s, e) => CarregarProximaImagem();
            timerTroca.Start();

            CarregarProximaImagem();
            ConfigurarInteracao();
        }

        private void CarregarProximaImagem()
        {
            if (caminhosImagens.Count == 0) return;

            try
            {
                imagemExibida?.Dispose();

                // LOADS NEW IMAGE
                imagemExibida = Image.FromFile(caminhosImagens[indiceAtual]);

                indiceAtual = (indiceAtual + 1) % caminhosImagens.Count;
                this.Invalidate(); // REDRAW
            }
            catch { /* IGNORE CORRUPTED FILES ERRORS */ }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (imagemExibida != null)
            {
                //CALCULATING THE BETTER SIZE TO FIT THE IMAGE IN THE WIDGET
                float ratio = Math.Min((float)this.Width / imagemExibida.Width, (float)this.Height / imagemExibida.Height);
                int novoW = (int)(imagemExibida.Width * ratio);
                int novoH = (int)(imagemExibida.Height * ratio);
                int posX = (this.Width - novoW) / 2;
                int posY = (this.Height - novoH) / 2;

                g.DrawImage(imagemExibida, posX, posY, novoW, novoH);
            }
            else
            {
                g.DrawString("Don't finded images", this.Font, Brushes.White, 10, 10);
            }

            //FRAME
            g.DrawRectangle(new Pen(Color.FromArgb(100, 255, 255, 255), 2), 0, 0, Width - 1, Height - 1);
        }

        private void ConfigurarInteracao()
        {
            this.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    this.Capture = false;
                    Message m = Message.Create(this.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
                    this.WndProc(ref m);
                }
                else if (e.Button == MouseButtons.Right) Application.Exit();
            };
        }
    }
}