using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Entertainment_Game
{
    public class GameOver
    {
        //Variables
        SpriteFont screenFont;
        String screenText = "";
        Level level;

        public bool gobacktomenu = false;

        //Constructor
        public GameOver(Level level)
        {
            this.level = level;
            screenFont = Game1.INSTANCE.Content.Load<SpriteFont>("Debug");
        }

        /*
         * Sets the game over data at the end of the game.
         */
        public void SetText()
        {            
            for (int i = 0; i < level.animPlayers.Length; i++)
            {
                if (level.animPlayers[i] != null)
                {
                    float kdratio;
                    if (level.animPlayers[i].playerKills != 0 && level.animPlayers[i].playerDied != 0)
                    {
                        kdratio = level.animPlayers[i].playerKills / level.animPlayers[i].playerDied;
                    }
                    else
                    {
                        kdratio = 0;
                    }
                    screenText += "Player " + level.animPlayers[i].playerIndex + ": " + level.animPlayers[i].playerKills + " " + level.animPlayers[i].playerDied + " " + kdratio + "\n";
                }
            }
        }

        /*
         * Draws the game over data on the screen.
         */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                gobacktomenu = true;
            }

            spriteBatch.DrawString(screenFont, screenText, Vector2.Zero, Color.White);
        }
    }
}
