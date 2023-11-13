using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandatory_pro_11_11_2023
{
    public partial class Form1 : Form
    {
        Pen pen, eraser;
        Graphics graphics;
        Bitmap bitmap;
        int shape = 0;
        String text = "";
        bool draw;
        bool erase;
        PointF eLocation;


        #region Initializers
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pen = new Pen(Color.Black, 3);
            eraser = new Pen(pictureBox1.BackColor, 5);
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            pictureBox1.Image = bitmap;
            draw = false;
            erase = false;
        }
        #endregion

        #region Menu
        private void shapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            draw=true;
            erase = false;
        }

        private void eraserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            draw = false;
            //erase = !erase;
        }

        #endregion

        #region Menu Items
        private void crossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 1;
            chooseColor();


        }

        private void diamondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 2;
            chooseColor();
        }

        private void isoscelesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 3;
            chooseColor();
        }

        private void kiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 4;
            chooseColor();
        }

        private void parallelogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 5;
            chooseColor();
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 6;
            chooseColor();
        }

        private void rightAngleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 7;
            chooseColor();
        }

        private void starToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 8;
            chooseColor();
        }

        private void trapeziumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 9;
            chooseColor();
        }

        private void octagonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shape = 10;
            chooseColor();
        }
        #endregion

        #region Picture Box Mouse Events
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            erase = false;
            PointF clicked = new PointF(e.X, e.Y);
            if (draw && shape != 11)
            {
                List<PointF> points = placeShape(loadPoints(select_shape(shape)), clicked);
                //PointF center = getCenter(points);
                graphics.DrawLines(pen, points.ToArray());
                //for (int i = 0; i < points.Count - 1; i++)
                //{
                //    graphics.DrawLine(pen, points[i], points[i + 1]);
                //}

            }
            else if (draw && shape == 11)
            {
                //Prints text

                graphics.DrawString(text, Font, pen.Brush, clicked);
            }
            pictureBox1.Refresh();
            timer1.Enabled = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            erase = !draw;
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (erase )
            {
                //timer1.Enabled = true;
                PointF eLocation = e.Location;
                graphics.DrawEllipse(eraser, eLocation.X, eLocation.Y, 10, 10);
                pictureBox1.Refresh();

            }
        }

        #endregion

        #region Picture Box Timers

        // Eraser Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }

        #endregion

        #region Buttons
        //Clear Button
        private void button1_Click(object sender, EventArgs e)
        {
            clearAll();
            draw = false;
            erase = false;
            pictureBox1.Refresh();
        }
        
        //Text Button
        private void button2_Click(object sender, EventArgs e)
        {
            draw = true;
            erase = false;
            shape = 11;
            pen.Brush = chooseColor(pen.Brush);
        }

        #endregion

        #region Text Fields

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            text = textBox1.Text;
        }
        #endregion

        #region Helper Functions
        private void chooseColor()
        {
            if (!(colorDialog1.ShowDialog() == DialogResult.Cancel))
            {
                pen.Color = colorDialog1.Color;

            }
        }

        private Brush chooseColor(Brush tool)
        {
            if (!(colorDialog1.ShowDialog() == DialogResult.Cancel))
            {
                tool = new SolidBrush(colorDialog1.Color);
                
                //var color = new Color();
                //color = colorDialog1.Color;
                //brush = color;
                //Brush brush = new SolidBrush(colorDialog1.Color);
            }
            return tool;
        }

        //private void earse(PointF start, PointF stop)
        //{
        //    float width = Math.Abs(stop.X - start.X);
        //    float height = Math.Abs(stop.Y - start.Y);
        //    RectangleF rect = new RectangleF(start.X, start.Y, width, height);  
        //    graphics.DrawEllipse(eraser, rect);
        //}

        private void clearAll()
        {
            graphics.Clear(pictureBox1.BackColor);
        }

        private String select_shape(int shape)
        {
            String json_Name = "";
            switch (shape)
            {
                case 1:
                    json_Name = "cross";
                    break;
                case 2:
                    json_Name = "diamond";
                    break;
                case 3:
                    json_Name = "isosceles";
                    break;
                case 4:
                    json_Name = "kite";
                    break;
                case 5:
                    json_Name = "parallelogram";
                    break;
                case 6:
                    json_Name = "rectangle";
                    break;
                case 7:
                    json_Name = "right_angle";
                    break;
                case 8:
                    json_Name = "star";
                    break;
                case 9:
                    json_Name = "trapezium";
                    break;
                case 10:
                    json_Name = "octagon";
                    break;
                default:
                    break;
            }
            return json_Name;
        }

        private List<PointF> loadPoints(String json_Name)
        {
            List<PointF> points = new List<PointF>();
            String jsonString = File.ReadAllText(json_Name+".json");
            points.Clear();
            points = JsonSerializer.Deserialize<List<PointF>>(jsonString);
            
            return points;
        }

        private PointF getCenter(List<PointF> points)
        {
            float x = 0;
            float y = 0;
            foreach (PointF point in points)
            {
                x += point.X;
                y += point.Y;
            }
            x = x / points.Count;
            y = y / points.Count;
            return new PointF(x, y);
        }

        

        private List<PointF> placeShape(List<PointF> points, PointF center)
        {
            List<PointF> temp = new List<PointF>();
            foreach (PointF point in points)
            {
                temp.Add(new PointF(point.X - center.X, point.Y - center.Y));
            }
            points.Clear();
            points = temp;

            return points;
        }

        #endregion
    }
}
