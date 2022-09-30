using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fost
{
    public partial class Form1 : Form
    {

        private const double dt = 0.001;
        private const double g = 9.8;
        private bool resistance = true;
        private bool speed = true;
        private const double pi = 3.14;
        private bool drawing = false;
        private double L = 0;
        private int speed_movements = 10;

        public Form1()
        {
            InitializeComponent();
            label1.Text = $"{pi}";
            label5.Text = $"{g}";
            label7.Text = $"{dt}";
            label19.Text = $"{speed_movements}ms";
            chart1.Series["Series1"].LegendText = "График XY";
        }

        private void draw(object sender=null, EventArgs e=null)
        {
            List<double> x = new List<double> { };
            List<double> y = new List<double> { };

            double v, m, a, k; // Let's go?
            if (!(double.TryParse(textBox1.Text, out v) && double.TryParse(textBox2.Text, out m) && double.TryParse(textBox3.Text, out a) && double.TryParse(textBox4.Text, out k)))
            {
                label15.Visible = true;
                return;
            }
            drawing = true;

            a = a * pi / 180;

           double v0x = v * Math.Cos(a);
            double v0y = v * Math.Sin(a);

            double t = 0;

            L = ((v * v) * Math.Sin(2 * (a))) / g;

            if (resistance)
            {
                while (y.Count() == 0 || y.Last() >= 0)
                {
                    x.Add(v0x * t);
                    y.Add((v0y * t) - (g * t * t / 2));
                    t += dt;
                }
            }
            else
            {
                while (y.Count() == 0 || y.Last() >= 0)
                {
                    x.Add((v0x * m / k) * (1 - Math.Exp(-1 * k * t / m)));
                    y.Add((m / k) * (((v0y + (m * g / k)) * (1 - Math.Exp(-1 * k * t / m))) - (g * t)));
                    t += dt;
                }
            }
            chart1.Series["Series1"].Points.Clear();
            if (speed)
                speedVariant(x, y);
            else
            {       
                Thread thr = new Thread(() => 
                {
                    notSpeedVariant(x, y);
                });
                thr.Start();
            }

            label20.Text = $"{L}";

            drawing = false;
            label15.Visible = false;
        }

        public void notSpeedVariant(List<double> x, List<double> y)
        {
            for (int i = 0; i < x.Count; i++)
            {
                chart1.Invoke(new MethodInvoker(delegate {
                    if (i > x.Count / 2)
                    Thread.Sleep(10);
                    chart1.Series["Series1"].LegendText = "График XY";
                    chart1.Series["Series1"].Points.AddXY(Math.Round(x[i], 2), Math.Round(y[i], 2));
                }));
        
            }
        }

        private void speedVariant(List<double> x, List<double> y)
        {
            chart1.Series["Series1"].LegendText = "График XY";
            for (int i = 0; i < x.Count; i++)
            {
                chart1.Series["Series1"].Points.AddXY(x[i], y[i]);
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {
                draw();
                return;
            }

            char input = e.KeyChar;
            string text = sender.ToString().Remove(0, 36);

            if (!Char.IsDigit(input) && 
                !(text.Length == 0 && input == '-') && 
                input != (char)8 && 
                !(text.IndexOf(',') == -1 && input == ',' && text.Length > 0))
            {
                e.Handled = true;
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

            resistance = !resistance;
            string answer = (resistance) ? "есть" : "нет";
            ((Label)sender).Text = $"сопротивление воздуха - {answer}";
        }

        private void label14_Click(object sender, EventArgs e)
        {
            ((Label)sender).Text = "by enotit <3";
        }

        private void label16_Click(object sender, EventArgs e)
        {
            speed = !speed;
            string plus = speed? "+" : "-";
            label16.Text = $"моментально ({plus})";
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (!drawing)
                chart1.Series["Series1"].Points.Clear();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }
    }
}
