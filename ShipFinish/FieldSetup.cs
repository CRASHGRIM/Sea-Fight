using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipsWar
{
    public class FieldSetup
    {
        public ModelGame GameModel;
        public Dictionary<int, bool> ActiveShip = new Dictionary<int, bool>();
        public Dictionary<int, int> AmountOfShipsStock = new Dictionary<int, int>();
        public Dictionary<int, int> AmountOfShips = new Dictionary<int, int>();
        public int Angle = 0;
        public FieldSetup(ModelGame game)
        {
            GameModel = game;
            AmountOfShipsStock[1] = 4;
            AmountOfShipsStock[2] = 3;
            AmountOfShipsStock[3] = 2;
            AmountOfShipsStock[4] = 1;
            AmountOfShips[1] = AmountOfShipsStock[1];
            AmountOfShips[2] = AmountOfShipsStock[2];
            AmountOfShips[3] = AmountOfShipsStock[3];
            AmountOfShips[4] = AmountOfShipsStock[4];
            for (int i = 1; i < 5; i++)
                ActiveShip[i] = false;
        }

        public void Reset()
        {
            for (int i = 0; i < ModelGame.Size; i++)
                for (int j = 0; j < ModelGame.Size; j++)
                    GameModel.FirstPlayer.Map.Location[i, j] = State.Empty;
            AmountOfShips[1] = AmountOfShipsStock[1];
            AmountOfShips[2] = AmountOfShipsStock[2];
            AmountOfShips[3] = AmountOfShipsStock[3];
            AmountOfShips[4] = AmountOfShipsStock[4];
            for (int i = 1; i < 5; i++)
                ActiveShip[i] = false;
        }

        public void ChooseShip(int lengthOfShip)
        {
            for (int i = 1; i < 5; i++)
                ActiveShip[i] = false;
            ActiveShip[lengthOfShip] = true;
        }

        public void PutShip(Point location)
        {
            int size = 0;
            for (int i = 1; i < 5; i++)
                if (ActiveShip[i])
                    size = i;
            if (size == 0)
                return;
            if (AmountOfShips[size] == 0)
                return;
            var ship = new Ship(location, Angle, size);
            foreach (var cell in ship.Cells)
            {
                if (cell.X < 0 || cell.Y < 0 || cell.X >= ModelGame.Size || cell.Y >= ModelGame.Size)
                    return;
                if (GameModel.FirstPlayer.Map.Location[cell.X, cell.Y] == State.Ship || GameModel.FirstPlayer.Map.Location[cell.X, cell.Y] == State.NearShip)
                    return;
            }
            AmountOfShips[size]--;
            foreach (var cell in ship.Cells)
                GameModel.FirstPlayer.Map.Location[cell.X, cell.Y] = State.Ship;
            foreach (var cell in ship.Cells)
            {
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                    {
                        if (cell.X + i >= 0 &&
                            cell.Y + j >= 0 &&
                            cell.X + i < ModelGame.Size &&
                            cell.Y + j < ModelGame.Size &&
                            GameModel.FirstPlayer.Map.Location[cell.X + i, cell.Y + j] != State.Ship)
                            GameModel.FirstPlayer.Map.Location[cell.X + i, cell.Y + j] = State.NearShip;
                    }
            }
        }
    }
}
