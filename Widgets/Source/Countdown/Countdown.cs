using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Countdown
{
    public class CountdownWidget : Form
    {
        private Timer timer;
        private DateTime dataAlvo = new DateTime(2026, 12, 25, 0, 0, 0);
        private string eventoNome = "CHRISTMAS";

        [STAThread]
        static void Main() => Application.Run(new CountdownWidget());

        public CountdownWidget()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(320, 150);
            this.BackColor = Color.FromArgb(25, 25, 25);
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => this.Invalidate();
            timer.Start();

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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // CALCULATING DATE
            TimeSpan diferenca = dataAlvo - DateTime.Now;

            // IF DATE NOW
            if (diferenca.Ticks < 0) diferenca = TimeSpan.Zero;

            // DRAW
            using (Pen p = new Pen(Color.FromArgb(35, 155, 0), 2)) // GREEN
            {
                g.DrawRectangle(p, 1, 1, Width - 3, Height - 3);
            }

            // DRAW EVENT NAME
            using (Font fTitulo = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                g.DrawString($"COUNTDOWN: {eventoNome}", fTitulo, Brushes.Gray, 20, 15);
            }

            // FORMAT
            string tempoStr = string.Format("{0}d {1:00}h {2:00}m {3:00}s",
                diferenca.Days, diferenca.Hours, diferenca.Minutes, diferenca.Seconds);

            using (Font fTempo = new Font("Arial", 26, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                Rectangle rectTempo = new Rectangle(0, 40, Width, 60);

                g.DrawString(tempoStr, fTempo, Brushes.Black, new Rectangle(2, 42, Width, 60), sf);
                g.DrawString(tempoStr, fTempo, Brushes.LightGreen, rectTempo, sf);
            }

            g.FillRectangle(Brushes.DimGray, 20, 110, Width - 40, 4);
            float progressoMinutos = (float)diferenca.Seconds / 60 * (Width - 40);
            g.FillRectangle(Brushes.Green, 20, 110, progressoMinutos, 4);

            // A
            g.DrawString(dataAlvo.ToString("dd/MM/yyyy HH:mm"), new Font("Segoe UI", 8), Brushes.DimGray, 20, 125);
        }
    }
}