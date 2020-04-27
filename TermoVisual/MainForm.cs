using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TermoVisual
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Список температур
        /// </summary>
        private List<Tuple<double, double>> list = new List<Tuple<double, double>>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                list.Clear();
                string[] lines = File.ReadAllLines(openFileDialog.FileName,Encoding.Default);
                foreach(string line in lines)
                {
                    if (line.Contains("Квартира")) continue;
                    string[] tokens = line.Split(';');
                    Tuple<double, double> tuple = new Tuple<double, double>(
                        Convert.ToDouble(tokens[0]),
                        Convert.ToDouble(tokens[1]));
                    list.Add(tuple);
                }
                Refresh();
            }
        }

        /// <summary>
        /// Преобрзовать температуру в цвет
        /// </summary>
        /// <param name="t">Температура</param>
        /// <returns>Цвет</returns>
        private Color get_t_color(double t)
        {
            double d = 2.8;
            double t1 = 14;
            if (t <= t1) return Color.FromArgb(0, 0, 255);
            t1 += d;
            if (t <= t1) return Color.FromArgb(0, Convert.ToInt32((t - t1+d)/d * 255.0), 255);
            t1 += d;
            if (t <= t1) return Color.FromArgb(0, 255, 255 - Convert.ToInt32((t - t1 + d) / d * 255.0));
            t1 += d;
            if (t <= t1) return Color.FromArgb(Convert.ToInt32((t - t1 + d) / d * 255.0), 255, 0);
            t1 += d;
            if (t <= t1) return Color.FromArgb(255, 255 - Convert.ToInt32((t - t1 + d) / d * 255.0), 0);
            return Color.FromArgb(255, 0, 0);
        }

        private void pbMap_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brush=new SolidBrush(Color.Black);
            SolidBrush brush_text1=new SolidBrush(Color.White);
            SolidBrush brush_text2 = new SolidBrush(Color.Black);
            Pen pen=new Pen(Color.Black);
            Font font=new Font("Times New Roman",10);
            Font font_flat = new Font("Times New Roman", 7);
            e.Graphics.FillRectangle(brush, 0, 0, 630, 350);
            if (list.Count == 0) return;
            int high = 35;
            int col_y = 350 / high;
            int col_x = list.Count / col_y+1;
            int width = 630 / col_x;
            int i = 0;
            int p = 0;
            for (int x = 0; x < col_x; x++)
            {
                for (int y = 0; y < col_y; y++)
                {
                    if (i >= list.Count) return;
                    e.Graphics.FillRectangle(new SolidBrush(get_t_color(list[i].Item2)), 
                        x * width, 
                        y * high, 
                        width, 
                        high);
                    e.Graphics.DrawRectangle(pen,
                        x * width,
                        y * high,
                        (x + 1) * width,
                        (y + 1) * high);

                    string s=Math.Round(list[i].Item2,2).ToString()+"C";
                    string flat = "кв. " + Math.Round(list[i].Item1, 2).ToString();
                    if (list[i].Item2 > 19 && list[i].Item2 < 24)
                    {
                        e.Graphics.DrawString(s, font, brush_text2, x * width + 1, y * high + 3);
                        e.Graphics.DrawString(flat, font_flat, brush_text2, x * width + 5, y * high + 20);
                    }
                    else
                    {
                        e.Graphics.DrawString(s, font, brush_text1, x * width + 1, y * high + 3);
                        e.Graphics.DrawString(flat, font_flat, brush_text1, x * width + 5, y * high + 20);
                    }            
                    
                    i++;
                    //if (p >= 1) return;
                }
                //if(p>=2) return;
                p++;
            }
        }
    }
}
