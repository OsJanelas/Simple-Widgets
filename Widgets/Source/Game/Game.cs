using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PixelGame
{
    public class PixelPuzzle : Form
    {
        private int[,] gradePlayer = new int[10, 10];

        private int[,] gabarito = {
            {0,0,1,1,0,0,1,1,0,0},
            {0,1,1,1,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,0,0,0},
            {0,0,0,0,1,1,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        };

        private int tamanhoQuadrado = 30;

        [STAThread]
        static void Main() => Application.Run(new PixelPuzzle());

        public PixelPuzzle()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(300, 350);
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.MouseClick += PixelPuzzle_MouseClick;
        }

        private void PixelPuzzle_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) Application.Exit();

            int col = e.X / tamanhoQuadrado;
            int lin = e.Y / tamanhoQuadrado;

            if (lin < 10 && col < 10)
            {
                if (gabarito[lin, col] == 1)
                    gradePlayer[lin, col] = 1;
                else
                    gradePlayer[lin, col] = 2;

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int l = 0; l < 10; l++)
            {
                for (int c = 0; c < 10; c++)
                {
                    Rectangle rect = new Rectangle(c * tamanhoQuadrado, l * tamanhoQuadrado, tamanhoQuadrado, tamanhoQuadrado);

                    if (gradePlayer[l, c] == 1)
                        g.FillRectangle(Brushes.Crimson, rect);
                    else if (gradePlayer[l, c] == 2)
                    {
                        g.FillRectangle(Brushes.DimGray, rect);
                        g.DrawLine(Pens.White, rect.Left, rect.Top, rect.Right, rect.Bottom);
                        g.DrawLine(Pens.White, rect.Right, rect.Top, rect.Left, rect.Bottom);
                    }

                    g.DrawRectangle(Pens.Black, rect);
                }
            }

            using (Font f = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                g.DrawString("Pixel Puzzle! (Dir: Leave)", f, Brushes.White, 10, 310);
            }
        }
    }
}