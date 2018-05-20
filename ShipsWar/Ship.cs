using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShipsWar
{
    class Ship
    {
        public int Size;
        public int Angle;
        public List<Point> Cells;

        public Ship(Point location, int angle, int size)
        {
            Size = size;
            Cells = new List<Point>(size);
            Angle = angle;
            for (int i = 0; i < size; i++)
                Cells.Add(new Point(Convert.ToInt32(location.X - i * Math.Cos(angle*Math.PI/180)), Convert.ToInt32(location.Y - i * Math.Sin(angle * Math.PI / 180))));
        }
    }
}
