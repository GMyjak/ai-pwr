using System;
using System.Collections.Generic;
using System.Text;

namespace Mancala
{
    public enum Player
    {
        A, B
    }

    public abstract class PlayerUtils
    {
        public static Player Other(Player p)
        {
            if (p == Player.A)
            {
                return Player.B;
            }

            return Player.A;
        }
    }
}
