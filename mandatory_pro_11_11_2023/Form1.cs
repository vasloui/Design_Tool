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
using System.Threading;
using System.Drawing.Imaging;

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
        bool preview;
        PointF origin;
        
        private struct Shape
        {
            public List<PointF> points;
            public Color color;
        }

        List<Shape> drawn_shapes = new List<Shape>();

        private struct Text_
        {
            public String text;
            public Font font;
            public Brush brush;
            public PointF point;
        }

        List<Text_> placed_text = new List<Text_>();


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
            draw = true;
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

        private void arrowToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            erase = !draw;
            preview = true;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (erase)
            {
                PointF eLocation = e.Location;
                graphics.DrawEllipse(eraser, eLocation.X, eLocation.Y, 10, 10);
                pictureBox1.Refresh();
            }
            else if (draw == true && preview == true && shape != 0 && shape != 11)
            {
                this.origin = new PointF(e.X, e.Y);
                Pen p = new Pen(Color.Gray, 2);
                drawShape(this.graphics, p, this.shape, this.origin);
                pictureBox1.Refresh();
                redrawAll();
             }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            erase = false;
            preview = false;

            this.origin = new PointF(e.X, e.Y);

            if (draw && shape != 11)
            {
                // Creates shape and saves it's info to a global list of struct type.
                Shape shape = new Shape();
                shape.points = drawShape(this.graphics, this.pen, this.shape, this.origin);
                shape.color = this.pen.Color;
                this.drawn_shapes.Add(shape);
                

            }
            else if (draw && shape == 11)
            {
                //Prints text and saves it's info to a global list of struct type.
                Text_ text_ = new Text_();
                text_.text = this.text;
                text_.font = this.Font;
                text_.brush = this.pen.Brush;
                text_.point = this.origin;
                this.placed_text.Add(text_);
                graphics.DrawString(text_.text, text_.font, text_.brush, text_.point);

            }
            pictureBox1.Refresh();
            
        }

        #endregion

        #region Picture Box Timers

        // Eraser Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (
            //    (pictureBox1.Width != this.Width) &&
            //    (pictureBox1.Height != this.Height)
            //)
            //{
            //    pictureBox1.Width = Form1.ActiveForm.Width;
            //    pictureBox1.Height = Form1.ActiveForm.Height;
            //    this.bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //    this.graphics = Graphics.FromImage(this.bitmap);
            //}
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
                this.pen.Color = colorDialog1.Color;

            }
        }

        private Brush chooseColor(Brush tool)
        {
            if (!(colorDialog1.ShowDialog() == DialogResult.Cancel))
            {
                tool = new SolidBrush(colorDialog1.Color);
            }
            return tool;
        }


        private void clearAll()
        {
            graphics.Clear(pictureBox1.BackColor);
            this.drawn_shapes.Clear();
            this.placed_text.Clear();
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
                    json_Name = "arrow";
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
            String jsonString = File.ReadAllText(json_Name + ".json");
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

        // The origin is the point where the mouse is clicked.
        private List<PointF> translateShape(List<PointF> points, PointF origin)
        {

            List<PointF> translated = new List<PointF>();
            PointF center = getCenter(points);
            float translationX = center.X - origin.X;
            float translationY = center.Y - origin.Y;

            // Applies the translation to each point.
            foreach (PointF point in points)
            {
                translated.Add(new PointF(point.X - translationX, point.Y - translationY));
            }

            return translated;
        }

        private List<PointF> drawShape(Graphics graphics, Pen pen, int shape, PointF origin)
        {
            List<PointF> points = translateShape(loadPoints(select_shape(shape)), origin);
            graphics.DrawLines(pen, points.ToArray());
            return points;
        }

        private void redrawAll()
        {
            graphics.Clear(pictureBox1.BackColor);
            foreach (Shape shape in this.drawn_shapes)
            {
                Pen p = new Pen(shape.color, this.pen.Width);
               graphics.DrawLines(p, shape.points.ToArray());
            }

            foreach (Text_ text_ in this.placed_text)
            {
                graphics.DrawString(text_.text, text_.font, text_.brush, text_.point);
            }
        }


        #endregion
    }
}
