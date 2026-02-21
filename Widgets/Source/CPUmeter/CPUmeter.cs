using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace CpuWidget
{
    public class CpuMonitor : Form
    {
        private PerformanceCounter cpuCounter;
        private List<float> historico = new List<float>();
        private Timer timer;
        private float valorAtual = 0;

        [STAThread]
        static void Main() => Application.Run(new CpuMonitor());

        public CpuMonitor()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.Size = new Size(250, 150);
            this.TopMost = true;
            this.DoubleBuffered = true;

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => {
                valorAtual = cpuCounter.NextValue();
                historico.Add(valorAtual);
                if (historico.Count > 40) historico.RemoveAt(0);
                this.Invalidate();
            };
            timer.Start();

            ConfigurarInteracao();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int larguraBarra = (int)((this.Width - 40) * (valorAtual / 100));
            Color corBarra = valorAtual > 80 ? Color.Tomato : Color.LimeGreen;

            g.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), 20, 45, Width - 40, 10);
            g.FillRectangle(new SolidBrush(corBarra), 20, 45, larguraBarra, 10);

            using (Font f = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                g.DrawString("CPU USAGE", f, Brushes.Gray, 20, 15);
                g.DrawString($"{(int)valorAtual}%", f, Brushes.White, Width - 70, 15);
            }

            if (historico.Count > 1)
            {
                using (Pen pen = new Pen(Color.Green, 2))
                {
                    PointF[] pontos = new PointF[historico.Count];
                    for (int i = 0; i < historico.Count; i++)
                    {
                        float x = 20 + (i * ((float)(Width - 40) / 40));
                        float y = Height - 20 - (historico[i] * 0.5f);
                        pontos[i] = new PointF(x, y);
                    }
                    g.DrawLines(pen, pontos);
                }
            }
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