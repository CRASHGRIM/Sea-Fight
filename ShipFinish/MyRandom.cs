using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ShipsWar
{
    public class MyRandomPoint
    {
        private List<Point> list;
        private Random X;
        public MyRandomPoint(int length)
        {
            X = new Random();
            
            list = new List<Point>();
            for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                    list.Add(new Point(i, j));
        }
        public Point Next()
        {
            var i = X.Next(list.Count);
            var result = list[i];
            list.RemoveAt(i);
            return result;
        }
    }
}
