using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Entertainment_Game
{
    public class endGameMenu
    {
        private Level ownerLevel;
        public  Boolean gobacktomenu  = false;
        public  Boolean restartgame   = false;
        private  float delayCounter   = 0;
        private float forceDelayLimit = 2000; // The menu must be viewed at least N milliseconds.

        
        private SpriteFont mainFont;
        private Texture2D wallpaper;
        private Texture2D avatar;
        private Texture2D[] scoreCardBackground = new Texture2D[4];

        private List<PlayerScoreCard> scorecards = new List<PlayerScoreCard>();

        private Vector2 scorecardOffset = new Vector2(285, 233);

        public endGameMenu(Level ownerLevel)
        {
            // Lateron we can load the player's kills/deaths and wins via this reference
            this.ownerLevel = ownerLevel;

            // Load the required menu resources:
            mainFont  = Game1.INSTANCE.Content.Load<SpriteFont>("menuSpriteFont");
            wallpaper = Game1.INSTANCE.Content.Load<Texture2D>("Images/EndGame/background");
            avatar    = Game1.INSTANCE.Content.Load<Texture2D>("Images/EndGame/avatar");

            for (int i = 1; i <= 4; ++i)
            {
                scoreCardBackground[i-1] = Game1.INSTANCE.Content.Load<Texture2D>("Images/EndGame/scorecard" + i);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Meh... no Update function...
            if (ownerLevel.animPlayers[0].inputDevice.isBbutton() && delayCounter > forceDelayLimit) gobacktomenu = true;
            if (ownerLevel.animPlayers[0].inputDevice.isAbutton() && delayCounter > forceDelayLimit) restartgame = true;

            delayCounter += gameTime.ElapsedGameTime.Milliseconds;

            spriteBatch.Draw(wallpaper, new Vector2(0, 0), Color.White);

            int yOffset = 95;

            for (int i = 0; i < scorecards.Count; ++i)
            {
                // Background:
                spriteBatch.Draw(scoreCardBackground[scorecards[i].position], new Vector2(scorecardOffset.X, scorecardOffset.Y + (i * yOffset)), Color.White);
                // Side avatar:
                spriteBatch.Draw(avatar, new Vector2(scorecardOffset.X + 5, scorecardOffset.Y + (i * yOffset)), scorecards[i].playerColor);
 
                // Player #N ( already embedded on the image)
                //spriteBatch.DrawString(mainFont, "Player #" + (i + 1), new Vector2(scorecardOffset.X + 115, scorecardOffset.Y + (i * yOffset)), Color.White);
                // Kills:
                spriteBatch.DrawString(mainFont, scorecards[i].kills + fixPlural(" kill", scorecards[i].kills), new Vector2(scorecardOffset.X + 115, scorecardOffset.Y + (i * yOffset) + 30), new Color(39, 39, 39), 0, new Vector2(0, 0), 1.2f, SpriteEffects.None, 1);               
                // Deaths:
                spriteBatch.DrawString(mainFont, scorecards[i].deaths + fixPlural(" death", scorecards[i].deaths), new Vector2(scorecardOffset.X + 320, scorecardOffset.Y + (i * yOffset) + 44), new Color(39, 39, 39), 0, new Vector2(0, 0), .8f, SpriteEffects.None, 1);
            }

        }

        public void LoadNewScreen(Level ownerLevel)
        {
            Console.WriteLine("Loading new winscreen.");
            delayCounter = 0;
            this.ownerLevel = ownerLevel;
            this.scorecards   = new List<PlayerScoreCard>();
            Random rand  = new Random();
            for (int i = 0; i < ownerLevel.playerTotal; ++i)
            {
                AnimPlayer player = ownerLevel.animPlayers[i];

                scorecards.Add(new PlayerScoreCard(player.playerKills, player.playerDied, player.playerSuicide, player.intPlayerIndex, player.playerColor));
            }

            scorecards.Sort(new PlayerScoreCard());
        }

        private String fixPlural(String input, int amt)
        {
            return input + ((amt == 1) ? "":"s");
        }
    }


    struct PlayerScoreCard : IComparer<PlayerScoreCard>
    {
        public int kills;
        public int deaths;
        public int suicides;
        public int position;
        public Color playerColor;

       // public PlayerScoreCard() { }

        public PlayerScoreCard(int kills, int deaths, int suicides, int position, Color playerColor)
        {
            this.kills       = kills;
            this.deaths      = deaths;
            this.suicides    = suicides;
            this.position    = position;
            this.playerColor = playerColor;
        }

        // Kills descending, deaths ascending
        public int Compare(PlayerScoreCard a, PlayerScoreCard b)
        {
            int diff = -a.kills.CompareTo(b.kills);
            return (diff != 0) ? diff : a.deaths.CompareTo(b.deaths);
        }
    }
}
