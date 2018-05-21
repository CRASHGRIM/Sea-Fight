using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ShipsWar
{

    public class Player
    {
        public Map Map;
        public  PlayerAI Opposite;
        public virtual event Action<Point, State> SetPosition;
        public Player(int size)
        {
            Map = new Map(size);
            SetPosition += Map.SetPos;
        }
        public bool Shoot(Point pos)
        {
            return Opposite.Shooting(pos);
        }
        public bool Shooting(Point pos)
        {
            switch (Map[pos]) {
                case State.Empty:
                    {
                        SetPosition(pos, State.Shooting);
                        //Map[pos] = State.shooting;
                        return false;
                    }
                case State.Ship:
                    {
                        Opposite.Map.CountDeadShips++;
                        SetPosition(pos, State.ShootedShip);
                        //Map[pos] = State.shootingShip;
                        Map.CountDeadShips++;
                        if (Map.IsShipDead(pos, new HashSet<Point>()))
                            ShipDead(pos);
                        return true;
                    }

            }
            return false;   
        }

        public void ShipDead(Point pos)
        {
            SetPosition(pos, State.KilledShip);
            var newdelta = Map.Delta.Concat(Map.DeltaD);
            foreach (var del in newdelta)
            {
                var newpos = Map.SumPoint(pos, del);
                if (Map[newpos] == State.ShootedShip)
                    ShipDead(newpos);
                else
                    if (Map[newpos] == State.Empty)
                        SetPosition(newpos, State.Shooting);
            }

        }
        public State[,] PrepareMap(State[,] inputMap)
        {
            var map = new State[inputMap.GetLength(0), inputMap.GetLength(1)];
            for (int i = 0; i < inputMap.GetLength(0); i++)
                for (int j = 0; j < inputMap.GetLength(1); j++)
                {
                    if (inputMap[i, j] != State.NearShip)
                        map[i, j] = inputMap[i, j];
                    else
                        map[i, j] = State.Empty;
                }
            Map.AmountOfAliveShips = Map.CountAliveShips();
            return map;
        }
    }

    public class PlayerAI : Player
    {
        public Player Opposite;
        private bool IsWounded = false;
        private Point OldTarget = Point.Empty;
        public event Action<Point, State> SetPosition;
        static private MyRandomPoint RandomPoint;
        public PlayerAI(string stringMap) : base(stringMap.Length)
        {
            RandomPoint = new MyRandomPoint(ModelGame.Size);
            Map = new Map(stringMap);
            SetPosition += Map.SetPos;
        }

        public void ShipDead(Point pos)
        {
            SetPosition(pos, State.KilledShip);
            var newdelta = Map.Delta.Concat(Map.DeltaD);
            foreach (var del in newdelta)
            {
                var newPosition = Map.SumPoint(pos, del);
                if (Map[newPosition] == State.ShootedShip)
                    ShipDead(newPosition);
                else
                    if (Map[newPosition] == State.Empty)
                        SetPosition(newPosition, State.Shooting);
            }

        }
        public bool Shoot()
        {
            var target = new Point();
            while (Opposite.Map[target] != State.Empty)
            {
                if (IsWounded)
                    target = NewTarget(OldTarget);
                else
                    target = RandomPoint.Next();
                if (Opposite.Map[target] == State.ShootedShip)
                    OldTarget = target;
                while (Opposite.Map[target] == State.Ship)
                {
                    OldTarget = target;
                    Opposite.Shooting(target);
                    if (Opposite.Map[target] == State.KilledShip)
                        IsWounded = false;
                    else
                        IsWounded = true;
                    target = NewTarget(target); 
                }
            }
            return Opposite.Shooting(target); 
        }

        private Func<Point, Point> NewTarget = (pos) => 
            { var x = new Random(); return Map.SumPoint(pos, (Map.Delta[x.Next(Map.Delta.Length)])); };


        public  bool  Shooting(Point pos) 
        {
            switch (Map[pos])
            {
                case State.Empty:
                    {
                        SetPosition(pos, State.Shooting);
                        Shoot();
                        return false;
                    }
                case State.Ship:
                    {
                        SetPosition(pos, State.ShootedShip);
                        Map.CountDeadShips++;
                        if (Map.IsShipDead(pos, new HashSet<Point>()))
                            ShipDead(pos);
                        return true;
                    }
            }
            return false;
        }

        public bool IsLost()
        {
            return Map.AmountOfAliveShips == 0;
            //if (Map.AmountOfAliveShips == 0)
            //    return true;
            //else
            //    return false;
        }
    }
}
