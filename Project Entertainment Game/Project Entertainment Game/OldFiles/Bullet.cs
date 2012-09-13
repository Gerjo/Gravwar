using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project_Entertainment_Game
{
    public class Bullet
    {
        //Variables
        public Vector2 position;
        public Rectangle hitbox;
        public int timeInGame = 0;
        public bool destroyed = false;
        public string weapon;
        public int damage;
        public Player player;
        public Vector2 direction;
        private Texture2D bulletTex;
        public int speed;
        private float rotation = 0;

        //Constructor
        public Bullet(Player player, Vector2 position, Vector2 direction, int speed, int damage, string weapon)
        {
            this.player = player;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.weapon = weapon;

            if (weapon == "Pistol")
            {
                bulletTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Bullet");
            }
            else if (weapon == "Machinegun")
            {
                bulletTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Bullet");
            }
            else if (weapon == "Rocketlauncher")
            {
                bulletTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Rocket");
            }
            else if (weapon == "Grenadelauncher")
            {
                bulletTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Grenade");
            }
            else
            {
                bulletTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Bullet");
            }
        }

        /*
         * This update method checks what direction the bullet should go and also
         * creates a hitbox around the bullet.
         */
        public void Update(GameTime gameTime)
        {
            if (weapon == "Grenadelauncher")
            {
                //speed = (int)(speed * 0.99);
                float gravity = player.gravity * 0.1f;
                direction.Y += gravity;
                /*if (player.gravitySide == Player.GravitySide.Up)
                    direction.Y -= gravity;
                else if (player.gravitySide == Player.GravitySide.Down)
                    direction.Y += gravity;
                else if (player.gravitySide == Player.GravitySide.Left)
                    direction.X -= gravity;
                else if (player.gravitySide == Player.GravitySide.Right)
                    direction.X += gravity;*/
            }

            position = Vector2.Add(position, Vector2.Multiply(direction, speed));
            Vector2 normalizedDirection = new Vector2(direction.X, direction.Y);
            normalizedDirection.Normalize();
            rotation = MathHelper.ToRadians(90) - (float)(Math.Atan2((double)normalizedDirection.Y, (double)normalizedDirection.X)) * -1;
            float ndx = normalizedDirection.X;
            float ndy = normalizedDirection.Y;
            if (ndx < 0)
            {
                ndx *= -1;
            }
            if (ndy < 0)
            {
                ndy *= -1;
            }
            hitbox = new Rectangle((int)position.X - ((bulletTex.Width + (int)(bulletTex.Height * ndx)) / 2),
                                    (int)position.Y - ((bulletTex.Width + (int)(bulletTex.Height * ndy)) / 2),
                                    (bulletTex.Width + (int)(bulletTex.Height * ndx)),
                                    (bulletTex.Width + (int)(bulletTex.Height * ndy)));
            timeInGame++;
        }

        //This draw method draws the bullet on the correct position.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTex, new Rectangle((int)position.X, (int)position.Y, bulletTex.Width, bulletTex.Height), null, Color.White, rotation, new Vector2(bulletTex.Width/2, bulletTex.Height/2), SpriteEffects.None, 0);
        }
    }
}
