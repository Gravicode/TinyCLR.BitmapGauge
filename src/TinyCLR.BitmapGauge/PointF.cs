using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TinyCLR.BitmapGauge
{
    public class PointF
    {
        public PointF(float ax, float ay)
        {
            this.X = ax;
            this.Y = ay;

        }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
