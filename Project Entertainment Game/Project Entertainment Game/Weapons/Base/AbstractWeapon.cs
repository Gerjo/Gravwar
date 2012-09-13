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
    abstract public class AbstractWeapon
    {
        public int currentAmmunition = 0; // Bullets currently in this gun - when set to "0" te weapon becomes unavailable.
        public int defaultAmmunition = 0; // Default amount of bullets given. 
        
        public int fireRateDelay   = 300; // Millisectonds between each bullet. Increment for less bullets per second.
        protected int fireRateCounter = 0;   // Timestamp when the last bullet was fired. Storing this inside the gun will prevent
                                             // a bug were players can shoot unlimited bullets by changing their weapons rapidly.

        protected Texture2D normalTexture = null;
        protected Texture2D fireTexture   = null;

        // This function MUST be implemented:
        abstract public AbstractBullet getBullet(AnimPlayer ownerPlayer);

        public AbstractWeapon()
        {
            resetAmmunition();
        }

        public bool isWaitingForFireDelay()
        {

            if(fireRateCounter > fireRateDelay) {
                fireRateCounter = 0;
                return false;
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            fireRateCounter += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void resetAmmunition()
        {
            currentAmmunition = defaultAmmunition;
        }

        public bool hasAmmunition()
        {
            return (currentAmmunition > 0);
        }

        public Texture2D getNormalTexture()
        {
            return normalTexture;
        }
        public Texture2D getFireTexture()
        {
            return fireTexture;
        } 
    }
}
