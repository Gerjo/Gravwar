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

namespace Project_Entertainment_Game.Weapons
{
    public class BouncyGun : AbstractWeapon
    {
        public BouncyGun() : base()
        {
            // Load the textures:
            normalTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armGrenade");
            fireTexture   = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armflashGrenade");

            fireRateDelay     = 1200;
            defaultAmmunition = 6; 

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--; // Remove a single bullet.
            return new BouncyGunBullet(ownerPlayer);
        }
    }

    public class BouncyGunBullet : AbstractBullet
    {
        private Vector2 gravity      = new Vector2(0, 0.02f);
        private Vector2 maxGravity   = new Vector2(0.1f, 0.1f);
        private float bounceFriction = .7f;
        private float friction       = 0.003f;

        public BouncyGunBullet(AnimPlayer ownerPlayer)
            : base(ownerPlayer)
        {
            // Load the textures:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/grenadeRotate");

            bulletSpeed   = 20;
            damageScore   = 80;
            isFlameable = true;
            size.X = 25;
            size.Y = 25;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

        }

        public override void Update(GameTime gameTime)
        {
            // The base must always be called first.
            base.Update(gameTime);

            if (totalTimeAlive >= 5500 && totalTimeAlive < 6000)
                isExploding = true;
            else if (totalTimeAlive >= 6000)
                isDestroyed = true;

            if (isExploding == true && soundExplosion == false)
            {
                soundExplosion = true;
                ownerPlayer.ownerLevel.audio.PlaySound("Grenade_Hit", ownerPlayer.ownerLevel.soundVolume);
                size *= 1.5f;
                damageScore /= 2;
            }

            if (normalizedDirection.X > 0) normalizedDirection.X      -= friction;
            else if (normalizedDirection.X < 0) normalizedDirection.X += friction;

            if (normalizedDirection.Y > 0) normalizedDirection.Y      -= friction;
            else if (normalizedDirection.Y < 0) normalizedDirection.Y += friction;

            if (Math.Abs(normalizedDirection.Y) < friction) normalizedDirection.Y = 0;
            if (Math.Abs(normalizedDirection.X) < friction) normalizedDirection.X = 0;

            Vector2.Clamp(normalizedDirection, maxGravity * -1, maxGravity);
            normalizedDirection += gravity;

            // Put your own "motion" code here, or use the basic one given by this class.
            basicBulletMotion();

            //Console.Write(normalizedDirection);
            
        }

        public override void CheckObjectCollision(Object obj)
        {
            Rectangle a        = getHitBox();
            Rectangle b        = obj.position;
            Rectangle c;

            if (a.Intersects(b))
            {
                
                Rectangle.Intersect(ref a, ref b, out c);
                
                if (c.Height <= c.Width)
                {
                    if (a.Top < b.Bottom && a.Top > b.Bottom - 10)
                    {
                        position.Y = (b.Bottom + a.Height / 2);
                        //Console.WriteLine("TOP! ^");
                    }
                    else
                    {
                        position.Y = (b.Top - a.Height / 2) + 1;
                        //Console.WriteLine("BOTTOM! v ");
                    }
                    normalizedDirection.Y *= -bounceFriction;
                }
                else
                {
                    if (a.Left < b.Right && a.Left > b.Right - 20)
                    {
                        position.X = b.Right + a.Width/2;
                        //Console.WriteLine("LEFT! <");
                    }
                    else {
                        position.X = b.Left - a.Width;
                        //Console.WriteLine("RIGHT! >");
                    }
                    normalizedDirection.X *= -bounceFriction;
                }
                
            }
        }


        public override Rectangle getHitBox()
        {
            return new Rectangle((int)(position.X - size.X / 2), (int)(position.Y - size.Y / 2), (int)size.X, (int)size.Y);
        }
    }
}
