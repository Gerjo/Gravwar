using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Entertainment_Game.Weapons
{
    abstract public class AbstractBullet
    {
        // NOTICE: This class has no "settings", if you wish to customize the bullet, edit the 
        // corresponding weapon or bullet class.

        protected float bulletDamage      = 0;     // Leave as-is. The weapon class sets this value.
        public    AnimPlayer ownerPlayer = null;  // Unlike AbstractWeapon, bullets are aware of their owner.
        protected Texture2D bulletTexture = null;  // Image of this bullet.
        protected Vector2 position;                // Position of this bullets.
        protected Vector2 size = new Vector2(6,6); 
        protected Vector2 normalizedDirection;     // Vector containing the normalized direction of this bullet. (length = 1)
        protected float bulletSpeed       = 1;     // Speed of the bullet. Height means faster.
        public    int damageScore         = 10;    // Damage points this bullets inflicts upon a player
        public bool isHit = false;
        public    bool isDestroyed        = false; // When true, this bullet will not be rendered and will be deleted from the game.
        public    float totalTimeAlive    = 0;     // Total milliseconds this bullet has been alive for. 
                                                   // this timer could e.q. be used for granades to explode

        protected bool soundExplosion = false;
        public bool isFlameable = false;
        public bool isExploding = false;
        public bool runRemovalCounter = false;
        public int flameRemovalCount = 20;
        private int startFlame = 5;
        private int currentFlame = 0;
        private int maxFlame = 16;
        private Texture2D[] flameTexture = new Texture2D[16];

        public AbstractBullet(AnimPlayer ownerPlayer)
        {
            this.ownerPlayer      = ownerPlayer;

            // Link the normalized direction we are aiming at to the bullet.
            normalizedDirection = ownerPlayer.inputDevice.getAimDirectionNormalized();

            // Set base the position: "steun vector" for the bullet:
            this.position = ownerPlayer.getBulletSpawnLocation(); // ownerPlayer.getShoulderLocation();

            // Because it should appear as if the bullet comes from the gun barrel, we increment the position.
            // Increase this digit to spawn the bullet farther away from the barrel.
            this.position += normalizedDirection * 45;

            for (int i = 0; i < maxFlame; i++)
                flameTexture[i] = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/Flame/" + (i + startFlame));
        }

        // Poor naming convention... TODO: fix it!
        public virtual Rectangle getHitBox()
        {
            // This bit needs some heavy work: GerjooStyle
            // return new Rectangle((int)(position.X + normalizedDirection.X * 30) + bulletTexture.Width/2, (int)(position.Y + normalizedDirection.Y * 30) + bulletTexture.Height/2, 10, 10);
            
            // This bit needs some work: SanderStyle
            // int width = (int)(bulletTexture.Height + (bulletTexture.Width * Math.Abs(normalizedDirection.X)));
            // int height = (int)(bulletTexture.Height + (bulletTexture.Width * Math.Abs(normalizedDirection.Y)));
            // return new Rectangle((int)(position.X - width/2), (int)(position.Y - height/2), width, height);
            
            // Easy way out: SanderStyle2
            return new Rectangle((int)(position.X - size.X/2), (int)(position.Y - size.Y/2), (int)size.X, (int)size.Y);
        }

        public virtual void CheckObjectCollision(Object obj)
        {
            if(obj.position.Intersects(getHitBox()))
                isDestroyed = true;
        }

        // Draw this bullet on the spriteBatch (and thus on your screen).
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (totalTimeAlive < 1) return;
            double angle = Math.Atan2(normalizedDirection.Y, normalizedDirection.X);
            if (!isExploding)
            spriteBatch.Draw(bulletTexture, position, null, Color.White, (float)angle, new Vector2(bulletTexture.Width/2, bulletTexture.Height/2), 1, SpriteEffects.FlipVertically, 0);
            else
            {
                if (currentFlame == 15)
                    currentFlame = 0;
                spriteBatch.Draw(flameTexture[currentFlame], position, null, Color.White, 0, new Vector2(flameTexture[currentFlame].Width / 2, flameTexture[currentFlame].Height / 2), 3, SpriteEffects.None, 0);
                currentFlame++;
            }
            // The following 3 lines can be used to visualise the bounding box:
            // Texture2D solidColor     = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/pixelWhite");
            // Rectangle boundingHitBox = getHitBox();
            // spriteBatch.Draw(solidColor, boundingHitBox, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        // Calculate the bullet trajectory here. Could be override to create e.g: "heat seeking missiles".
        public virtual void Update(GameTime gameTime)
        {
            totalTimeAlive += gameTime.ElapsedGameTime.Milliseconds;
            if (runRemovalCounter)
            {
                bulletSpeed = 0;
                flameRemovalCount--;
            }
            if (flameRemovalCount <= 0)
                isDestroyed = true;
        }

        public void basicBulletMotion()
        {
            // Basic vector math, take the normalized vector then multiply it with a "velocity".
            position += normalizedDirection * bulletSpeed;
        }
    }
}
