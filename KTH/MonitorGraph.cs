using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace KTH {
    public partial class MonitorGraph : UserControl {

        public DBPanel panel1;
        private List<double> values;

        public string desc;
        public string unit;
        //public string latestVal = "";

        public double minValue = 0;
        public double maxValue = 0.001;
        //private double displayMinValue = 0;

        private int maxWidth, maxHeight;

        private int round;

        private int displayX;

        public MonitorGraph(string desc, int round, string unit, int width, int height) {
            InitializeComponent();


            this.Size = new Size(width, height);

            maxWidth = this.Size.Width - 1;
            maxHeight = this.Size.Height - 20;

            this.desc = desc;
            this.round = round;
            this.unit = unit;
            panel1 = new DBPanel();
            panel1.Size = this.Size;
            panel1.Location = new Point(0, 0);
            panel1.MouseMove += MonitorGraph_MouseMove;
            panel1.MouseLeave += MonitorGraph_MouseLeave;
            this.Controls.Add(panel1);

            values = new List<double>();

            panel1.Paint += new PaintEventHandler(panel1_Paint);
        }

        private Pen graphPen = new Pen(Color.Purple, 1);
        private Pen graphPen2 = new Pen(Color.Purple, 1);
        Font drawFont = new Font("Courier New", 8);
        Font drawFontLarge = new Font("Courier New", 14, FontStyle.Bold);
        SolidBrush drawBrush = new SolidBrush(Color.Black);
        Brush markBrush = new SolidBrush(Color.Purple);
        Point[] graphPoints;

        long time, time2;
        long timeavg;

        private void panel1_Paint(object sender, PaintEventArgs e) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if(values.Count > 1) g.DrawLines(graphPen, graphPoints);
            //maxValue = 55555;
            //g.DrawString(("max "+maxValue.ToString()).PadLeft(12, ' '), drawFont, drawBrush, new Point(this.Size.Width - 65, 0));
            //g.DrawString(("min"+minValue.ToString()).PadLeft(12, ' '), drawFont, drawBrush, new Point(this.Size.Width - 65, this.Size.Height - 15));

            stopwatch.Stop();

            time = (long)(stopwatch.ElapsedTicks * 1000000.0 / Stopwatch.Frequency);
            timeavg = (timeavg * 9 + time) / 10;

            // Display value at specific point on curve
            int x = displayX - (maxWidth - values.Count);

            g.DrawString(desc, drawFont, drawBrush, 0, 0);
            g.DrawString("min: " + minValue.ToString("F" + round.ToString()) + " max: " + maxValue.ToString("F" + round.ToString()), drawFont, drawBrush, 150, 0);

            if(x > 0 && x < values.Count) {
                g.FillEllipse(markBrush, displayX - 3, graphPoints[x].Y - 3, 6, 6);
                //g.DrawString(values[displayX].ToString(), drawFont, markBrush, 5, this.Size.Height-20);
                //g.DrawString(desc + ": " + values[x].desc, drawFont, markBrush, 0, 0);
                g.DrawString(String.Format("{0,10}", $"{values[x].ToString("F" + round.ToString())} {unit}"), drawFontLarge, markBrush, new Point(this.Size.Width - 75 - 50, 0));
            } else if(values.Count > 0) {
                //g.DrawString(desc + ": " + latestVal, drawFont, drawBrush, new Point(0, 0));
                //g.DrawString("drw:"+time.ToString()+"µs", drawFont, drawBrush, new Point(0, 0));
                //g.DrawString("add:"+time2.ToString() + "µs", drawFont, drawBrush, new Point(0, 80));
                g.DrawString(String.Format("{0,10}", $"{values[values.Count - 1].ToString("F" + round.ToString())} {unit}"), drawFontLarge, drawBrush, new Point(this.Size.Width - 75 - 50, 0));
            }
        }

        public void addValue(double val) {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            //val = (double) Math.Round(val, round);
            values.Add(val);

            if(values.Count > maxWidth) {
                values.RemoveAt(0);
            }

            double newMax = double.MinValue;
            double newMin = double.MaxValue;
            foreach(double value in values) {
                if(value > newMax) newMax = value;
                if(value < newMin) newMin = value;
            }

            //if (newMax < maxValue) maxValue = val;
            maxValue = newMax;
            //displayMinValue = newMin;
            minValue = newMin;

            /*if(maxValue - minValue < 0.1 ) {
                minValue = maxValue - 1;
                if(minValue < 0) {
                    minValue = maxValue;
                    maxValue = maxValue + 1;
                }
            }*/

            //if (val > maxValue) maxValue = val;

            if(graphPoints == null) {
                graphPoints = new Point[values.Count];
            }

            if(graphPoints.Length < values.Count) {
                Point[] tempPoints = graphPoints;
                graphPoints = new Point[values.Count];
            }

            int y = 0;
            for(int i = 0; i < values.Count; i++) {
                if(maxValue == minValue) {
                    y = (this.Size.Height - 20) / 2;
                } else {
                    y = (int)(((values[i] - minValue) * maxHeight )/(maxValue - minValue));
                    y = (this.Size.Height - 2) - y;
                }
                graphPoints[i] = new Point(i + (maxWidth - values.Count), y + 1);
            }

            //minValue = Math.Round(values.Min(), 0);
            /*if (maxValue - minValue < 0.001) {
                maxValue = minValue + 0.001;
            }*/

            //stopwatch.Stop();

            //time2 = (long)(stopwatch.ElapsedTicks * 1000000.0 / Stopwatch.Frequency);

            panel1.Invalidate();
        }


        private void MonitorGraph_MouseMove(object sender, MouseEventArgs e) {
            displayX = e.X;
            panel1.Invalidate();
        }
        private void MonitorGraph_MouseLeave(object sender, EventArgs e) {
            displayX = -1;
            panel1.Invalidate();
        }
    }
}
