using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Entertainment_Game
{
    public class Object
    {
        //Variables
        public Rectangle position;
        public Rectangle startposition;
        private Texture2D objectTex;

        private int moveX = 0;
        private int moveY = 0;
        public int movespeed = 0;

        public bool movementup = false;
        public bool movementdown = false;
        public bool movementleft = false;
        public bool movementright = false;

        //Constructor non-moveable object
        public Object(string texture, int x, int y, int width, int height)
        {
            position = new Rectangle(x, y, width, height);
            objectTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Maps/" + texture);
        }

        //Constructor movable object
        public Object(string texture, int x, int y, int width, int height, int moveX, int moveY, int movespeed)
        {
            this.movespeed = movespeed;

            if (moveX > 0)
            {
                movementright = true;
            }
            if (moveY > 0)
            {
                movementdown = true;
            }
            if (moveX < 0)
            {
                movementleft = true;
                moveX *= -1;
            }
            if (moveY < 0)
            {
                movementup = true;
                moveY *= -1;
            }
            this.moveX = moveX;
            this.moveY = moveY;

            startposition = new Rectangle(x, y, width, height);
            position = new Rectangle(x, y, width, height);
            objectTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Maps/" + texture);
        }

        /*
         * This function checks for movement of the object.
         */
        public void Update()
        {
            if (movementup)
            {
                position.Y -= movespeed;
                if (position.Y <= startposition.Y - moveY)
                {
                    movementup = false;
                    movementdown = true;
                }
            }
            else if (movementdown)
            {
                position.Y += movespeed;
                if (position.Y >= startposition.Y + moveY)
                {
                    movementup = true;
                    movementdown = false;
                }
            }

            if (movementleft)
            {
                position.X -= movespeed;
                if (position.X <= startposition.X - moveX)
                {
                    movementleft = false;
                    movementright = true;
                }
            }
            else if (movementright)
            {
                position.X += movespeed;
                if (position.X >= startposition.X + moveX)
                {
                    movementleft = true;
                    movementright = false;
                }
            }
        }

        /*
         * This function draws the object on the correct place.
         */ 
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (objectTex != null)
                spriteBatch.Draw(objectTex, position, Color.White);

            // The following lines can be used to visualize the bounding box.
            //Texture2D solidColor     = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/pixelWhite");
            //Rectangle boundingHitBox = position;
            //spriteBatch.Draw(solidColor, new Vector2(boundingHitBox.X, boundingHitBox.Y), boundingHitBox, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        }
    }
}
