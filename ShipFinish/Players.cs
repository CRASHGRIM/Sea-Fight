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
            SetPosition += Map.SetPosition;
        }
        public bool Shoot(Point pos)
        {
            return Opposite.Shooting(pos);
        }
        public bool Shooting(Point position)
        {
            switch (Map[position]) {
                case State.Empty:
                    {
                        SetPosition(position, State.Shooting);
                        return false;
                    }
                case State.Ship:
                    {
                        Opposite.Map.CountDeadShips++;
                        SetPosition(position, State.ShootedShip);
                        Map.CountDeadShips++;
                        if (Map.IsShipDead(position, new HashSet<Point>()))
                            ShipDead(position);
                        return true;
                    }

            }
            return false;   
        }

        public void ShipDead(Point position)
        {
            SetPosition(position, State.KilledShip);
            var newDelta = Map.Delta.Concat(Map.DeltaD);
            foreach (var del in newDelta)
            {
                var newPosition = Map.SumPoint(position, del);
                if (Map[newPosition] == State.ShootedShip)
                    ShipDead(newPosition);
                else
                    if (Map[newPosition] == State.Empty)
                        SetPosition(newPosition, State.Shooting);
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


}
