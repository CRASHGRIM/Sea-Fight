using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ShipsWar
{
    public class PlayerAI
    {
        public Player Opposite;
        public Map Map;
        private bool IsWounded = false;
        private Point OldTarget = Point.Empty;
        public event Action<Point, State> SetPosition;
        static private MyRandomPoint RandomPoint;
        public PlayerAI(string stringMap)
        {
            RandomPoint = new MyRandomPoint(ModelGame.Size);
            Map = new Map(stringMap);
            SetPosition += Map.SetPosition;
        }

        public void ShipDead(Point pos)
        {
            SetPosition(pos, State.KilledShip);
            var newDelta = Map.Delta.Concat(Map.DeltaD);
            foreach (var del in newDelta)
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
            var target = RandomPoint.Next();
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


        public bool Shooting(Point position)
        {
            switch (Map[position])
            {
                case State.Empty:
                    {
                        SetPosition(position, State.Shooting);
                        Shoot();
                        return false;
                    }
                case State.Ship:
                    {
                        SetPosition(position, State.ShootedShip);
                        Map.CountDeadShips++;
                        if (Map.IsShipDead(position, new HashSet<Point>()))
                            ShipDead(position);
                        return true;
                    }
            }
            return false;
        }
    }
}
