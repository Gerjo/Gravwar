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
    public class RocketLauncher : AbstractWeapon
    {
        public RocketLauncher() : base()
        {
            // Load the textures:
            normalTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armRocket");
            fireTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armflashRocket");

            fireRateDelay     = 400;
            defaultAmmunition = 10; // Many bullets, ergo we'll never run out.

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--; // Remove a single bullet.
            return new RocketBullet(ownerPlayer);
        }
    }

    public class RocketBullet : AbstractBullet
    {
        public RocketBullet(AnimPlayer ownerPlayer) : base(ownerPlayer)
        {
            // Load the textures:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/rocket");
            bulletSpeed   = 25;
            damageScore   = 70;
            isFlameable   = true;
        }

        public override void Update(GameTime gameTime)
        {
            // The base must always be called first.
            base.Update(gameTime);

            // Put your own "motion" code here, or use the basic one given by this class.
            basicBulletMotion();
            if (isExploding == true && soundExplosion == false)
            {
                soundExplosion = true;
                ownerPlayer.ownerLevel.audio.PlaySound("Rocket_Hit", ownerPlayer.ownerLevel.soundVolume);
            }
        }

        public override void CheckObjectCollision(Object obj)
        {
            if (obj.position.Intersects(getHitBox()))
            {
                bulletSpeed = 0;
                if (!soundExplosion)
                {
                    ownerPlayer.ownerLevel.audio.PlaySound("Rocket_Hit", ownerPlayer.ownerLevel.soundVolume);
                    soundExplosion = true;
                    size *= 15;
                    damageScore /= 2;
                }
                isExploding = true;
                flameRemovalCount--;
                if (flameRemovalCount <= 0)
                    isDestroyed = true;
            }
        }
    }
}
