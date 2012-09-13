using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project_Entertainment_Game
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;
        Menu gameMenu;
        Videos introVideoPlayer;
        public Level level;
        //public GameOver gameOver;
        public endGameMenu endGameMenu;

        //Keyboard states
        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;
        
        //Random generator
        public Random random = new Random();

        //Instance
        public static Game1 INSTANCE;

        //Different kinds of gamestates
        private enum GameState { IntroMovie, Menu, InGame, EndGame, LevelEditor }

        private GameState gameState = GameState.IntroMovie;

        //Constructor
        public Game1()
        {
            INSTANCE = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets the res to 1280x720 on fullscreen
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /*
         * This function loads all the content that is needed in the game
         * Everything is loaded at the start so the game can run without
         * loading times. And we all hate loading times :D
         */
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            introVideoPlayer = new Videos(Content.Load<Video>("TeamKaboomGravWarTitles"));
            gameMenu = new Menu();
            //gameMenu.startGame = true; // NOTICE: uncomment to directly start in the first level!
            level = new Level();
            //gameOver = new GameOver(level);
            endGameMenu = new endGameMenu(level);

            debugFont = Content.Load<SpriteFont>("Debug");
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /*
         * In the Update function everything will be updated. It checks
         * if the gamestate has changed. And also makes sure the correct
         * things are being updated. Meaning the menu won't update when
         * the game is running for example. It also checks if the escape
         * button is pressed so the game can exit or when F8 is pressed
         * for entering debugMode.
         */
        protected override void Update(GameTime gameTime)
        {
            newKeyboardState = Keyboard.GetState();
            if (introVideoPlayer.isDonePlaying)
            {
                gameMenu.StartMusic();
                gameState = GameState.Menu;
                introVideoPlayer.isDonePlaying = false;
            }

            if (endGameMenu.gobacktomenu == true)
            {
                //gameOver.gobacktomenu = false;
                endGameMenu.gobacktomenu = false;
                level = new Level();
                gameState = GameState.Menu;
            }
            else if (endGameMenu.restartgame == true)
            {
                endGameMenu.restartgame = false;
                level.objects = new List<Object>();
                gameMenu.startGame = true;
            }

            if (gameMenu.startGame == true)
            {
                gameState = GameState.InGame;
                level.LoadContent(gameMenu);
                gameMenu.startGame = false;
            }

            if (level.endGame == true)
            {
                level.audio.StopMusic();
                gameState = GameState.EndGame;
                //gameOver.SetText();
                endGameMenu.LoadNewScreen(level);
                level.endGame = false;
            }

            if (newKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.InGame)
            {
                level.Update(gameTime);
            }

            oldKeyboardState = newKeyboardState;
            base.Update(gameTime);
        }

        /*
         * In the Draw function everything will be drawn. There is
         * also made use of the gameState again. This way nothing gets
         * drawn that is not needed. Also debug data will be drawn
         * when debugMode=true.
         */
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (gameState == GameState.IntroMovie)
            {
                introVideoPlayer.Draw(spriteBatch);
            }
            if (gameState == GameState.Menu)
            {
                gameMenu.Draw(gameTime, spriteBatch);
            }
            if (gameState == GameState.InGame)
            {
                level.Draw(gameTime, spriteBatch, Vector2.Zero);
            }

            // TODO: remove the "true ||" bit
            if (gameState == GameState.EndGame)
            {
                endGameMenu.Draw(gameTime, spriteBatch);
                //gameOver.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
