using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Entertainment_Game
{
    // TODO: Ammo indicator?
    // TODO: Fix collision bug with object where player remains stuck.

    public class AnimPlayer
    {

        // Location of this player:
        public Vector2 position = new Vector2(400, 400);

        // Sprite map settings (You probably want to leave these as-is):
        private Dimension viewport = new Dimension(200, 200);
        private int frameDecrement = 200;   // Width of each sprite in the spritemap.
        private int milliesPerFrame = 40;   // Lowering this value will speed up the animation. (TODO: make this dynamic? walking slow = slow animation?)
        private float textureScale = .4f;   // Since the whole lot drawn by me is FULL HD, but since we don't actually use
                                            // proper high quality everything has to be scaled down.
                                            // IMPORTANT: if you change the scale, most other coordinates have to be changed, too.

        // Internal variables, leave as-is:
        private int frameOffset          = 0;         // Used for the sprite map.
        private int milliesFrameCounter  = 0;         // Used for the sprite map (counter for delay between sprites)
        private float gravCoolDownCount  = 99999;     // Used internally to calculate the delay between gravity switches.
        private float jumpCoolDownCount  = 99999;     // Used internally to calculate the delay between jumps.
        private float blinkCoolDownCount = 20;
        private int currentRespawnCount  = 180;
        private float lastHitCount       = 0;         // How many seconds ago was the last bullet hit.
        private bool isJumpAllowed       = true;
        private bool isWalking           = false;
        private bool isBlinking          = true;
        private bool isDead              = false;
        private float aimDirection       = 0;         // Direction we are aiming at (either joystick or mouse).
        public  Level ownerLevel;
        public  PlayerIndex playerIndex;
        public  int intPlayerIndex;
        public  Color playerColor;
        public  WeaponManager weaponManager;          // Deals with all the weapons. Initiated in constructor.
        public  GamePadInput inputDevice;             // Reads all input data. (keyboard, controller, mouse, etc)
        private Audio audio;

        // Player hitbox, adjust if we re
        public Rectangle currentHitbox = new Rectangle(0, 0, 40, 70);// The X & Y coords will be updated accordingly.

        // Textures and fonts, leave as-is:
        private Texture2D walkingTex     = null;
        private Texture2D walkingTexGlow = null;
        private Texture2D armTex         = null;
        private Texture2D armTexShoot    = null;
        private Texture2D pixelTex       = null;
        private Texture2D haltTex        = null;
        private Texture2D haltTexGlow    = null;
        private Texture2D deadTex        = null;
        private SpriteFont healthFont    = null;


        // Motion settings (Edit these figures freely):
        private int jumpPower               = 20;                       // Jump "height" of the player.
        private Vector2 velocity            = new Vector2(0, 0);        // Objects current velocity, starting speed is best left at 0.
        private Vector2 friction            = new Vector2(0.5f, 0.5f);  // Per iteration will decrease the velocity.
        private Vector2 gravityForce        = new Vector2(0.75f, 0.75f);  // Gravitational force.
        private Vector2 easingVelocity      = new Vector2(1.5f, 1.5f);    // Accelleration velocity.
        private Vector2 maxVelocity         = new Vector2(10f, 10f);    // Maximum player walking speed.
        private Vector2 maxGravity          = new Vector2(200f, 200f);  // Maximum gravity speed.
        private bool useMaxGravHarsh        = false;                    // Apply a gravity limit and includes jumping. (best left disabled)
        private bool useMaxGravGraceful     = true;                     // Apply a gravity limit, but not to jumping. (best left enabled)

        // Settings for the health, and the health "flash". (still being coded)
        public  float playerHealth          = 100;  // Players current health percentage (leave as-is).
        public  int playerMaxHealth         = 100;  // Maximum health percentage.
        private int showFlashBelowHealth    = 75;   // When to start "flashing" the health glow thing.
        private float waitTillHealthGain    = 1000; // How long to wait before the health regeneration starts? 
                                                    // using this delay we force the player to seek cover.
        private int respawnTimer            = 180;

        public int playerDied               = 0;    // Times this player died (leave as-is).
        public int playerKills              = 0;    // Kills this player has made (leave as-is).
        public int playerSuicide            = 0;    // Times the player managed to kill himself (leave as-is).


        // The actual health regenerate formulate is located in the "doHealthRegenerate" function. Edit this function
        // if you'd like to change the behaviour.

        // Set the internal delays between certain actions. TIP: Set the 0 to disable the delay.
        private float gravCoolDownLength = 0;  // Milliseconds between each "gravity" switch.
        private float jumpCoolDownLength = 0;  // Milliseconds between each jump.


        // States that player has. Leave as-is.
        public enum GravitySide { Up, Down, Left, Right }
        public GravitySide gravitySide;

        // Debug variables. We should strive to remove these (and their functionality) before the final build.
        private Texture2D mouseTexture = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/mouse");

        public AnimPlayer(Level ownerLevel, int intPlayerIndex)
        {
            this.ownerLevel     = ownerLevel;
            this.intPlayerIndex = intPlayerIndex;

            switch(intPlayerIndex) {
                case 0:
                    playerIndex = PlayerIndex.One;
                    playerColor = Color.Red;
                    break;

                case 1:
                    playerIndex = PlayerIndex.Two;
                    playerColor = Color.Blue;
                    break;
                case 2:
                    playerIndex = PlayerIndex.Three;
                    playerColor = Color.Green;  
                    break;
                case 3:
                    playerIndex = PlayerIndex.Four;
                    playerColor = Color.Yellow;
                    break;
            }
                        
            weaponManager       = new WeaponManager(this);
            audio               = new Audio();

            // If controller "one" is connected use controller as input device. Else use the computer's keyboard and mouse.
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                inputDevice = new GamePadInput(this);
            }
            else if (playerIndex == PlayerIndex.One)
            {
                inputDevice = new KeyboardInput(this);
            }


            // May have to abstract this routine into a "loadContent" function.
            walkingTex      = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/walkingSpriteMap");
            walkingTexGlow  = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/walkingSpriteMapGlow");
            armTex          = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/arm");
            armTexShoot     = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/armflash");
            pixelTex        = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/pixel");
            haltTex         = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/standingStill2");
            haltTexGlow     = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/standingStillGlow");
            deadTex         = Game1.INSTANCE.Content.Load<Texture2D>("Images/AnimPlayer/dead");
            healthFont      = Game1.INSTANCE.Content.Load<SpriteFont>("Fonts/HealthFont");
            

            // Will spawn the player at said location:
            spawnPlayer(ownerLevel.spawnPositions[intPlayerIndex]);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Draw the background arm:     
            // spriteBatch.Draw(armTex, getShoulderLocation(), new Rectangle(0, 0, 100, 100), Color.White, getBaseRotation(), new Vector2(8, 19), textureScale, SpriteEffects.None, 0);

            // DEBUG: Uncomment to draw the bounding box:
            //spriteBatch.Draw(pixelTex, new Vector2(currentHitbox.X, currentHitbox.Y), currentHitbox, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            // Mirror the image according to the movement direction.
            SpriteEffects spriteEffect = (isMirrored()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // We are in mid air, so show a frame that looks like the player is jumping:
            if (isJumpAllowed == false) frameOffset = 200;

            // Clipping canvas. The Update function will update "frameOffset" accordingly.
            Rectangle clippingCanvas = new Rectangle(frameOffset, 0, viewport.width, viewport.height);



            if (playerHealth >= 0)
            {
                if (isWalking || isJumpAllowed == false)
                {
                    // Drow the glow texture on the background, if necessary: (add flashing glow here)
                    if (playerHealth < showFlashBelowHealth)
                    {
                        spriteBatch.Draw(walkingTexGlow, position, clippingCanvas, playerColor, getBaseRotation(), new Vector2(100, 100), textureScale, spriteEffect, 0);
                    }

                    // Draw a walking sprite, or a sprite that is in mid air.
                    spriteBatch.Draw(walkingTex, position, clippingCanvas, playerColor, getBaseRotation(), new Vector2(100, 100), textureScale, spriteEffect, 0);
                }
                else
                {
                    // Drow the glow texture on the background, if necessary: (add flashing glow here)
                    if (playerHealth < showFlashBelowHealth)
                    {
                        if (isBlinking == true)
                            spriteBatch.Draw(haltTexGlow, position, new Rectangle(0, 0, clippingCanvas.Width, clippingCanvas.Height), new Color(255, 0, 0, playerHealth / 100), getBaseRotation(), new Vector2(100, 100), textureScale, spriteEffect, 0);
                    }

                    // Draw a sprite that is standing still.
                    spriteBatch.Draw(haltTex, position, new Rectangle(0, 0, clippingCanvas.Width, clippingCanvas.Height), playerColor, getBaseRotation(), new Vector2(100, 100), textureScale, spriteEffect, 0);
                }
                // Firstly check whether the weapon has been loaded already.
                if (weaponManager.getTexture() != null)
                {
                    // Draw the foreground arm: TODO: add this code: (isMirrored()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally    
                    SpriteEffects weaponMirror = (isMirrored()) ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    spriteBatch.Draw(weaponManager.getTexture(), getShoulderLocation(), new Rectangle(0, 0, 150, 40), playerColor, aimDirection, new Vector2(8, 19), textureScale, weaponMirror, 0);
                }

                // Dispatch the draw event to the weapon manager, which will draw the bullets.
                weaponManager.Draw(gameTime, spriteBatch);
            }
            else
            {
                // Dead player:
                spriteBatch.Draw(deadTex, position, new Rectangle(0, 0, 250, clippingCanvas.Height), playerColor, getBaseRotation(), new Vector2(100, 100), textureScale, spriteEffect, 0);
            }

            

            // DEBUG: Draws a mouse cursor onscreen. This should aid debugging since we can visually see where we are aiming at.
            spriteBatch.Draw(mouseTexture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);

            // DEBUG: Draw the health percentage onscreen:
            //spriteBatch.DrawString(healthFont, Math.Round(playerHealth) + "% ", position, new Color(225, 225, 225, 128));
        }

        private void recalculateCurrentHitBox()
        {
            currentHitbox.X = (int)position.X - currentHitbox.Width / 2;
            currentHitbox.Y = (int)position.Y - currentHitbox.Height / 2;
        }

        public void Update(GameTime gameTime)
        {
            // Aim angle (used by the draw method to "point" the weapon).
            if (inputDevice != null)
            { 
                aimDirection = inputDevice.getAimDirectionAngle();
            }

            // Increment some internal timers:
            gravCoolDownCount   += gameTime.ElapsedGameTime.Milliseconds;
            jumpCoolDownCount   += gameTime.ElapsedGameTime.Milliseconds;
            lastHitCount        += gameTime.ElapsedGameTime.Milliseconds;
            blinkCoolDownCount--;

            if (blinkCoolDownCount <= 0)
            {
                isBlinking = !isBlinking;
                blinkCoolDownCount = 20;
            }

            // Regenarate the player's health automatically:
            if(!isDead)
                doHealthRegenerate();

            // Check for player respawn:
            checkForRespawn();

            // Dispatch the update event to the weaponManager. This class updates the bullets, too.
            weaponManager.Update(gameTime);

            // handle any user input, such as keypressed or mousemovement:
            if (inputDevice != null)
            { 
                handleInput();
            }

            // Also handles friction and easing:
            applyGravity();

            // Update the current position:
            position += velocity;

            // As we've changed position, we also must update the bounding hitbox accordingly:
            recalculateCurrentHitBox();

            // Rotate the bounding box accordingly with the image. (The player is higher than wide, thus the boundingbox must match this)
            if ((currentHitbox.Width < currentHitbox.Height && (gravitySide == GravitySide.Left || gravitySide == GravitySide.Right)) ||
                (currentHitbox.Width > currentHitbox.Height && (gravitySide == GravitySide.Up || gravitySide == GravitySide.Down)))
            {
                currentHitbox.Width  ^= currentHitbox.Height;
                currentHitbox.Height ^= currentHitbox.Width;
                currentHitbox.Width  ^= currentHitbox.Height;
            }

            // Using time based techniques, rather than frame based.
            if ((milliesFrameCounter += gameTime.ElapsedGameTime.Milliseconds) > milliesPerFrame && isWalking)
            {
                milliesFrameCounter = 0;
                updateSpriteViewPort();
            }

            // Rotate the arm sprite, obviously this is just a demo. In due time
            // we must link this to the XBOX controller's joystick 
            //armRotation += .04f;
        }

        private void updateSpriteViewPort()
        {
            //frameOffset -= frameDecrement;

            int toggle = 1;

            // Determine which side considered "walking forward":
            switch(gravitySide) {
                case GravitySide.Down:
                    if (velocity.X > 0) toggle = -1;
                    break;
                case GravitySide.Up:
                    if (velocity.X < 0) toggle = -1;
                    break;
                case GravitySide.Left:
                    if (velocity.Y > 0) toggle = -1;
                    break;
                case GravitySide.Right:
                    if (velocity.Y < 0) toggle = -1;
                    break;
           }

            // If the player is mirrored, the feet motion must reverse, too:
            if (!isMirrored()) toggle *= -1; // Toggle the toggle!
           
            // Apply the new frame increment.
            frameOffset += frameDecrement * toggle;
            
            // Reset the sprite map if out of bounds: (jump back to the first or last frame)
            if (frameOffset < 0) frameOffset = 2200;
            if (frameOffset > 2200) frameOffset = 0;
        }

        private void applyGravity()
        {
            // Check if any of the "forces" is greater than the maximum permitted gravity:
            bool isGravLimitReached = (velocity.Y >= maxGravity.Y || velocity.Y <= maxGravity.Y * -1 || velocity.X >= maxGravity.X || velocity.X <= maxGravity.X * -1);

            // Restrict the velocity to the maximum permitted gravity.
            if (useMaxGravHarsh) velocity = Vector2.Clamp(velocity, maxGravity * -1, maxGravity);

            // Apply friction and gravity to their corresponding sides:
            switch (gravitySide)
            {
                case GravitySide.Up:   // Intentional fall through (2x)
                case GravitySide.Down:
                    if (velocity.X > 0) velocity.X -= friction.X;
                    if (velocity.X < 0) velocity.X += friction.X;
                    if (gravitySide == GravitySide.Down && (!useMaxGravGraceful || velocity.Y < maxGravity.Y)) velocity.Y += gravityForce.Y;
                    if (gravitySide == GravitySide.Up && (!useMaxGravGraceful || velocity.Y > maxGravity.Y * -1)) velocity.Y -= gravityForce.Y;

                    if (Math.Abs(velocity.X) < .4) velocity.X = 0; // This should prevent the moonwalking action:
                    isWalking = (Math.Abs(velocity.X) > 0);
                    break;
                case GravitySide.Left: // Intentional fall through (2x)
                case GravitySide.Right:
                    if (velocity.Y > 0) velocity.Y -= friction.Y;
                    if (velocity.Y < 0) velocity.Y += friction.Y;
                    if (gravitySide == GravitySide.Right && (!useMaxGravGraceful || velocity.X < maxGravity.X)) velocity.X += gravityForce.X;
                    if (gravitySide == GravitySide.Left && (!useMaxGravGraceful || velocity.X > maxGravity.X * -1)) velocity.X -= gravityForce.X;

                    if (Math.Abs(velocity.Y) < .4) velocity.Y = 0; // This should prevent the moonwalking action:
                    isWalking = (Math.Abs(velocity.Y) > 0);
                    break;

                // default case has been purposefully ommited.
            }
        }

        // If you'd like reassign buttons, you must edit the files located in the Controls folder. 
        private void handleInput()
        {
            if (isDead) return;
            // Right motion: playerController
            if (inputDevice.isRightWalk() && gravitySide != GravitySide.Right && gravitySide != GravitySide.Left)
            {
                if (velocity.X < maxVelocity.X) velocity.X += easingVelocity.X;
            }

            // Left motion:
            if (inputDevice.isLeftWalk() && gravitySide != GravitySide.Right && gravitySide != GravitySide.Left)
            {
                if (velocity.X > maxVelocity.X * -1) velocity.X -= easingVelocity.X;
            }

            // Up motion:
            if (inputDevice.isUpWalk() && gravitySide != GravitySide.Up && gravitySide != GravitySide.Down)
            {
                if (velocity.Y > maxVelocity.Y * -1) velocity.Y -= easingVelocity.X;
            }

            // Down motion:
            if (inputDevice.isDownWalk() && gravitySide != GravitySide.Up && gravitySide != GravitySide.Down)
            {
                if (velocity.Y < maxVelocity.Y * 1) velocity.Y += easingVelocity.Y;
            }

            // Jump motion 
            if (inputDevice.isJumpButton())
            {
                // The variable "isJumpAllowed" checks whether the player is on the floor or mid-air.
                // The function isAllowedToJump() checks whether there is an delay required between each jump.
                if (isJumpAllowed && isAllowedToJump())
                {
                    jumpCoolDownCount = 0; // Reset the internal timer back to 0.
                    doJump();
                }
            }

            // Fire a bullet:
            if (inputDevice.isFireBullet())
            {
                // NOTICE: The weapon manager handles the delay between each bullets.
                // NOTICE: The actual delay between each bullet is set via the corresponding weapon class. 
                weaponManager.fireBullet();
            }

            // "isAllowedToChangeGravity" accounts for the preset delay.
            if (isAllowedToChangeGravity())
            {
                gravCoolDownCount = 0; // Reset the internal timer back to 0.
                if (inputDevice.isSwitchGravLeft()) setGravitySide(GravitySide.Left);
                if (inputDevice.isSwitchGravRight()) setGravitySide(GravitySide.Right);
                if (inputDevice.isSwitchGravUp()) setGravitySide(GravitySide.Up);
                if (inputDevice.isSwitchGravDown()) setGravitySide(GravitySide.Down);
            }
        }

        private void doJump()
        {
            switch (gravitySide)
            {
                case GravitySide.Up:    velocity.Y = +jumpPower; break;
                case GravitySide.Down:  velocity.Y = -jumpPower; break;
                case GravitySide.Left:  velocity.X = +jumpPower; break;
                case GravitySide.Right: velocity.X = -jumpPower; break;
            }

            // Disallow jumping whilst in mid air. (onCollision with object will reset this value)
            isJumpAllowed = false;
        }

        // TODO: Rework the health regenerate formula.
        private void doHealthRegenerate()
        {
            // Health is only gained after xx milliseconds since the last hit. Additionally we make sure the health
            // does not rise above the maximum.
            if (lastHitCount > waitTillHealthGain && playerHealth < playerMaxHealth)
            {
                // This could be some fancy formula.
                playerHealth += 0.2f;
            }
        }

        public void setGravitySide(GravitySide gravitySide)
        {
            // Singleton, we can only switch if the gravity side isn't set to this value already.
            if (this.gravitySide != gravitySide)
            {
                isJumpAllowed = false;
                this.gravitySide = gravitySide;
            }
        }

        // Edit this function freely!
        // TODO: Disable ceiling bounce.
        private float getWallBounce(float value, GravitySide gravitySide)
        {
            // No bounce at all
            if (gravitySide == GravitySide.Left)
                return -gravityForce.X;
            else if (gravitySide == GravitySide.Right)
                return gravityForce.X;
            else if (gravitySide == GravitySide.Up)
                return -gravityForce.Y;
            else if (gravitySide == GravitySide.Down)
                return gravityForce.Y;
            else
                return 0;

            // No bounce on the floor
            //if (this.gravitySide == gravitySide) return 0;
            
            // Bounce on the floor:
            // if (this.gravitySide == gravitySide) return (float)(value * -1 * .5);

            // Bounce on any other side:
            //return (float)(value * -1 * .5);
        }

        public float getBaseRotation()
        {
            switch (gravitySide)
            {
                case GravitySide.Up:    return (float)Math.PI;
                case GravitySide.Down:  return 0;
                case GravitySide.Left:  return (float)Math.PI / 2;
                case GravitySide.Right: return (float)Math.PI + (float)Math.PI / 2;
                default: return 0;
            }

        }

        public Vector2 getShoulderLocation()
        {
            //isJumpAllowed = false; // Uncomment this line to aid debugging.

            Vector2 mirrorOffset = new Vector2(0, 0);

            bool isMirrored = this.isMirrored();
            if (!isMirrored)
            {

                switch (gravitySide)
                {
                    case GravitySide.Up:    mirrorOffset = new Vector2(-10, 0); break;
                    case GravitySide.Down:  mirrorOffset = new Vector2(10, 0);  break;
                    case GravitySide.Left:  mirrorOffset = new Vector2(0, 10);  break;
                    case GravitySide.Right: mirrorOffset = new Vector2(0, -10); break;
                }
            }

            switch (gravitySide)
            {
                case GravitySide.Up:    return Vector2.Add(mirrorOffset, Vector2.Add(position, new Vector2(5, 18)));
                case GravitySide.Down:  return Vector2.Add(mirrorOffset, Vector2.Add(position, new Vector2(-6, -20)));
                case GravitySide.Left:  return Vector2.Add(mirrorOffset, Vector2.Add(position, new Vector2(18, -7)));
                case GravitySide.Right: return Vector2.Add(mirrorOffset, Vector2.Add(position, new Vector2(-18, 4)));
            }

            // This line is never reached, however we still return a default value.
            return position;
        }

        public Vector2 getBulletSpawnLocation()
        {
            return getShoulderLocation();
        }

        // TODO: implement function.
        public void checkPlayerCollision(AnimPlayer player)
        {
            // Compare "currentHitbox" here.
            if (currentHitbox.Intersects(player.currentHitbox))
            {
                // Detect side and adjust the velocity accordingly.
            }
        }

        public void checkBulletCollision(Weapons.AbstractBullet bullet)
        {
            if (bullet.isDestroyed) return;

            if (currentHitbox.Intersects(bullet.getHitBox()) && isDead == false)
            {
                // Inflict damage on this player. The removeHealth function returns if this player died.
                if (!bullet.isHit)
                {
                    if (removeHealth(bullet.damageScore))
                    {
                        bullet.ownerPlayer.playerKills++;
                    }
                    bullet.isHit = true;
                }
                if (bullet.isFlameable == false)
                    bullet.isDestroyed = true;
                else
                {
                    // Reset the counter for when the player was last hit by a bullet.
                    lastHitCount = 0;
                    bullet.isExploding = true;
                    // Mark the bullet as destroyed (The corresponding weaponManager will delete the bullet).
                    bullet.runRemovalCounter = true;
                }
            }
        }

        public void checkObjectCollision(Object obj)
        {
            // By default we have no collision.
            Boolean hasCollision = false;

            // Verify whether the player bounding box intersects the object's bounding box:
            if (obj.position.Intersects(currentHitbox))
            {
                // Check for collision on the player's right side:
                if (currentHitbox.Right >= obj.position.Left && currentHitbox.Right < obj.position.Left + currentHitbox.Width/2 + velocity.X)
                {
                    hasCollision = true;
                    position.X   = obj.position.Left - currentHitbox.Width / 2;
                    velocity.X   = getWallBounce(velocity.X, GravitySide.Right);

                    if (gravitySide == GravitySide.Right) isJumpAllowed = true;
                }

                // Check for collision on the player's left side:
                else if (currentHitbox.Left <= obj.position.Right && currentHitbox.Left > obj.position.Right - currentHitbox.Width/2 + velocity.X)
                {
                    hasCollision = true;
                    position.X   = obj.position.Right + currentHitbox.Width / 2;
                    velocity.X   = getWallBounce(velocity.X, GravitySide.Left);

                    if (gravitySide == GravitySide.Left) isJumpAllowed = true;
                }

                // Check for collision on the player's top side:
                else if (currentHitbox.Top <= obj.position.Bottom && currentHitbox.Top > obj.position.Bottom - currentHitbox.Height/2 + velocity.Y)
                {
                    hasCollision = true;
                    position.Y   = obj.position.Bottom + currentHitbox.Height / 2;
                    velocity.Y   = getWallBounce(velocity.Y, GravitySide.Up);

                    if (gravitySide == GravitySide.Up) isJumpAllowed = true;
                }

                // Check for collision on the player's bottom side:
                else if (currentHitbox.Bottom >= obj.position.Top && currentHitbox.Bottom < obj.position.Top + currentHitbox.Height/2 + velocity.Y)
                {
                    hasCollision = true;
                    position.Y   = obj.position.Top - currentHitbox.Height / 2;
                    velocity.Y   = getWallBounce(velocity.Y, GravitySide.Down);

                    if (gravitySide == GravitySide.Down) isJumpAllowed = true;
                }

                // If we have collision, "glue" the player to this object:
                if (hasCollision)
                {
                    if (obj.movementdown)       position.Y += obj.movespeed;
                    else if (obj.movementup)    position.Y -= obj.movespeed;
                    else if (obj.movementleft)  position.X -= obj.movespeed;
                    else if (obj.movementright) position.X += obj.movespeed;

                    // Recalculate the bounding hitbox, this is necessary b/c we've resolved intersection and thus moved the player around.
                    recalculateCurrentHitBox();
                }
            }
        }

        public void checkOutOfLevel()
        {
            Rectangle hitBoxPlayerRect  = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            Rectangle playField         = new Rectangle(0, 0, 1280, 720);

            if (!hitBoxPlayerRect.Intersects(playField))
            {
                playerSuicide++;
                spawnPlayer();
            }
        }

        public void checkItemCollision(Items i)
        {
            Rectangle hitBoxPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            if (hitBoxPlayerRect.Intersects(i.position))
            {
                if (i.type != null && i.spawnTime == 0)
                {
                    if (i.type == "gw")
                    {
                        weaponManager.gwTaken();
                    }
                    else
                    {
                        weaponManager.setPickUpWeapon(i.getPickUpWeapon());
                    }
                }
                i.ItemTaken();
            }
        }

        public void spawnPlayer()
        {
            spawnPlayer(ownerLevel.spawnPositions[new Random().Next(ownerLevel.spawnPositions.Count - 1)]);
        }

        private void spawnPlayer(SpawnPosition newSpawn)
        {
            // Reset the player's heath:
            playerHealth = playerMaxHealth;

            // Assign the new location and gravity angle:
            position    = newSpawn.GetSpawnVector();
            velocity    = new Vector2();
            gravitySide = newSpawn.GetSpawnGravitySide();
            weaponManager.dropPickUpWeapon();

            // OPTIONAL: Add some code here to remove all player's bullets from the game.
        }

        // If you actually switch gravity, do not forget to reset the "gravCoolDownCount" variable to 0.
        private Boolean isAllowedToChangeGravity()
        {
            return (gravCoolDownCount >= gravCoolDownLength);
        }

        // If you actually do jump, dont forget to reset "jumpCoolDownCount" variable to 0.
        private Boolean isAllowedToJump()
        {
            return (jumpCoolDownCount >= jumpCoolDownLength);
        }


        private Boolean isMirrored()
        {
            //Console.Out.WriteLine(getAimDirectionAngle());
            float angle = aimDirection - getBaseRotation();

            return (angle > -1.5 && angle < 1.5);

            /* //Old routine:
            switch (gravitySide)
            {
                case GravitySide.Left:  return (movementDirection != MovementDirection.Up);
                case GravitySide.Right: return (movementDirection == MovementDirection.Up);
                case GravitySide.Up:    return (movementDirection == MovementDirection.Left);
                case GravitySide.Down:  return (movementDirection != MovementDirection.Left);
            }
            
            return false;*/
        }

        private bool removeHealth(int amount)
        {
            // Remove some health, respawn if necessary:
            if ((playerHealth -= amount) <= 0 && isDead == false)
            {
                isDead = true;
                playerDied++;
                switch(playerIndex)
                {
                    case PlayerIndex.One:
                        audio.PlaySound("Deathsound_01", ownerLevel.soundVolume);
                        break;
                    case PlayerIndex.Two:
                        audio.PlaySound("Deathsound_02", ownerLevel.soundVolume);
                        break;
                    case PlayerIndex.Three:
                        audio.PlaySound("Deathsound_03", ownerLevel.soundVolume);
                        break;
                    case PlayerIndex.Four:
                        audio.PlaySound("Deathsound_04", ownerLevel.soundVolume);
                        break;
                    default:
                        break;
                }
                // TODO: Start dying animation, then when this animation is finished call the "spawnPlayer()" function.
                return true;
            }
            return false;
        }

        private void checkForRespawn()
        {
            if (isDead)
            {
                currentRespawnCount--;
            }
            if (currentRespawnCount <= 0)
            {
                currentRespawnCount = respawnTimer;
                isDead = false;
                spawnPlayer();
            }
        }
    }

    // Dimension model, should reduce the amount of "height" and "width" variables.
    class Dimension
    {
        // Public variables intentionally used.
        public int width;
        public int height;

        public Dimension()
            : this(0, 0) // Constructor chaining.
        {
        }
        public Dimension(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
