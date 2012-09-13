using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Project_Entertainment_Game
{
    class Videos
    {
        public enum Movies { Intro }

        private Video vid;
        private VideoPlayer vidPlayer = new VideoPlayer();

        public bool isPlaying = false;
        public bool isDonePlaying = false;

        public Videos(Video video)
        {
            this.vid = video;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isPlaying == false)
            {
                vidPlayer.Play(vid);
                isPlaying = true;
            }
            else if (isPlaying == true)
            {
                spriteBatch.Draw(vidPlayer.GetTexture(), new Rectangle(0, 0, 1280, 720), Color.White);

                if (Keyboard.GetState().IsKeyDown(Keys.Space) || vidPlayer.State == MediaState.Stopped || Mouse.GetState().LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    if (vidPlayer.State != MediaState.Stopped) 
                        vidPlayer.Stop();
                    isPlaying = false;
                    isDonePlaying = true;
                }
            }
        }
    }
}
