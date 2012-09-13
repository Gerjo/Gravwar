using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Entertainment_Game
{
    public class Level
    {
        //Variables
        private Texture2D backgroundTex;
        private SpriteFont levelTimer;
        private SpriteFont countDownFont;
        public Audio audio;

        public List<SpawnPosition> spawnPositions = new List<SpawnPosition>();
        public List<Object> objects = new List<Object>();
        public List<Items> items = new List<Items>();
        public Boolean endGame = false;
        public AnimPlayer[] animPlayers = new AnimPlayer[4];

        private Rectangle[] crownpos = new Rectangle[4];
        private Texture2D[] crowntex = new Texture2D[4];
        private float[] crownrot = new float[4];

        // Some global game settings:
        public int playerTotal = 2;
        public int fragLimit = 50;
        public int timeLimit = 10 * 60;
        public int currentTime = 10 * 60;
        public float soundVolume = 1;
        public float musicVolume = 1;
        private int countdownTime = 4 * 60;
        private int countText = 0;
        public string levelName = "Maps/Annihilation.txt";

        public enum LevelStates { Play, Pause, WinScreen, StartCountdown }
        public LevelStates levelState = LevelStates.StartCountdown;

        struct Crown
        {
            public int index;
            public bool inUse;
        }

        // Textures:
        private Texture2D pauzeScreenOverlay = null;

        //Constructor
        public Level()
        {
        }

        /*
         * The LoadContent function of level is one of the most
         * important functions in the game. It loads the level,
         * players and objects. It also loads all the
         * spawn positions.
         */
        public void LoadContent(Menu gameMenu)
        {
            //Hier wordt de informatie vanuit het menu geladen in het level
            fragLimit = gameMenu.fragLimit;
            timeLimit = gameMenu.timeLimit * 60 * 60;

            //Audio initialization.
            audio = new Audio();
            musicVolume = gameMenu.musicVol;
            soundVolume = gameMenu.soundVol;

            playerTotal = gameMenu.numPlayers;

            if (timeLimit == 0)
            {
                timeLimit = int.MaxValue - countdownTime - countdownTime;
            }
            if (fragLimit == 0)
            {
                fragLimit = int.MaxValue;
            }
            if (gameMenu.levelName != null)
            {
                levelName = gameMenu.levelName;
            }
            currentTime = timeLimit + countdownTime;

            //Console.Write("Timelimit: " + timeLimit + " Fraglimit: " + fragLimit + "\n");
            //Load some resources:
            levelTimer = Game1.INSTANCE.Content.Load<SpriteFont>("LevelTimer");
            countDownFont = Game1.INSTANCE.Content.Load<SpriteFont>("Menu");
            pauzeScreenOverlay = Game1.INSTANCE.Content.Load<Texture2D>("Images/Miscellaneous/pauseScreen");
            crowntex[0] = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/Crown4");
            crowntex[1] = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/Crown3");
            crowntex[2] = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/Crown2");
            crowntex[3] = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/Crown1");

            

            //Create an filestream.
            FileStream fstream = new FileStream(levelName, FileMode.Open, FileAccess.Read);
            byte[] filecontent = new byte[fstream.Length];

            //Create an array to put our data in.
            string[][] leveldata = new string[filecontent.Length][];

            //Read the file and put data in the filecontent.
            for (int i = 0; i < fstream.Length; i++)
            {
                fstream.Read(filecontent, i, 1);
            }

            //Split up the file in "words".
            int currentAt = 0;
            int currentAt2 = 0;
            for (int i = 0; i < filecontent.Length; i++)
            {

                if (leveldata[currentAt] == null)
                {
                    leveldata[currentAt] = new string[255];
                }
                if ((char)filecontent[i] == ' ')
                {
                    currentAt2++;
                }
                else if ((char)filecontent[i] == (char)10)
                {
                    currentAt2 = 0;
                    currentAt++;
                }
                else
                {
                    leveldata[currentAt][currentAt2] += (char)filecontent[i];
                }
            }

            //Go through every data in leveldata.
            for (int i = 0; i < leveldata.Length; ++i)
            {
                if (leveldata[i] != null)
                {
                    if (leveldata[i][0].Contains("background:"))
                    {
                        backgroundTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Maps/" + leveldata[i][1]);
                    }
                    else if (leveldata[i][0].Contains("backgroundmusic:"))
                    {
                        audio.LoadMusic(leveldata[i][1]);
                    }
                    else if (leveldata[i][0].Contains("object:"))
                    {
                        objects.Add(new Object(leveldata[i][1], int.Parse(leveldata[i][2]), int.Parse(leveldata[i][3]), int.Parse(leveldata[i][4]), int.Parse(leveldata[i][5])));
                    }
                    else if (leveldata[i][0].Contains("moveableobj:"))
                    {
                        objects.Add(new Object(leveldata[i][1], int.Parse(leveldata[i][2]), int.Parse(leveldata[i][3]), int.Parse(leveldata[i][4]), int.Parse(leveldata[i][5]), int.Parse(leveldata[i][6]), int.Parse(leveldata[i][7]), int.Parse(leveldata[i][8])));
                    }
                    else if (leveldata[i][0].Contains("item:"))
                    {
                        items.Add(new Items(int.Parse(leveldata[i][2]), int.Parse(leveldata[i][3]), leveldata[i][1]));
                    }
                    else if (leveldata[i][0].Contains("\r"))
                    {
                    }
                    else if (leveldata[i][0].Contains("spawn:"))
                    {
                        spawnPositions.Add(new SpawnPosition(int.Parse(leveldata[i][1]), int.Parse(leveldata[i][2]), leveldata[i][3]));
                    }
                }
            }

            //What if no background is found?
            if (backgroundTex == null)
            {
                backgroundTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Maps/BasicShapes/Rectangle");
            }


            // Initiate 4 players:
            for (int i = 0; i < playerTotal; i++)
            {
                animPlayers[i] = new AnimPlayer(this, i);
            }

            // Start the game, we *could* add a start game countdown screen here.
            levelState = LevelStates.StartCountdown;
        }

        /*
         * The update function in level makes sure the audio is running
         * if there is any in the level. Also updates every player in the
         * game and decreases the currentTime. If the currentTime is reached
         * it also activates the endGame boolean to end the game. The
         * function also checks if a player reached the fraglimit and if
         * it is reached it will also end the game.
         */
        public void Update(GameTime gameTime)
        {
            if (!audio.musicPlaying && audio.song != null)
            {
                audio.PlayMusic();
                audio.musicPlaying = true;
            }

            if (levelState == LevelStates.StartCountdown)
            {

                if (currentTime == timeLimit + 4 * 60)
                {
                    audio.PlaySound("3", soundVolume);
                    countText = 3;
                }
                else if (currentTime == timeLimit + 3 * 60)
                {
                    audio.PlaySound("2", soundVolume);
                    countText = 2;
                }
                else if (currentTime == timeLimit + 2 * 60)
                {
                    audio.PlaySound("1", soundVolume);
                    countText = 1;
                }
                else if (currentTime == timeLimit + 1 * 60)
                {
                    audio.PlaySound("start", soundVolume);
                    countText = 0;
                }
                else if (currentTime == timeLimit)
                {
                    levelState = LevelStates.Play;
                }
                currentTime--;
            }

            // Toggle the Pauze game state. 
            foreach (AnimPlayer somePlayer in animPlayers)
            {
                if (somePlayer != null && somePlayer.inputDevice != null)
                {
                    //OPTIONAL: Here we could take notion of whomever pressed the pauzebutton...
                    if (somePlayer.inputDevice.isStartPause() && levelState != LevelStates.StartCountdown) levelState = LevelStates.Pause;
                    if (somePlayer.inputDevice.isStopPause() && levelState != LevelStates.StartCountdown) levelState = LevelStates.Play;
                    if (somePlayer.inputDevice.isGobackToMenu() && levelState == LevelStates.Pause && levelState != LevelStates.StartCountdown) endGame = true;
                }
            }

            // Halt execution if the level state is not "play".
            if (levelState != LevelStates.Play)
                return;

            // Update any items in the game. These are mainly weapon pickups:
            foreach (Items item in items) { if (item != null) { item.Update(gameTime); } }
            foreach (Object obj in objects) { if (obj != null) { obj.Update(); } }

            foreach (AnimPlayer somePlayer in animPlayers)
            {
                // Dispatch the update event to all players:
                if (somePlayer != null)
                {
                    somePlayer.Update(gameTime);


                    // Check whether the player has managed to get himself killed:
                    somePlayer.checkOutOfLevel();

                    // Hit-test this player against all game objects:
                    foreach (Object obj in objects) { if (obj != null) somePlayer.checkObjectCollision(obj); }
                    foreach (Items item in items) { if (item != null) somePlayer.checkItemCollision(item); }

                    // Hit-test this player's bullets against other players and objects:
                    foreach (Weapons.AbstractBullet bullet in somePlayer.weaponManager.allBullets)
                    {
                        if (bullet.isDestroyed) continue; // This bullet has already "hit" something.

                        // Bullet VS all players:
                        foreach (AnimPlayer testingPlayer in animPlayers)
                        {
                            if (testingPlayer != null)
                            {
                                if (testingPlayer == somePlayer) continue; // Suicide not allowed.
                                // Hit-test bullet against player:
                                testingPlayer.checkBulletCollision(bullet);
                            }
                        }

                        // Bullet VS all objects: 
                        foreach (Object obj in objects)
                        {
                            bullet.CheckObjectCollision(obj);
                        }

                    }

                    // Check if the kill (frag) limit has been reached:
                    if (somePlayer.playerKills >= fragLimit)
                    {
                        endGame = true;
                    }
                }
            }

            // Handle the time limit and game clock:
            if (currentTime == 660)
                audio.PlaySound("10 seconds left", soundVolume);
            else if (currentTime == 270)
                audio.PlaySound("3", soundVolume);
            else if (currentTime == 180)
                audio.PlaySound("2", soundVolume);
            else if (currentTime == 120)
                audio.PlaySound("1", soundVolume);
            else if (currentTime == 60)
                audio.PlaySound("endgame", soundVolume);
            if (currentTime <= 0) endGame = true;
            if (currentTime > 0) currentTime--;
        }

        /*
         * This draw function draws every object and player in the game. It also draws
         * the level background and the countdown timer.
         */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 worldOffset)
        {
            // Draw the leve wallpaper.
            spriteBatch.Draw(backgroundTex, new Rectangle(0, 0, 1280, 720), Color.White);

            // Dispatch the draw event to all game objects:
            foreach (Object obj in objects) obj.Draw(gameTime, spriteBatch);
            foreach (Items item in items) item.Draw(gameTime, spriteBatch);
            foreach (AnimPlayer somePlayer in animPlayers)
            {
                if (somePlayer != null)
                {
                    somePlayer.Draw(gameTime, spriteBatch);
                }
            }

            Crown[] crownArr = new Crown[4];
            int temp;
            bool isSorted = false;

            for (int i = 0; i < playerTotal; i++)
            {
                crownArr[i].index = animPlayers[i].playerKills;
                crownArr[i].inUse = false;
            }

            while (!isSorted)
            {
                isSorted = true;
                for (int i = 0; i < crownArr.Length - 1; i++)
                {
                    if (crownArr[i].index > crownArr[i + 1].index)
                    {
                        temp = crownArr[i + 1].index;
                        crownArr[i + 1].index = crownArr[i].index;
                        crownArr[i].index = temp;
                        isSorted = false;
                    }
                }
            }

            for (int i = 0; i < playerTotal; i++)
            {
                if (crownArr[3].index == animPlayers[i].playerKills && crownArr[3].inUse == false)
                {
                    crownpos[3] = getCrownPos(animPlayers[i]);
                    crownrot[3] = animPlayers[i].getBaseRotation();
                    crownArr[3].inUse = true;
                }
                else if (crownArr[2].index == animPlayers[i].playerKills && crownArr[2].inUse == false)
                {
                    crownpos[2] = getCrownPos(animPlayers[i]);
                    crownrot[2] = animPlayers[i].getBaseRotation();
                    crownArr[2].inUse = true;
                }
                else if (crownArr[1].index == animPlayers[i].playerKills && crownArr[1].inUse == false)
                {
                    crownpos[1] = getCrownPos(animPlayers[i]);
                    crownrot[1] = animPlayers[i].getBaseRotation();
                    crownArr[1].inUse = true;
                }
                else if (crownArr[0].index == animPlayers[i].playerKills && crownArr[0].inUse == false)
                {
                    crownpos[0] = getCrownPos(animPlayers[i]);
                    crownrot[0] = animPlayers[i].getBaseRotation();
                    crownArr[0].inUse = true;
                }
            }

            for (int i = 0; i < crowntex.Length; i++)
                //spriteBatch.DrawString(countDownFont, i.ToString(), new Vector2(crownpos[i].X, crownpos[i].Y), Color.White);
                spriteBatch.Draw(crowntex[i], crownpos[i], null, Color.White, crownrot[i], new Vector2(crowntex[i].Width/2, crowntex[i].Height/2), SpriteEffects.None, 0);

            // Alternatively we could draw a in-game menu here.


            // Draw the game countdown clock:
            if(currentTime/60 < 9999)
                if (currentTime < timeLimit)
                {
                    // Show an actual countdown:
                    spriteBatch.DrawString(levelTimer, "" + (int)(currentTime / 60), new Vector2(1170, 0), Color.White);
                }
                else
                {
                    // The game is still in its countdown "Grace" zone, hence the time limit is shown rather than the actual count.
                    spriteBatch.DrawString(levelTimer, "" + (int)(timeLimit / 60), new Vector2(1170, 0), Color.White);
                }

            // The game has been paused, thus draw some fancy pausescreen overlay.
            if (levelState == LevelStates.Pause)
            {
                spriteBatch.Draw(pauzeScreenOverlay, new Rectangle(0, 0, 1280, 720), Color.White);
            }
            else if (levelState == LevelStates.StartCountdown)
            {
                string countDownString = ".. " + countText + " ..";
                if (countText == 0)
                    countDownString = ".. FIGHT ..";
                int countDownFontSize = (int)countDownFont.MeasureString(countDownString).X;
                spriteBatch.DrawString(countDownFont, countDownString, new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2 - countDownFontSize / 2, 300), Color.White);
            }
        }

        private Rectangle getCrownPos(AnimPlayer p)
        {
            Rectangle rect = new Rectangle((int)p.position.X, (int)p.position.Y, 25, 25);
            switch (p.gravitySide)
            {
                case AnimPlayer.GravitySide.Up:
                    rect.Y += p.currentHitbox.Height / 2 + 30;
                    break;
                case AnimPlayer.GravitySide.Down:
                    rect.Y -= p.currentHitbox.Height / 2 + 30;
                    break;
                case AnimPlayer.GravitySide.Left:
                    rect.X += p.currentHitbox.Width / 2 + 30;
                    break;
                case AnimPlayer.GravitySide.Right:
                    rect.X -= p.currentHitbox.Width / 2 + 30;
                    break;
            }
            if (p.playerHealth > 0)
                return rect;
            else
                return new Rectangle();
        }
    }
}
