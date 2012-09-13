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
    /*
     * 
     * THIS FILE IS NOT USED, EDIT "BouncyGun" instead. - Gerjo
     * 
     */
    public class GrenadeLauncher : AbstractWeapon
    {

        public GrenadeLauncher()
            : base()
        {
            // Load the textures:
            normalTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armGrenade");
            fireTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armflashGrenade");

            fireRateDelay = 1000;
            defaultAmmunition = 5;

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--; // Remove a single bullet.
            return new Grenade(ownerPlayer);
        }
    }

    public class Grenade : AbstractBullet
    {
        private Vector2 gravity = new Vector2(1, 0f);
        private Vector2 maxGravity = new Vector2(1, 10f);
        private float friction = 0.6f;

        public Grenade(AnimPlayer ownerPlayer)
            : base(ownerPlayer)
        {
            // Load the textures:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/grenade");
            bulletSpeed = 1;
            damageScore = 50;
        }

        public override void CheckObjectCollision(Object obj)
        {
            //Create the bounding boxes
            BoundingBox b = new BoundingBox(new Vector3(obj.position.X, obj.position.Y, 0), new Vector3(obj.position.X + obj.position.Width, obj.position.Y + obj.position.Height, 0));
            BoundingSphere s = new BoundingSphere(new Vector3(position.X, position.Y, 0), 20);

            if (s.Intersects(b))
            {
                float angle = (float)Math.Atan2((obj.position.Y + obj.position.Height / 2) - position.Y, (obj.position.X + obj.position.Width / 2) - position.X);

                float angleobjUpperLeft = (float)Math.Atan2(obj.position.Height / 2, -obj.position.Width / 2);
                float angleobjLowerLeft = (float)Math.Atan2(-obj.position.Height / 2, -obj.position.Width / 2);
                float angleobjUpperRight = (float)Math.Atan2(obj.position.Height / 2, obj.position.Width / 2);
                float angleobjLowerRight = (float)Math.Atan2(-obj.position.Height / 2, obj.position.Width / 2);

                //Determine if we should change the direction
                if (bulletSpeed > 0.2)
                {
                    //LeftSide
                    if (angle > angleobjUpperLeft && angle < Math.PI || angle < angleobjLowerLeft && angle > -Math.PI)
                    {
                        normalizedDirection.X *= -1;
                    }

                    //RightSide
                    if (angle < angleobjUpperRight && angle > 0 || angle > angleobjLowerRight && angle < 0)
                    {
                        normalizedDirection.X *= -1;
                    }

                    //TopSide
                    if (angle > angleobjUpperRight && angle < angleobjUpperLeft)
                    {
                        normalizedDirection.Y *= -1;
                        gravity.Y = -maxGravity.Y;
                    }

                    //DownSide
                    if (angle < angleobjLowerRight && angle > angleobjLowerLeft)
                    {
                        normalizedDirection.Y *= -1;
                    }

                    bulletSpeed *= friction;
                    //maxGravity.Y *= friction;
                }
                else
                {
                    normalizedDirection = Vector2.Zero;

                    if (angle > angleobjUpperRight && angle < angleobjUpperLeft)
                    {
                        maxGravity = Vector2.Zero;
                        gravity = Vector2.Zero;
                    }
                    bulletSpeed = 0;
                }

            }

        }

        public override void Update(GameTime gameTime)
        {
            // The base must always be called first.
            base.Update(gameTime);

            if (gravity.Y < maxGravity.Y)
                gravity.Y += 0.01f;


            if (totalTimeAlive >= 6000) isDestroyed = true;

            // Put your own "motion" code here, or use the basic one given by this class.
            position += normalizedDirection * bulletSpeed;
            position.Y += gravity.Y;
        }
    }
}
