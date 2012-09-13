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
    // TODO Switch weapons.
    public class WeaponManager
    {
        private AnimPlayer ownerPlayer;                       // Reference to the player which owns this weapon manager.
        private Audio audio;
        private Weapons.AbstractWeapon currentWeapon = null;  // Reference to the weapon currently in use.
        
        private Weapons.AbstractWeapon defaultWeapon = null;  // Default weapon.
        private Weapons.AbstractWeapon pickupWeapon  = null;  // Weapon that has been picked up

        public  List<Weapons.AbstractBullet> allBullets; // Holds ALL bullets spawned by this player.

        // Used for weaponflash:
        private bool isShooting           = false; // Used for bullet "flash" animation state, leave as-is.
        private float shootingDuration    = 50;    // Milliseconds the "flash" should be shown for. Increase to show longer.
        private float shootingTimeElapsed = 0;     // Internal counter, leave as-is.

        public WeaponManager(AnimPlayer player)
        {
            this.ownerPlayer = player;
            audio = new Audio();
            // Create the collections which will hold our bullets fired/weapons. Mind the clever use of "generics"
            allBullets  = new List<Weapons.AbstractBullet>();

            //defaultWeapon = new Weapons.HeatSeeking();
            defaultWeapon = new Weapons.Pistol();
            //defaultWeapon = new Weapons.BouncyGun();
            pickupWeapon  = null;

            currentWeapon = defaultWeapon;
        }

        public void setPickUpWeapon(Weapons.AbstractWeapon someWeapon)
        {
            // Pickup will always override the current pickup.
            pickupWeapon  = someWeapon;
            currentWeapon = pickupWeapon;
        }

        public void dropPickUpWeapon()
        {
            pickupWeapon = null;
        }

        // Returns the texture of a hand holding a gun. The texture iself is stored within the 
        // corresponding weapon class.
        public Texture2D getTexture()
        {
            if (currentWeapon == null) return null; // Weapon hasn't been loaded yet.

            // If the "state" is shooting, return an image with a nice "flash" commong from the barrel.
            if (isShooting) return currentWeapon.getFireTexture();

            // If we're not shooting, return an image without the aforementioned "flash"
            return currentWeapon.getNormalTexture();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Dispatch the draw each individual bullet:
            foreach (Weapons.AbstractBullet bullet in allBullets)
            {
                bullet.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Increment the time elapsed since we work time based, rather than frame based.
            shootingTimeElapsed += gameTime.ElapsedGameTime.Milliseconds;

            // Determine whether to hide the gun "flash":
            if (isShooting && shootingTimeElapsed > shootingDuration)
            {
                isShooting          = false;
                shootingTimeElapsed = 0;
            }

            // Dispatch the update events to each bullet:
            for(int i = 0; i < allBullets.Count; ++i) {
                // Check if the bullet is still "alive"
                if (!allBullets[i].isDestroyed)
                {
                    allBullets[i].Update(gameTime);
                }
                else
                {
                    allBullets.RemoveAt(i);

                    // The decrement is required b/c we're removing an item from the stack
                    // thus it is not required to jump to the next item as this item will 
                    // automatically fall into the recently "deleted" spot.
                    if (i > 0) --i;
                }
            }

            // Dispatch the update events to each weapon:
            if (pickupWeapon != null)  pickupWeapon.Update(gameTime); 
            if (defaultWeapon != null) defaultWeapon.Update(gameTime);
        }

        public void fireBullet()
        {
            // If the pickupWeapon has bullets, use it. Else use the default weapon.
            if (pickupWeapon != null && pickupWeapon.hasAmmunition())
            {
                currentWeapon = pickupWeapon;
            }
            else 
            { 
                currentWeapon = defaultWeapon; 
            }

            if (!currentWeapon.isWaitingForFireDelay())
            {
                // Boolean for showing the weapon flash:
                isShooting = true;

                // Obtain a bullet from the current weapon. Notice how we pass the "owner" as argument.
                // when the bullet hits another player, we can give "points" to the ownerPlayer.
                Weapons.AbstractBullet tempBullet = currentWeapon.getBullet(ownerPlayer);

                // Add the aforehand created bullet to the bullet collection:
                allBullets.Add(tempBullet);

                if (currentWeapon is Weapons.Pistol)
                    audio.PlaySound("MachineGun_Fire", (ownerPlayer.ownerLevel.soundVolume/10)*4);
                else if (currentWeapon is Weapons.MachineGun)
                    audio.PlaySound("MachineGun_Fire", (ownerPlayer.ownerLevel.soundVolume / 10) * 4);
                else if (currentWeapon is Weapons.RocketLauncher)
                    audio.PlaySound("Rocket_Fire", (ownerPlayer.ownerLevel.soundVolume / 10) * 4);
                else if (currentWeapon is Weapons.GrenadeLauncher)
                    audio.PlaySound("Rocket_Fire", (ownerPlayer.ownerLevel.soundVolume / 10) * 4);
            }
        }

        public void gwTaken()
        {
            currentWeapon.fireRateDelay /= 2;
        }
    }
}
