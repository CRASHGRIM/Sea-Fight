using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShipsWar
{
    public class Map
    {
        public static readonly Point[] Delta = new Point[] {
            new Point(-1,0),
            new Point(1,0),
            new Point(0,1),
            new Point(0,-1)};
        public static readonly Point[] DeltaD = new Point[] {
            new Point(-1,1),
            new Point(1,1),
            new Point(1,-1),
            new Point(-1,-1)};

        public State[,] Location;
        public int AmountOfAliveShips;
        public int CountDeadCells()
        {
            int i = 0;
            foreach (var cell in Location)
                if (cell == State.KilledShip || cell==State.ShootedShip)
                    i++;
            return i;
        }
        public int CountAliveShips()
        {
            int i = 0;
            foreach (var cell in Location)
                if (cell == State.Ship)
                    i++;
            return i;
        }
        public int CountDeadShips;

        public Map(string stringMap)
        {
            CountDeadShips = 0;
            AmountOfAliveShips = 0;
            Location = CreateMap(stringMap);
            AmountOfAliveShips = CountAliveShips();
        }

        public Map(int size)
        {
            CountDeadShips = 0;
            AmountOfAliveShips = 0;
            Location = new State[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    Location[i, j] = State.Empty;
        }

        public State this[int i, int j]
        {
            get { return Location[i, j]; }
            private set {  this.Location[i, j] = value; }
        }
        
        public void SetPosition(Point pos, State state)
        {
            this[pos] = state;
        }
        

        public bool IsShipDead(Point pos, HashSet<Point> alreadyChecked)
        {
            alreadyChecked.Add(pos);
            foreach (var del in Delta)
            {
                var newpos = SumPoint(pos, del);
                if (this[newpos] == State.Ship)
                    return false;
            }
            foreach (var del in Delta)
            {
                var newPosition = SumPoint(pos, del);
                if (!alreadyChecked.Contains(newPosition))
                {
                    if (this[newPosition] == State.ShootedShip)
                        return IsShipDead(newPosition, alreadyChecked);
                }
            }
            
            return true;
        }

        public State this[Point pos]
        {
            get { return Location[pos.X, pos.Y]; }
             set { this.Location[pos.X, pos.Y] = value; }
        }
        private State[,] CreateMap(string map, string separator = "\r\n")
        {
            var rows = map.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Select(z => z.Length).Distinct().Count() != 1)
                throw new Exception($"Wrong test map '{map}'");
            if (rows.Length != ModelGame.Size || rows[0].Length != ModelGame.Size)
                throw new Exception($"Readed inncorect size map!'{map}'");
            var mapResult = new State[rows.Length, rows[0].Length];
            for (var x = 0; x < rows[0].Length; x++)
                for (var y = 0; y < rows.Length; y++)
                {
                    mapResult[x, y] = CreateCreatureBySymbol(rows[y][x]);
                    if (mapResult[x, y] == State.Ship)
                        AmountOfAliveShips++;
                }
            return mapResult;
        }

        private static State CreateCreatureBySymbol(char c)
        {
            switch (c)
            {
                case '*':
                    return State.Ship;
                case ' ':
                    return State.Empty;
                default:
                    throw new Exception($"wrong character for ICreature {c}");
            }
        }


        static public Point SumPoint(Point point1, Point point2)
        {
            var result = new Point(point1.X + point2.X, point1.Y + point2.Y);
            if (result.X < 0)
                result.X = 0;
            if (result.Y < 0)
                result.Y = 0;
            if (result.X >= ModelGame.Size)
                result.X = ModelGame.Size - 1;
            if (result.Y >= ModelGame.Size)
                result.Y = ModelGame.Size - 1;
            return result;
        }
    }
}
