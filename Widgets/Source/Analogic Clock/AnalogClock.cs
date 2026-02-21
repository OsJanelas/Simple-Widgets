using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AnalogClock
{
    public class ClockWidget : Form
    {
        private Timer timer;
        private Point arrastarCursor;
        private Point arrastarForm;
        private bool arrastando = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClockWidget());
        }

        public ClockWidget()
        {
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TransparencyKey = Color.Empty;
            this.Size = new Size(150, 150);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) => this.Invalidate();
            timer.Start();

            this.MouseDown += (s, e) => { arrastando = true; arrastarCursor = Cursor.Position; arrastarForm = this.Location; };
            this.MouseMove += (s, e) => {
                if (arrastando)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(arrastarCursor));
                    this.Location = Point.Add(arrastarForm, new Size(dif));
                }
            };
            this.MouseUp += (s, e) => arrastando = false;

            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Leave", null, (s, e) => Application.Exit());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int margem = 10;
            int diametro = Math.Min(Width, Height) - (margem * 2);
            Rectangle rect = new Rectangle(margem, margem, diametro, diametro);
            Point centro = new Point(Width / 2, Height / 2);

            using (Pen penBorda = new Pen(Color.FromArgb(32, 32, 32), 6))
            {
                g.FillEllipse(Brushes.WhiteSmoke, rect);
                g.DrawEllipse(penBorda, rect);
            }

            for (int i = 0; i < 12; i++)
            {
                double angulo = i * 30 * Math.PI / 180;
                int x1 = (int)(centro.X + (diametro / 2 - 5) * Math.Sin(angulo));
                int y1 = (int)(centro.Y - (diametro / 2 - 5) * Math.Cos(angulo));
                int x2 = (int)(centro.X + (diametro / 2 - 15) * Math.Sin(angulo));
                int y2 = (int)(centro.Y - (diametro / 2 - 15) * Math.Cos(angulo));
                g.DrawLine(new Pen(Color.Black, 3), x1, y1, x2, y2);
            }

            DateTime agora = DateTime.Now;

            // THIS PART IS MADE WITH GEMINI
            DesenharPonteiro(g, centro, (agora.Hour % 12 + agora.Minute / 60.0) * 30, diametro * 0.25, new Pen(Color.Black, 8));
            DesenharPonteiro(g, centro, (agora.Minute + agora.Second / 60.0) * 6, diametro * 0.4, new Pen(Color.Gray, 5));
            DesenharPonteiro(g, centro, agora.Second * 6, diametro * 0.45, new Pen(Color.Green, 2));

            g.FillEllipse(Brushes.Black, centro.X - 6, centro.Y - 6, 8, 8);
        }

        private void DesenharPonteiro(Graphics g, Point centro, double anguloDeg, double comprimento, Pen pen)
        {
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.ArrowAnchor;

            double rad = Math.PI * anguloDeg / 180;
            int x = (int)(centro.X + comprimento * Math.Sin(rad));
            int y = (int)(centro.Y - comprimento * Math.Cos(rad));
            g.DrawLine(pen, centro.X, centro.Y, x, y);
        }
    }
}