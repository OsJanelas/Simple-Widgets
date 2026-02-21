using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;

namespace Calendar
{
    public class Calendar : Form
    {
        private DateTime date;
        private Point dragCursor;
        private Point dragForm;
        private bool drag = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Calendar());
        }

        public Calendar()
        {
            this.date = DateTime.Now;
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(240, 260);
            this.BackColor = Color.FromArgb(30, 30, 30); // Dark background
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            // DRAG AND CLOSE EVENTS
            this.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left) { drag = true; dragCursor = Cursor.Position; dragForm = this.Location; }
            };
            this.MouseMove += (s, e) => {
                if (drag)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(dragCursor));
                    this.Location = Point.Add(dragForm, new Size(dif));
                }
            };
            this.MouseUp += (s, e) => drag = false;
            this.MouseClick += (s, e) => { if (e.Button == MouseButtons.Right) Application.Exit(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. MONTHS AND YEAR TITLE
            string nomeMes = date.ToString("MMMM yyyy", CultureInfo.CurrentCulture).ToUpper();
            using (Font fontMes = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString(nomeMes, fontMes, Brushes.White, 20, 20);
            }

            // DAYS OF THE WEEK
            string[] diasSemana = { "S", "M", "T", "W", "T", "F", "S" };
            int larguraDia = (Width - 40) / 7;
            using (Font fontSemana = new Font("Arial", 9, FontStyle.Bold))
            {
                for (int i = 0; i < 7; i++)
                {
                    g.DrawString(diasSemana[i], fontSemana, Brushes.Gray, 20 + (i * larguraDia), 60);
                }
            }

            // 3. Lógica do Calendário
            DateTime primeiroDiaDoMes = new DateTime(date.Year, date.Month, 1);
            int diaDaSemanaInicial = (int)primeiroDiaDoMes.DayOfWeek;
            int diasNoMes = DateTime.DaysInMonth(date.Year, date.Month);

            int xBase = 20;
            int yBase = 90;
            int linha = 0;

            using (Font fontDia = new Font("Arial", 10, FontStyle.Regular))
            {
                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    int coluna = (diaDaSemanaInicial + dia - 1) % 7;
                    int x = xBase + (coluna * larguraDia);
                    int y = yBase + (linha * 35);

                    // Destacar o dia atual
                    if (dia == DateTime.Now.Day && date.Month == DateTime.Now.Month && date.Year == DateTime.Now.Year)
                    {
                        g.FillEllipse(Brushes.Green, x - 5, y - 5, 28, 28);
                        g.DrawString(dia.ToString(), fontDia, Brushes.White, x, y);
                    }
                    else
                    {
                        g.DrawString(dia.ToString(), fontDia, Brushes.LightGray, x, y);
                    }

                    if (coluna == 6) linha++; // Pula para a próxima linha no sábado
                }
            }

            // Rodapé informativo
            g.DrawString("Right button to leave", new Font("Segoe UI", 7), Brushes.DimGray, 20, Height - 20);
        }
    }
}