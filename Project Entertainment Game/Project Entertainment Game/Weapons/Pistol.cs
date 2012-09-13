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
    public class Pistol : AbstractWeapon
    {
        public Pistol() : base()
        {
            // Load the textures:
            normalTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/arm");
            fireTexture   = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armflash");

            fireRateDelay     = 300;
            defaultAmmunition = 999999999; // Many bullets, ergo we'll never run out.

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--; // Remove a single bullet.
            return new PistolBullet(ownerPlayer);
        }
    }

    public class PistolBullet : AbstractBullet
    {
        public PistolBullet(AnimPlayer ownerPlayer) : base(ownerPlayer)
        {
            // Load the textures:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/bulletRotate");
            bulletSpeed   = 45;
            damageScore   = 20;
        }

        public override void Update(GameTime gameTime)
        {
            // The base must always be called first.
            base.Update(gameTime);

            // Put your own "motion" code here, or use the basic one given by this class.
            basicBulletMotion();
        }
    }
}
