﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTest
{
    /// <summary>
    /// Test code for drawing lines and other objects on a page.
    /// </summary>
    public partial class MainForm : Form
    {
        private bool _drawLineMode { get; set; } = false;
        private Image pageImage { get; set; }
        private Point _startPoint { get; set; }
        private int ZoomLevel { get; set; } = 100;
        private bool ZoomToFit { get; set; } = true;



        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Resize(object sender, EventArgs e)
        {
            resizeImage();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //PanelShapeChanged();
            //ImageChanged();
        }

        private double aspectRatio(Size item)
        {
            return (double)item.Width / item.Height;
        }

        private void DisplayPage()
        {
            pbPageImage.Image = pageImage;
        }

        private void drawLine_Click(object sender, EventArgs e)
        {
            _drawLineMode = true;
            statusMessage.Text = "Draw line mode: hold the mouse button down to draw the line";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void drawOthers_Click(object sender, EventArgs e)
        {

        }

        private int FindCheckedRadioButton(GroupBox gb)
        {
            for (int index = 0; index < gb.Controls.Count; index++)
            {
                Control ccc = gb.Controls[index];
                if (ccc is RadioButton)
                {
                    RadioButton rb = (RadioButton)ccc;
                    if (rb.Checked)
                        return index;
                }
            }
            Debug.Fail("Scream at the top of my lungs \"Why wasn't a radio button selected?\"");
            return -1;
        }

        private void Flasks_Click(object sender, EventArgs e)
        {
            pageImage = Resource1.chemical_flasks_2_1417112;
            ImageChanged();
            DisplayPage();
        }

        private void FrontPage_Click(object sender, EventArgs e)
        {
            pageImage = Resource1.FrontPage;
            ImageChanged();
            DisplayPage();
        }


        private void ImageChanged()
        {
            foreach (Object item in groupBox1.Controls)
            {
                if (item is RadioButton)
                {
                    RadioButton rb = (RadioButton)item;
                    if (rb.Checked)
                    {
                        ImageChanged(rb);
                        break;
                    }
                }
            }
        }

        private void ImageChanged(object sender, EventArgs e)
        {
            ImageChanged((RadioButton)sender);
        }

        private void ImageChanged(RadioButton rb)
        {
            Size cell = picturePanel.Size;

            if (rb.Text == "Tall")
            {
                pbPageImage.Height = cell.Height;
                pbPageImage.Width = (int)((double)Size.Width / aspectRatio(cell));
                pbPageImage.Top = 0;
                pbPageImage.Left = (cell.Width - pbPageImage.Width) / 2;
            }
            else if (rb.Text == "Square")
            {
                pbPageImage.Size = cell;
                pbPageImage.Top = 0;
                pbPageImage.Left = 0;
            }
            else if (rb.Text == "Wide")
            {
                pbPageImage.Height = (int)((double)Size.Height * aspectRatio(cell));
                pbPageImage.Width = cell.Width;
                pbPageImage.Top = (cell.Height - pbPageImage.Height) / 2;
                pbPageImage.Left = 0;
            }
            else
            {
                Debug.Assert(false, "Wasn't expecting this!");
            }


            Image img = new Bitmap(pbPageImage.Width, pbPageImage.Height);
            Rectangle rect = new Rectangle(pbPageImage.Left + 2, pbPageImage.Top + 2, pbPageImage.Width - 4, pbPageImage.Height - 4);
            using (var graphics = Graphics.FromImage(img))
            {
                Pen bluePen = new Pen(Color.Blue, 3);
                Pen redPen = new Pen(Color.Red, 5);
                graphics.DrawRectangle(bluePen, rect);
                graphics.DrawEllipse(redPen, rect);
            }

            pageImage = img;
            DisplayPage();
        }

        /// <summary>
        /// Check to see if a point is inside the actual image
        /// </summary>
        /// 
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the image, false otherwise</returns>
        /// 
        private bool insideImage(Point point)
        {

            bool rv = false;
            //try
            //{
            //    // Do the clever stuff here
            //    Point pointPageImage = PointToScreen(new Point(pbPageImage.Bounds.Left, pbPageImage.Bounds.Top));
            //    Point pointCursor = Cursor.Position;

            //    pointPageImage.X = pointCursor.X - pointPageImage.X;
            //    pointPageImage.Y = pointCursor.Y - pointPageImage.Y;

            //    int _absoluteImagePositionX = pbPageImage.Width / 2 - pageImage.Width / 2;
            //    int _absoluteImagePositionY = pbPageImage.Height / 2 - pageImage.Height / 2;

            //    rv = pbPageImage.ClientRectangle.Contains(PointToClient(Control.MousePosition));

            //}
            //catch (Exception)
            //{
            //    // I don't care
            //}
            return rv;
        }

        private void pbPageImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (_drawLineMode)
            {
                _startPoint = e.Location;
                statusMessage.Text = "Draw line mode: release the mouse button to finish drawing the line";
                fromLocation.Text = "From: " + e.Location.ToString();
                fromLocation.Visible = true;
                toLocation.Visible = true;
            }
        }

        private void pbPageImage_MouseEnter(object sender, EventArgs e)
        {

        }

        private void pbPageImage_MouseHover(object sender, EventArgs e)
        {

        }

        private void pbPageImage_MouseLeave(object sender, EventArgs e)
        {
            if (_drawLineMode)
            {
                statusMessage.Text = "Draw line mode cancelled: mouse moved outside the image area";
                _drawLineMode = false;
            }
        }

        private void pbPageImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawLineMode)
            {
                toLocation.Text = "to: " + e.Location.ToString();
            }

            if (insideImage(e.Location))
            {
                hitCheck.Text = "Inside";
            }
            else
            {
                hitCheck.Text = "Outside";
            }
        }

        private void pbPageImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (_drawLineMode)
            {
                Point endPoint = e.Location;
                endPoint.Y = _startPoint.Y;
                _drawLineMode = false;
                toLocation.Text = "to: " + endPoint.ToString();
                statusMessage.Text = "Draw line mode ended: mouse button released";

                Pen blackPen = new Pen(Color.Black, 3);

                using (var graphics = pbPageImage.CreateGraphics())
                {
                    graphics.DrawLine(blackPen, _startPoint, endPoint);
                }
            }
        }

        private void resizeImage()
        {
            if (ZoomToFit)
            {
                picturePanel.AutoScroll = false;
                if (pageImage == null)
                {
                    //pbPageImage.Size = picturePanel.Size;
                }
                else
                {
                }
            }
            else
            {
                picturePanel.AutoScroll = true;
                pbPageImage.Width = ZoomLevel * picturePanel.Width / 100;
                pbPageImage.Height = ZoomLevel * picturePanel.Height / 100;
            }

            DisplayPage();
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            TableLayoutPanelCellPosition pos = tableLayoutPanel1.GetCellPosition(picturePanel);

            if (e.Column == pos.Column && e.Row == pos.Row)
            {
                // This is the cell we are interested in.
                PositionPictureBox(e.CellBounds);
            }
        }

        private void PositionPictureBox(Rectangle cellArea)
        {
            Rectangle rectangle = cellArea;
            rectangle.X = 0;
            rectangle.Y = 0;

            double cellAspectRatio = aspectRatio(cellArea.Size);

            switch (FindCheckedRadioButton(PanelGroupBox))
            {
                case 1: // Tall
                    rectangle.Width = (int)(rectangle.Height / cellAspectRatio);
                    rectangle.X = (cellArea.Width - rectangle.Width) / 2;
                    break;
                case 2: // Square
                    break;
                case 3: // Wide
                    rectangle.Height = (int)(rectangle.Width * cellAspectRatio);
                    rectangle.Y = (cellArea.Height - rectangle.Height) / 2;
                    break;
                default:
                    Debug.Fail("Give up now. It's not working.");
                    break;
            }

            picturePanel.Size = rectangle.Size;
            picturePanel.Location = rectangle.Location;
        }
    }
}
