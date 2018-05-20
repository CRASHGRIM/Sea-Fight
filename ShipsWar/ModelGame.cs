using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ShipsWar;

namespace ShipsWar
{

    public class ModelGame
    {
        public Player FirstPlayer;
        public PlayerAI SecondPlayer;
        public static int Size = 10;
        public ModelGame(string map2)
        {
            FirstPlayer = new Player(Size);
            SecondPlayer = new PlayerAI(map2);
            FirstPlayer.Opposite = SecondPlayer;
            SecondPlayer.Opposite = FirstPlayer;
        }     
    }
}
