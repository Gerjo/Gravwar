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
    public class MachineGun : AbstractWeapon
    {
        public MachineGun() : base()
        {
            // Load the textures:
            normalTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armUzi");
            fireTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/armflashUzi");

            fireRateDelay     = 50;
            defaultAmmunition = 100;

            resetAmmunition();
        }

        // Return a new bullet instance that belongs to this type of gun.
        public override AbstractBullet getBullet(AnimPlayer ownerPlayer)
        {
            currentAmmunition--;// Remove a single bullet.
            
            return new MachineGunBullet(ownerPlayer);
        }
    }

    public class MachineGunBullet : AbstractBullet
    {
        public MachineGunBullet(AnimPlayer ownerPlayer) : base(ownerPlayer)
        {
            // Load the texture:
            bulletTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/bulletMachineGun");
            bulletSpeed   = 30;
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
