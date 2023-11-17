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
        List<Object> history = new List<Object>();

        #region Structs

        #region Shape
        private struct Shape
        {
            public List<PointF> points;
            public Color color;
            public Pen pen;
            public Graphics graphics;

            public void redraw()
            {
                Pen p = new Pen(color, this.pen.Width);
                graphics.DrawLines(p, points.ToArray());
            }
        }
        #endregion

        #region Text
        private struct Text_
        {
            public String text;
            public Font font;
            public Brush brush;
            public PointF point;
            public Graphics graphics;

            public void redraw()
            {
                graphics.DrawString(text, font, brush, point);
            }
        }
        #endregion

        #region Eraser
        private struct Eraser
        {
            public Pen pen;
            public PointF[] points;
            public Graphics graphics;

            public void redraw()
            {
                if (points.Length > 1)
                {
                    graphics.DrawCurve(pen, points);
                }
            }
        }

        List<PointF> eraserPrev = new List<PointF>();
        #endregion


        #endregion

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
                this.origin = e.Location;
                this.eraserPrev.Add(origin);
                if (eraserPrev.Count > 1)
                {
                    graphics.DrawCurve(eraser, eraserPrev.ToArray());
                }
                pictureBox1.Refresh();
            }
            else if (draw == true && preview == true && shape != 0 && shape != 11)
            {
                this.origin = new PointF(e.X, e.Y);
                Pen p = new Pen(Color.Gray, 2);
                drawShape(this.graphics, p, this.shape, this.origin);
                pictureBox1.Refresh();
                reDrawAllFromList();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            erase = false;
            preview = false;

            if (draw == false)
            {
                Eraser eraser = new Eraser();
                eraser.pen = this.eraser;
                eraser.points = this.eraserPrev.ToArray();
                eraser.graphics = this.graphics;
                history.Add(eraser);
                this.eraserPrev.Clear();
            }

            this.origin = new PointF(e.X, e.Y);

            if (draw && shape != 11)
            {
                // Creates shape and saves it's info to a global list of struct type.
                Shape shape = new Shape();
                shape.points = drawShapeAndReturnPoints(this.graphics, this.pen, this.shape, this.origin);
                shape.color = this.pen.Color;
                shape.pen = this.pen;
                shape.graphics = this.graphics;
                history.Add(shape);
            }
            else if (draw && shape == 11)
            {
                //Prints text and saves it's info to a global list of struct type.
                Text_ text_ = new Text_();
                text_.text = this.text;
                text_.font = this.Font;
                text_.brush = this.pen.Brush;
                text_.point = this.origin;
                text_.graphics = this.graphics;
                history.Add(text_);
                graphics.DrawString(text_.text, text_.font, text_.brush, text_.point);
            }
            pictureBox1.Refresh();

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

        // Save button
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("mypic.png", ImageFormat.Png);
        }

        // Move right button
        private void button6_Click(object sender, EventArgs e)
        {
            applyOffset_X_ForLast(15);
            reDrawAllFromList();
            pictureBox1.Refresh();
        }

        // Move up button
        private void button7_Click(object sender, EventArgs e)
        {
            applyOffset_Y_ForLast(-15);
            reDrawAllFromList();
            pictureBox1.Refresh();
        }

        // Move left button
        private void button8_Click(object sender, EventArgs e)
        {
            applyOffset_X_ForLast(-15);
            reDrawAllFromList();
            pictureBox1.Refresh();
        }

        // Move down button
        private void button9_Click(object sender, EventArgs e)
        {
            applyOffset_Y_ForLast(15);
            reDrawAllFromList();
            pictureBox1.Refresh();
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
            this.history.Clear();
            textBox1.Clear();
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

        private void drawShape(Graphics graphics, Pen pen, int shape, PointF origin)
        {
            List<PointF> points = translateShape(loadPoints(select_shape(shape)), origin);
            graphics.DrawLines(pen, points.ToArray());
        }

        private List<PointF> drawShapeAndReturnPoints(Graphics graphics, Pen pen, int shape, PointF origin)
        {
            List<PointF> points = translateShape(loadPoints(select_shape(shape)), origin);
            graphics.DrawLines(pen, points.ToArray());
            return points;
        }

        //private List<Object> moveShapeX(List<Object> arr, int step)
        //{
        //    List<Object> tempArr = arr;
        //    if (arr.Count > 0 && arr[arr.Count - 1].GetType() == typeof(Shape))
        //    {
        //        //List<PointF> points = ((Shape)arr[arr.Count - 1]).points;
        //        Shape shape = (Shape)arr[arr.Count - 1];
        //        tempArr.RemoveAt(tempArr.Count - 1);


        //        List<PointF> translated = new List<PointF>();
        //        foreach (PointF point in shape.points)
        //        {
        //            translated.Add(new PointF(point.X + step, point.Y));
        //        }

        //        shape.points.Clear();
        //        shape.points.AddRange(translated);
        //        tempArr.Add((Object)shape);
        //        //shape.redraw();
        //    }
        //    return tempArr;
        //}

        private void applyOffset_X_ForLast(int offset)
        { 
            var last = history.LastOrDefault();
            if (last != null)
            {
                if (last.GetType() == typeof(Shape))
                {
                    Shape shape = (Shape)last;
                    List<PointF> translated = new List<PointF>();
                    foreach (PointF point in shape.points)
                    {
                        translated.Add(new PointF(point.X + offset, point.Y));
                    }
                    shape.points.Clear();
                    shape.points.AddRange(translated);
                    history.RemoveAt(history.Count - 1);
                    history.Add((Object)shape);
                }
            }

        }

        private void applyOffset_Y_ForLast(int offset)
        {
            var last = history.LastOrDefault();
            if (last != null)
            {
                if (last.GetType() == typeof(Shape))
                {
                    Shape shape = (Shape)last;
                    List<PointF> translated = new List<PointF>();
                    foreach (PointF point in shape.points)
                    {
                        translated.Add(new PointF(point.X, point.Y + offset));
                    }
                    shape.points.Clear();
                    shape.points.AddRange(translated);
                    history.RemoveAt(history.Count - 1);
                    history.Add((Object)shape);
                }
            }

        }

        private void reDrawAllFromList()
        {
            graphics.Clear(pictureBox1.BackColor);
            foreach (Object obj in history)
            {
                if (obj.GetType() == typeof(Shape))
                
                {
                    ((Shape)obj).redraw();
                    //Pen p = new Pen((Shape)obj).color, this.pen.Width);
                    //graphics.DrawLines(p, points.ToArray());

                }
                else if (obj.GetType() == typeof(Text_))
                {
                    ((Text_)obj).redraw();
                }
                else if (obj.GetType() == typeof(Eraser))
                {
                    ((Eraser)obj).redraw();
                }
            }
        }

        #endregion
    }
}
