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
    public class HeatSeeking : AbstractWeapon
    {
        public HeatSeeking() : base()
        {
            // Load the textures:
            normalTexture   = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/arm");
            fireTexture     = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/armflash");

            fireRateDelay       = 20;
            defaultAmmunition   = 1000000;

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--; // Remove a single bullet.

            return new HeatSeekingBullet(ownerPlayer);
        }
    }

    public class HeatSeekingBullet : AbstractBullet
    {
        private int targetPlayerIndex = -1; // Who are we "attacking"

        public HeatSeekingBullet(AnimPlayer ownerPlayer) : base(ownerPlayer)
        {
            // Load the texture:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/bulletRotate");
            bulletSpeed = 2;
            damageScore = 10;

            // Find the player closest by:
            targetPlayerIndex = getPlayerIndexClosestToBullet();

            // Do something here... ?
            if (targetPlayerIndex == -1) { }

        }

        private int getPlayerIndexClosestToBullet()
        {
            // Using this routine we can find the player which is the closest to this bullet:
            float closestDistance = -1;
            int closestIndex      = -1;
            for (int i = 0; i < ownerPlayer.ownerLevel.animPlayers.Length; ++i)
            {
                // Firstly make sure we're not attacking ourselfs:
                if (ownerPlayer.ownerLevel.animPlayers[i] != ownerPlayer)
                {
                    // Get the euclidean distance (squared) to the player:
                    float distance = Vector2.DistanceSquared(position, ownerPlayer.ownerLevel.animPlayers[i].position);

                    // Find the closest player to this bullet.
                    if (closestIndex == -1 || closestDistance > distance)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }

            }
            
            return closestIndex;
        }

        // TODO: make this work.
        public override void Update(GameTime gameTime)
        {
            // The base must always be called first.
            base.Update(gameTime);

            targetPlayerIndex = 1;

            // This could've worked.
            //double dotProduct = Vector2.Dot(ownerPlayer.ownerLevel.animPlayers[targetPlayerIndex].position, position);

            Vector2 normalizedPosition = new Vector2(normalizedDirection.X, normalizedDirection.Y);
            normalizedPosition.Normalize();

            Vector2 normalizedTarget = new Vector2(ownerPlayer.ownerLevel.animPlayers[targetPlayerIndex].position.Y, ownerPlayer.ownerLevel.animPlayers[targetPlayerIndex].position.X);
            normalizedTarget.Normalize();

            double angle1 = Math.Atan2(normalizedPosition.Y, normalizedPosition.X);
            double angle2 = Math.Atan2(normalizedTarget.Y, normalizedTarget.X);

            // Angle between bullet and player:
            double angle = angle1 - angle2;

            float clamp = 10f;

            if (angle > clamp)  angle = clamp;
            if (angle > -clamp) angle = -clamp;

            Vector2 fancy = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            fancy.Normalize();

            normalizedDirection = fancy;

            // Put your own "motion" code here, or use the basic one given by this class.
            basicBulletMotion();
        }
    }
}
