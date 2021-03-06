using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;

namespace BitmapGauge
{
    public partial class BitmapGauge 
    {
        #region Private Attributes
        public Font font { get; set; } = new Font(FontFamily.GenericSansSerif, 8);
        private float minValue= 0;
        private float maxValue = 100;
        private float threshold = 25;
        private float currentValue = 0;
        private float recommendedValue = 25;
        private int noOfDivisions = 10;
        private int noOfSubDivisions=3;
        private string dialText;
        private Color ForeColor = Color.Black;
        private Color dialColor = Color.FromArgb(230, 230, 250);
        private float glossinessAlpha = 72;//25;
        private int oldWidth, oldHeight;
        int x, y, width, height;
        float fromAngle = 135F;
        float toAngle = 405F;
        private bool enableTransparentBackground = false;
        private bool requiresRedraw;
        private Image backgroundImg;
        private Rectangle rectImg;
        Bitmap bmp;
        //Graphics gfx;
        #endregion

        public int Width { get; set; }
        public int Height { get; set; }

        public Color BackColor { get; set; } = Color.White;

        public BitmapGauge(int w,int h)
        {
            this.Width = w;
            this.Height = h;
            
          
            
            this.noOfDivisions = 10;
            this.noOfSubDivisions = 3;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent;
            //this.Resize += new EventHandler(AquaGauge_Resize);
            this.requiresRedraw = true;
            Resize();
        }

        #region Public Properties
        /// <summary>
        /// Mininum value on the scale
        /// </summary>
     
        public float MinValue
        {
            get { return minValue; }
            set
            {
                if (value < maxValue)
                {
                    minValue = value;
                    if (currentValue < minValue)
                        currentValue = minValue;
                    if (recommendedValue < minValue)
                        recommendedValue = minValue;
                    requiresRedraw = true;
                    
                }
            }
        } 

        /// <summary>
        /// Maximum value on the scale
        /// </summary>
       
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                if (value > minValue)
                {
                    maxValue = value;
                    if (currentValue > maxValue)
                        currentValue = maxValue;
                    if (recommendedValue > maxValue)
                        recommendedValue = maxValue;
                    requiresRedraw = true;
                   
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Threshold area from the Recommended Value. (1-99%)
        /// </summary>
        public float ThresholdPercent
        {
            get { return threshold; }
            set
            {
                if (value > 0 && value < 100)
                {
                    threshold = value;
                    requiresRedraw = true;
                   
                }
            }
        }

        /// <summary>
        /// Threshold value from which green area will be marked.
        /// </summary>
        public float RecommendedValue
        {
            get { return recommendedValue; }
            set
            {
                if (value > minValue && value < maxValue)
                {
                    recommendedValue = value;
                    requiresRedraw = true;
                    
                }
            }
        }

        /// <summary>
        /// Value where the pointer will point to.
        /// </summary>
        public float Value
        {
            get { return currentValue; }
            set
            {
                if (value >= minValue && value <= maxValue)
                {
                    currentValue = value;
                    
                }
            }
        }

        /// <summary>
        /// Background color of the dial
        /// </summary>
        public Color DialColor
        {
            get { return dialColor; }
            set
            {
                dialColor = value;
                requiresRedraw = true;
                
            }
        }

        /// <summary>
        /// Glossiness strength. Range: 0-100
        /// </summary>
      
        public float Glossiness
        {
            get
            {
                return (glossinessAlpha * 100) / 220;
            }
            set
            {
                float val = value;
                if (val > 100)
                    value = 100;
                if (val < 0)
                    value = 0;
                glossinessAlpha = (value * 220) / 100;
               
            }
        }

        /// <summary>
        /// Get or Sets the number of Divisions in the dial scale.
        /// </summary>
        public int NoOfDivisions
        {
            get { return this.noOfDivisions; }
            set
            {
                if (value > 1 && value < 25)
                {
                    this.noOfDivisions = value;
                    requiresRedraw = true;
                  
                }
            }
        }

        /// <summary>
        /// Gets or Sets the number of Sub Divisions in the scale per Division.
        /// </summary>
        public int NoOfSubDivisions
        {
            get { return this.noOfSubDivisions; }
            set
            {
                if (value > 0 && value <= 10)
                {
                    this.noOfSubDivisions = value;
                    requiresRedraw = true;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the Text to be displayed in the dial
        /// </summary>
        public string DialText
        {
            get { return this.dialText; }
            set
            {
                this.dialText = value;
                requiresRedraw = true;
           
            }
        }

        /// <summary>
        /// Enables or Disables Transparent Background color.
        /// Note: Enabling this will reduce the performance and may make the control flicker.
        /// </summary>
        public bool EnableTransparentBackground
        {
            get { return this.enableTransparentBackground; }
            set
            {
                this.enableTransparentBackground = value;
                requiresRedraw = true;
                
            }
        }
        #endregion

        #region Overriden Control methods
        /// <summary>
        /// Draws the pointer.
        /// </summary>
        /// <param name="e"></param>
        public Bitmap GetGauge()
        {
            width = this.Width - x * 2;
            height = this.Height - y * 2;
            var gfx = Graphics.FromImage(bmp);
            PaintBackground(gfx);
            DrawPointer(gfx,((width) / 2) + x, ((height) / 2) + y);
            return bmp;
        }

        /// <summary>
        /// Draws the dial background.
        /// </summary>
        /// <param name="e"></param>
        protected void PaintBackground(Graphics gfx)
        {
            if (!enableTransparentBackground)
            {
                //base.OnPaintBackground(e);
            }

            //e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            gfx.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, Width, Height);
            if (backgroundImg == null || requiresRedraw)
            {
                backgroundImg = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(backgroundImg);
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                width = this.Width - x * 2;
                height = this.Height - y * 2;
                rectImg = new Rectangle(x, y, width, height);

                //Draw background color
                Brush backGroundBrush = new SolidBrush(Color.FromArgb(120, dialColor));
                if (enableTransparentBackground)
                {
                    float gg = width / 60;
                    //g.FillEllipse(new SolidBrush(this.Parent.BackColor), -gg, -gg, this.Width+gg*2, this.Height+gg*2);
                }
                g.FillEllipse(backGroundBrush, x, y, width, height);

                //Draw Rim
                SolidBrush outlineBrush = new SolidBrush(Color.FromArgb(100, Color.FromArgb(112, 128, 144)));//gray
                Pen outline = new Pen(outlineBrush, (float)(width * .03));
                g.DrawEllipse(outline, rectImg.X, rectImg.Y, rectImg.Width, rectImg.Height);
                Pen darkRim = new Pen(Color.FromArgb(112, 128, 144));//gray
                g.DrawEllipse(darkRim, x, y, width, height);

                //Draw Callibration
                DrawCalibration(g,rectImg, ((width) / 2) + x, ((height) / 2) + y);

                //Draw Colored Rim
                Pen colorPen = new Pen(Color.FromArgb(190, Color.FromArgb(220, 220, 220)), this.Width / 40);
                Pen blackPen = new Pen(Color.FromArgb(250, Color.Black), this.Width / 200);
                int gap = (int)(this.Width * 0.03F);
                Rectangle rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                DrawArc(g,colorPen, rectg, 135, 270);

                //Draw Threshold
                colorPen = new Pen(Color.FromArgb(200, Color.FromArgb(124, 252, 0)), this.Width / 50);
                rectg = new Rectangle(rectImg.X + gap, rectImg.Y + gap, rectImg.Width - gap * 2, rectImg.Height - gap * 2);
                float val = MaxValue - MinValue;
                val = (100 * (this.recommendedValue - MinValue)) / val;
                val = ((toAngle - fromAngle) * val) / 100;
                val += fromAngle;
                float stAngle = val - ((270 * threshold) / 200);
                if (stAngle <= 135) stAngle = 135;
                float sweepAngle = ((270 * threshold) / 100);
                if (stAngle + sweepAngle > 405) sweepAngle = 405 - stAngle;
                DrawArc(g,colorPen, rectg, stAngle, sweepAngle);

                //Draw Digital Value
                Rectangle digiRect = new Rectangle((int)(this.Width / 2F - (int)this.width / 5F), (int)(this.height / 1.2F), (int)(this.width / 2.5F), (int)(this.Height / 9F));
                Rectangle digiFRect = new Rectangle((int)(this.Width / 2 - this.width / 7), (int)(this.height / 1.18), (int)(this.width / 4), (int)(this.Height / 12));
                g.FillRectangle(new SolidBrush(Color.FromArgb(30, Color.Gray)), digiRect.X, digiRect.Y, digiRect.Width, digiRect.Height);
                DisplayNumber(g, this.currentValue, digiFRect);

                SizeF textSize = g.MeasureString(this.dialText, this.font);
                RectangleF digiFRectText = new RectangleF(this.Width / 2 - textSize.Width / 2, (int)(this.height / 1.5), textSize.Width, textSize.Height);
                g.DrawString(dialText, this.font, new SolidBrush(this.ForeColor), digiFRectText);
                requiresRedraw = false;
            }
            gfx.DrawImage(backgroundImg, rectImg.X,rectImg.Y);
        }

        void DrawArc(Graphics g, Pen pen, Rectangle rect, double StartAngle,double SweepAngle)
        {
            g.DrawArc(pen, rect, (float)StartAngle, (float)SweepAngle);
            /*
            int ax, ay;
            var start_angle = StartAngle.ToRadians();
            var end_angle = SweepAngle.ToRadians();
            var r = rect.Width/2;
            for (var i = start_angle; i < end_angle; i = i + 0.05)
            {
                ax = rect.X+ (int)(r + Math.Cos(i) * r);
                ay = rect.Y+ (int)(r + Math.Sin(i) * r);
                var solid = new SolidBrush(pen.Color);
                g.FillRectangle(solid, ax, ay, (int)pen.Width, (int)pen.Width); // center point is (x = 50, y = 100)
            }*/
        }

       
        #endregion

        #region Private methods
        /// <summary>
        /// Draws the Pointer.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        private void DrawPointer(Graphics gr, int cx, int cy)
        {
            float radius = this.Width / 2 - (this.Width * .12F);
            float val = MaxValue - MinValue;

            Image img = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(img);
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            val = (100 * (this.currentValue - MinValue)) / val;
            val = ((toAngle - fromAngle) * val) / 100;
            val += fromAngle;

            float angle = GetRadian(val);
            float gradientAngle = angle;

            PointF[] pts = new PointF[5];

            pts[0].X = (float)(cx + radius * Math.Cos(angle));
            pts[0].Y = (float)(cy + radius * Math.Sin(angle));

            pts[4].X = (float)(cx + radius * Math.Cos(angle - 0.02));
            pts[4].Y = (float)(cy + radius * Math.Sin(angle - 0.02));

            angle = GetRadian((val + 20));
            pts[1].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            pts[1].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            pts[2].X = cx;
            pts[2].Y = cy;

            angle = GetRadian((val - 20));
            pts[3].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            pts[3].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            Brush pointer = new SolidBrush(Color.Black);
            FillPolygon(g,pointer, pts);

            PointF[] shinePts = new PointF[3];
            angle = GetRadian(val);
            shinePts[0].X = (float)(cx + radius * Math.Cos(angle));
            shinePts[0].Y = (float)(cy + radius * Math.Sin(angle));

            angle = GetRadian(val + 20);
            shinePts[1].X = (float)(cx + (this.Width * .09F) * Math.Cos(angle));
            shinePts[1].Y = (float)(cy + (this.Width * .09F) * Math.Sin(angle));

            shinePts[2].X = cx;
            shinePts[2].Y = cy;

            SolidBrush gpointer = new SolidBrush(Color.FromArgb(112, 128, 144)); //(shinePts[0], shinePts[2], Color.SlateGray, Color.Black);
            FillPolygon(g,gpointer, shinePts);

            Rectangle rect = new Rectangle(x, y, width, height);
            DrawCenterPoint(g, rect, ((width) / 2) + x, ((height) / 2) + y);

            DrawGloss(g);

            gr.DrawImage(img, 0, 0);
        }
        void FillPolygon(Graphics g,Brush brush, PointF[] Points)
        {
            g.FillPolygon(brush, Points);
            /*
            if (Points.Length <= 1) return;
            var pen = new Pen(brush);
            for (var i = 0; i < Points.Length; i++)
            {
                if (i + 1 < Points.Length)
                {
                    var Src = Points[i];
                    var Dst = Points[i + 1];
                    g.DrawLine(pen, (int)Src.X, (int)Src.Y, (int)Dst.X, (int)Dst.Y);
                }
            }*/
        }
        /// <summary>
        /// Draws the glossiness.
        /// </summary>
        /// <param name="g"></param>
        private void DrawGloss(Graphics g)
        {
            RectangleF glossRect = new RectangleF(
               x + (float)(width * 0.10),
               y + (float)(height * 0.07),
               (float)(width * 0.80),
               (float)(height * 0.7));
            var gradientBrush = 
                new LinearGradientBrush(glossRect,
                Color.FromArgb((int)glossinessAlpha, Color.White),
                Color.Transparent,
                LinearGradientMode.Vertical);
                
            g.FillEllipse(gradientBrush, (int)glossRect.X, (int)glossRect.Y, (int)glossRect.Width, (int)glossRect.Height);

            //TODO: Gradient from bottom
            glossRect = new RectangleF(
               x + (float)(width * 0.25),
               y + (float)(height * 0.77),
               (float)(width * 0.50),
               (float)(height * 0.2));
            int gloss = (int)(glossinessAlpha / 3);
            gradientBrush =
                new LinearGradientBrush(glossRect,
                Color.Transparent, Color.FromArgb(gloss, this.BackColor),
                LinearGradientMode.Vertical);
            
            g.FillEllipse(gradientBrush, (int)glossRect.X, (int)glossRect.Y, (int)glossRect.Width, (int)glossRect.Height);
        }

        /// <summary>
        /// Draws the center point.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        private void DrawCenterPoint(Graphics g, Rectangle rect, int cX, int cY)
        {
            float shift = Width / 5;
            RectangleF rectangle = new RectangleF(cX - (shift / 2), cY - (shift / 2), shift, shift);
            var brush = new SolidBrush(Color.FromArgb(100, this.dialColor)); //LinearGradientBrush(rect, Color.Black, Color.FromArgb(100, this.dialColor), LinearGradientMode.Vertical);
            g.FillEllipse(brush, (int) rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);

            shift = Width / 7;
            rectangle = new RectangleF(cX - (shift / 2), cY - (shift / 2), shift, shift);
            brush = new SolidBrush(Color.FromArgb(112, 128, 144));//new LinearGradientBrush(rect, Color.SlateGray, Color.Black, LinearGradientMode.ForwardDiagonal);
            g.FillEllipse(brush, (int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        /// Draws the Ruler
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="cX"></param>
        /// <param name="cY"></param>
        private void DrawCalibration(Graphics g, Rectangle rect, int cX, int cY)
        {
            int noOfParts = this.noOfDivisions + 1;
            int noOfIntermediates = this.noOfSubDivisions;
            float currentAngle = GetRadian(fromAngle);
            int gap = (int)(this.Width * 0.01F);
            float shift = this.Width / 25;
            Rectangle rectangle = new Rectangle(rect.X + gap, rect.Y + gap, rect.Width - gap, rect.Height - gap);

            int x, y, x1, y1;
            float tx, ty, radius;
            radius = rectangle.Width / 2 - gap * 5;
            float totalAngle = toAngle - fromAngle;
            float incr = GetRadian(((totalAngle) / ((noOfParts - 1) * (noOfIntermediates + 1))));

            Pen thickPen = new Pen(Color.Black, Width / 50);
            Pen thinPen = new Pen(Color.Black, Width / 100);
            float rulerValue = MinValue;
            for (int i = 0; i <= noOfParts; i++)
            {
                //Draw Thick Line
                x = (int)(cX + radius * Math.Cos(currentAngle));
                y = (int)(cY + radius * Math.Sin(currentAngle));
                x1 = (int)(cX + (radius - Width / 20) * Math.Cos(currentAngle));
                y1 = (int)(cY + (radius - Width / 20) * Math.Sin(currentAngle));
                g.DrawLine(thickPen, x, y, x1, y1);

                //Draw Strings
                StringFormat format = new StringFormat();
                tx = (float)(cX + (radius - Width / 10) * Math.Cos(currentAngle));
                ty = (float)(cY - shift + (radius - Width / 10) * Math.Sin(currentAngle));
                Brush stringPen = new SolidBrush(this.ForeColor);
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                g.DrawString(rulerValue.ToString() + "", font, stringPen,  tx, ty);
                rulerValue += (float)((MaxValue - MinValue) / (noOfParts - 1));
                rulerValue = (float)Math.Round(rulerValue);

                //currentAngle += incr;
                if (i == noOfParts - 1)
                    break;
                for (int j = 0; j <= noOfIntermediates; j++)
                {
                    //Draw thin lines 
                    currentAngle += incr;
                    x = (int)(cX + radius * Math.Cos(currentAngle));
                    y = (int)(cY + radius * Math.Sin(currentAngle));
                    x1 = (int)(cX + (radius - Width / 50) * Math.Cos(currentAngle));
                    y1 = (int)(cY + (radius - Width / 50) * Math.Sin(currentAngle));
                    g.DrawLine(thinPen, x, y, x1, y1);
                }
            }
        }

        /// <summary>
        /// Converts the given degree to radian.
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        public float GetRadian(float theta)
        {
            return theta * (float)Math.PI / 180F;
        }

        /// <summary>
        /// Displays the given number in the 7-Segement format.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="number"></param>
        /// <param name="drect"></param>
        private void DisplayNumber(Graphics g, float number, Rectangle drect)
        {
            try
            {
                string num = number.ToString("000.00");
                num.PadLeft(3, '0');
                float shift = 0;
                if (number < 0)
                {
                    shift -= width / 17;
                }
                bool drawDPS = false;
                char[] chars = num.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    char c = chars[i];
                    if (i < chars.Length - 1 && chars[i + 1] == '.')
                        drawDPS = true;
                    else
                        drawDPS = false;
                    if (c != '.')
                    {
                        if (c == '-')
                        {
                            DrawDigit(g, -1, new PointF(drect.X + shift, drect.Y), drawDPS, drect.Height);
                        }
                        else
                        {
                            DrawDigit(g, Int16.Parse(c.ToString()), new PointF(drect.X + shift, drect.Y), drawDPS, drect.Height);
                        }
                        shift += 15 * this.width / 250;
                    }
                    else
                    {
                        shift += 2 * this.width / 250;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Draws a digit in 7-Segement format.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="number"></param>
        /// <param name="position"></param>
        /// <param name="dp"></param>
        /// <param name="height"></param>
        private void DrawDigit(Graphics g, int number, PointF position, bool dp, float height)
        {
            float width;
            width = 10F * height / 13;

            Pen outline = new Pen(Color.FromArgb(40, this.dialColor));
            Pen fillPen = new Pen(Color.Black);

            #region Form Polygon Points
            //Segment A
            PointF[] segmentA = new PointF[5];
            segmentA[0] = segmentA[4] = new PointF(position.X + GetX(2.8F, width), position.Y + GetY(1F, height));
            segmentA[1] = new PointF(position.X + GetX(10, width), position.Y + GetY(1F, height));
            segmentA[2] = new PointF(position.X + GetX(8.8F, width), position.Y + GetY(2F, height));
            segmentA[3] = new PointF(position.X + GetX(3.8F, width), position.Y + GetY(2F, height));

            //Segment B
            PointF[] segmentB = new PointF[5];
            segmentB[0] = segmentB[4] = new PointF(position.X + GetX(10, width), position.Y + GetY(1.4F, height));
            segmentB[1] = new PointF(position.X + GetX(9.3F, width), position.Y + GetY(6.8F, height));
            segmentB[2] = new PointF(position.X + GetX(8.4F, width), position.Y + GetY(6.4F, height));
            segmentB[3] = new PointF(position.X + GetX(9F, width), position.Y + GetY(2.2F, height));

            //Segment C
            PointF[] segmentC = new PointF[5];
            segmentC[0] = segmentC[4] = new PointF(position.X + GetX(9.2F, width), position.Y + GetY(7.2F, height));
            segmentC[1] = new PointF(position.X + GetX(8.7F, width), position.Y + GetY(12.7F, height));
            segmentC[2] = new PointF(position.X + GetX(7.6F, width), position.Y + GetY(11.9F, height));
            segmentC[3] = new PointF(position.X + GetX(8.2F, width), position.Y + GetY(7.7F, height));

            //Segment D
            PointF[] segmentD = new PointF[5];
            segmentD[0] = segmentD[4] = new PointF(position.X + GetX(7.4F, width), position.Y + GetY(12.1F, height));
            segmentD[1] = new PointF(position.X + GetX(8.4F, width), position.Y + GetY(13F, height));
            segmentD[2] = new PointF(position.X + GetX(1.3F, width), position.Y + GetY(13F, height));
            segmentD[3] = new PointF(position.X + GetX(2.2F, width), position.Y + GetY(12.1F, height));

            //Segment E
            PointF[] segmentE = new PointF[5];
            segmentE[0] = segmentE[4] = new PointF(position.X + GetX(2.2F, width), position.Y + GetY(11.8F, height));
            segmentE[1] = new PointF(position.X + GetX(1F, width), position.Y + GetY(12.7F, height));
            segmentE[2] = new PointF(position.X + GetX(1.7F, width), position.Y + GetY(7.2F, height));
            segmentE[3] = new PointF(position.X + GetX(2.8F, width), position.Y + GetY(7.7F, height));

            //Segment F
            PointF[] segmentF = new PointF[5];
            segmentF[0] = segmentF[4] = new PointF(position.X + GetX(3F, width), position.Y + GetY(6.4F, height));
            segmentF[1] = new PointF(position.X + GetX(1.8F, width), position.Y + GetY(6.8F, height));
            segmentF[2] = new PointF(position.X + GetX(2.6F, width), position.Y + GetY(1.3F, height));
            segmentF[3] = new PointF(position.X + GetX(3.6F, width), position.Y + GetY(2.2F, height));

            //Segment G
            PointF[] segmentG = new PointF[7];
            segmentG[0] = segmentG[6] = new PointF(position.X + GetX(2F, width), position.Y + GetY(7F, height));
            segmentG[1] = new PointF(position.X + GetX(3.1F, width), position.Y + GetY(6.5F, height));
            segmentG[2] = new PointF(position.X + GetX(8.3F, width), position.Y + GetY(6.5F, height));
            segmentG[3] = new PointF(position.X + GetX(9F, width), position.Y + GetY(7F, height));
            segmentG[4] = new PointF(position.X + GetX(8.2F, width), position.Y + GetY(7.5F, height));
            segmentG[5] = new PointF(position.X + GetX(2.9F, width), position.Y + GetY(7.5F, height));

            //Segment DP
            #endregion

            #region Draw Segments Outline
            FillPolygon(g,outline.Brush, segmentA);
            FillPolygon(g,outline.Brush, segmentB);
            FillPolygon(g,outline.Brush, segmentC);
            FillPolygon(g,outline.Brush, segmentD);
            FillPolygon(g,outline.Brush, segmentE);
            FillPolygon(g,outline.Brush, segmentF);
            FillPolygon(g,outline.Brush, segmentG);
            #endregion

            #region Fill Segments
            //Fill SegmentA
            if (IsNumberAvailable(number, 0, 2, 3, 5, 6, 7, 8, 9))
            {
                FillPolygon(g,fillPen.Brush, segmentA);
            }

            //Fill SegmentB
            if (IsNumberAvailable(number, 0, 1, 2, 3, 4, 7, 8, 9))
            {
                FillPolygon(g,fillPen.Brush, segmentB);
            }

            //Fill SegmentC
            if (IsNumberAvailable(number, 0, 1, 3, 4, 5, 6, 7, 8, 9))
            {
                FillPolygon(g,fillPen.Brush, segmentC);
            }

            //Fill SegmentD
            if (IsNumberAvailable(number, 0, 2, 3, 5, 6, 8, 9))
            {
                FillPolygon(g,fillPen.Brush, segmentD);
            }

            //Fill SegmentE
            if (IsNumberAvailable(number, 0, 2, 6, 8))
            {
                FillPolygon(g,fillPen.Brush, segmentE);
            }

            //Fill SegmentF
            if (IsNumberAvailable(number, 0, 4, 5, 6, 7, 8, 9))
            {
                FillPolygon(g,fillPen.Brush, segmentF);
            }

            //Fill SegmentG
            if (IsNumberAvailable(number, 2, 3, 4, 5, 6, 8, 9, -1))
            {
                FillPolygon(g,fillPen.Brush, segmentG);
            }
            #endregion

            //Draw decimal point
            if (dp)
            {
                    g.FillEllipse(fillPen.Brush, 
                    (int)(position.X + GetX(10F, width)),
                    (int)(position.Y + GetY(12F, height)),
                    (int)(width / 7),
                    (int)(width / 7));
            }
        }

        /// <summary>
        /// Gets Relative X for the given width to draw digit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private float GetX(float x, float width)
        {
            return x * width / 12;
        }

        /// <summary>
        /// Gets relative Y for the given height to draw digit
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private float GetY(float y, float height)
        {
            return y * height / 15;
        }

        /// <summary>
        /// Returns true if a given number is available in the given list.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="listOfNumbers"></param>
        /// <returns></returns>
        private bool IsNumberAvailable(int number, params int[] listOfNumbers)
        {
            if (listOfNumbers.Length > 0)
            {
                foreach (int i in listOfNumbers)
                {
                    if (i == number)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Restricts the size to make sure the height and width are always same.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resize()
        {
        
            if (this.Width < 136)
            {
                this.Width = 136;
            }

         

            if (oldWidth != this.Width)
            {
                this.Height = this.Width;
                oldHeight = this.Width;
            }
            if (oldHeight != this.Height)
            {
                this.Width = this.Height;
                oldWidth = this.Width;
            }

            x = 5;
            y = 5;
            width = this.Width - 10;
            height = this.Height - 10;

            bmp = new Bitmap(Width+5,Height+5);

           
        }
        #endregion
    }

    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    } 
    public static class StringExtensions
    {
        public static void PadLeft(this string val,int count,char c)
        {
            var iter =  count - val.Length;
            var add = "";
            if (iter > 0){
                for(var i=0; i<iter;i++)
                    add += c;
            }
            val = add + val;
            
        }
    }
}
