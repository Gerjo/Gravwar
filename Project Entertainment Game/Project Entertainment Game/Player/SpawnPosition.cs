using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Project_Entertainment_Game
{
    public class SpawnPosition
    {
        //Variables
        public int x;
        public int y;
        public string gravity;

        //Constructor
        public SpawnPosition(int x, int y, string gravity)
        {
            this.x = x;
            this.y = y;
            this.gravity = gravity;
        }

        // Return as vector, used for AnimPlayer.
        public Vector2 GetSpawnVector() 
        {
            return new Vector2(x, y);
        }

        // Return as Enum, used for AnimPlayer
        public AnimPlayer.GravitySide GetSpawnGravitySide()
        {
            // Apparently the XBOX doesn't support any of the Enum methods (TryParse, GetValues & Parse), hence
            // were using a rather silly switch case.
            switch (gravity.ToUpper())
            {
                case "UP":      return AnimPlayer.GravitySide.Up;
                case "DOWN":    return AnimPlayer.GravitySide.Down;
                case "LEFT":    return AnimPlayer.GravitySide.Left;
                case "RIGHT":   return AnimPlayer.GravitySide.Right;
                default: return AnimPlayer.GravitySide.Down; 
            }
        }
    }
}
