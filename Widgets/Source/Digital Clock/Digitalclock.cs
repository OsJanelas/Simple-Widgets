using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DigitalClock
{
    public class DigitalClock : Form
    {
        private Timer timer;

        [STAThread]
        static void Main() => Application.Run(new DigitalClock());

        public DigitalClock()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(300, 120);
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            DateTime agora = DateTime.Now;

            using (Pen p = new Pen(Color.FromArgb(50, 255, 255, 255), 1))
            {
                g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            }

            string hora = agora.ToString("HH:mm");
            string segundos = agora.ToString(":ss");
            string data = agora.ToString("dddd, dd 'de' MMMM").ToUpper();

            using (Font fHora = new Font("Arial Black", 50, FontStyle.Regular))
            using (Font fSeg = new Font("Arial", 20, FontStyle.Bold))
            {
                Size sizeHora = TextRenderer.MeasureText(hora, fHora);
                float x = (Width - sizeHora.Width) / 2 - 10;
                float y = 15;

                g.DrawString(hora, fHora, Brushes.Black, x + 2, y + 2);
                g.DrawString(hora, fHora, Brushes.White, x, y);

                g.DrawString(segundos, fSeg, Brushes.DimGray, x + sizeHora.Width - 15, y + 35);
            }

            using (Font fData = new Font("Arial", 9, FontStyle.Regular))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(data, fData, Brushes.GreenYellow, new Rectangle(0, 90, Width, 20), sf);
            }
        }
    }
}