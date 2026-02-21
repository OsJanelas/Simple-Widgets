using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace ClimaWidgetCorrigido
{
    public class WeatherWidget : Form
    {
        private string temperature = "--°C";
        private string city = "SÃO PAULO - BRAZIL";
        private System.Windows.Forms.Timer timerUpdate;

        [STAThread]
        static void Main() => Application.Run(new WeatherWidget());

        public WeatherWidget()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(250, 130);
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            timerUpdate = new System.Windows.Forms.Timer { Interval = 600000 }; // 10 min
            timerUpdate.Tick += async (s, e) => await Search();
            timerUpdate.Start();

            Config();
            _ = Search();
        }

        private async Task Search()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // OPEN-METEO URL
                    string url = "https://api.open-meteo.com/v1/forecast?latitude=-23.54&longitude=-46.63&current_weather=true";
                    string response = await client.GetStringAsync(url);

                    using (JsonDocument doc = JsonDocument.Parse(response))
                    {
                        // EXTRAT JSON ELEMENT
                        JsonElement root = doc.RootElement;

                        // NAVEGATE TO "current_weather" AND EXTRACT TEMPERATURE
                        if (root.TryGetProperty("current_weather", out JsonElement weather))
                        {
                            double temp = weather.GetProperty("temperature").GetDouble();
                            temperature = $"{Math.Round(temp)}°C";
                        }
                    }
                }
            }
            catch (Exception)
            {
                temperature = "ERROR: a error ocorred";
            }
            this.Invalidate(); // REDRAW SCREEN
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // BACKGROUND
            using (LinearGradientBrush lgb = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(150, 160, 190), Color.FromArgb(20, 20, 30), 90f))
            {
                g.FillRectangle(lgb, this.ClientRectangle);
            }

            // SUN
            g.FillEllipse(Brushes.Gold, 20, 35, 45, 45);

            // TEXTS
            using (Font fTemp = new Font("Arial", 32, FontStyle.Bold))
            using (Font fCity = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                g.DrawString(temperature, fTemp, Brushes.White, 80, 30);
                g.DrawString(city, fCity, Brushes.SkyBlue, 85, 85);
            }
        }

        private void Config()
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