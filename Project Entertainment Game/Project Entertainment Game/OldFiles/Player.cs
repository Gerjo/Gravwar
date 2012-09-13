using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Project_Entertainment_Game
{
    public class Player
    {
        //Variables
        public Level level;
        private Audio audio;
        private Texture2D playerTex;
        private Texture2D cooldownTex;
        public PlayerIndex playerIndex;
        public Weapon weapon;

        public enum GravitySide { Up, Down, Left, Right }
        public GravitySide gravitySide;

        private enum MovementDirection { Up, Down, Left, Right, AnalogX, AnalogY, Jump }

        public Rectangle intersecting;

        //Adjustables
        private const float STARTHEALTH = 100;

        public Vector2 position = new Vector2(0, 0);
        public Vector2 GUIposition;
        public Rectangle playerHitbox = new Rectangle(0, 0, 16, 46); //Must be divideable by 2.

        public Vector2 playerOffset = new Vector2(175f, 175f);
        private int width = 50;
        private int height = 50;
        public int gravity = 3;
        private int gravityCoolDownTime = 200;
        private int speed = 2;
        private double friction = 0.9;
        private double friction2 = 1;
        private int jumpPower = 75;
        private bool jumpAllowed = false;

        //Don't touch
        private float moveSpeedX;
        private float moveSpeedY;
        private int moveGravity;

        public int kills = 0;
        public int deaths = 0;
        public float health = STARTHEALTH;

        public float currentRotation;
        public Rectangle currentHitbox = new Rectangle(0, 0, 0, 0);
        private int currentGravityCoolDownTime = 0;
        private bool onFloor = false;

        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;
        private int currentMouseWheelValue;

        /*
         * The constructor of player requires the playernumber and
         * the level that calls it.
         */ 
        public Player(int playerNumber, Level level)
        {
            if (playerNumber == 0)
            {
                this.playerIndex = PlayerIndex.One;
                GUIposition = new Vector2(10, 20);
            }
            else if (playerNumber == 1)
            {
                this.playerIndex = PlayerIndex.Two;
                GUIposition = new Vector2(70, 20);
            }
            else if (playerNumber == 2)
            {
                this.playerIndex = PlayerIndex.Three;
                GUIposition = new Vector2(130, 20);
            }
            else if (playerNumber == 3)
            {
                this.playerIndex = PlayerIndex.Four;
                GUIposition = new Vector2(190, 20);
            }
            this.level = level;
            
            LoadContent(playerNumber);
        }

        /*
         * This function loads the images to the screen. And also initializes an
         * new audio class. The player also gets a weapon... ofcourse.
         */
        public void LoadContent(int playerNumber)
        {
            playerTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/nietmariogroen");
            playerTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/nietmariogroen");
            cooldownTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/mousePointer");
            
            audio = new Audio();
            weapon = new Weapon(this, playerIndex);
            
            Spawn();
        }

        /*
         * The update function is split up into different functions because this function
         * would get very big and messy and we dont want that.
         */
        public void Update(GameTime gameTime)
        {
            Input();
            GravitySwitch();
            CheckOutOfLevel();
            CheckHealth();
            weapon.Update(gameTime, position);
        }

        /*
         * This function draws the player, hud and weapon.
         */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (playerIndex == PlayerIndex.One)
            {
                spriteBatch.Draw(playerTex, new Rectangle((int)position.X, (int)position.Y, width, height), null, Color.Red, currentRotation, playerOffset, SpriteEffects.None, 0);
            }
            if (playerIndex == PlayerIndex.Two)
            {
                spriteBatch.Draw(playerTex, new Rectangle((int)position.X, (int)position.Y, width, height), null, Color.Blue, currentRotation, playerOffset, SpriteEffects.None, 0);
            }
            if (playerIndex == PlayerIndex.Three)
            {
                spriteBatch.Draw(playerTex, new Rectangle((int)position.X, (int)position.Y, width, height), null, Color.Green, currentRotation, playerOffset, SpriteEffects.None, 0);
            }
            if (playerIndex == PlayerIndex.Four)
            {
                spriteBatch.Draw(playerTex, new Rectangle((int)position.X, (int)position.Y, width, height), null, Color.Yellow, currentRotation, playerOffset, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(cooldownTex, new Rectangle((int)GUIposition.X, (int)GUIposition.Y, 50-(currentGravityCoolDownTime/10), 10), Color.White);
            weapon.Draw(gameTime, spriteBatch);
        }
        
        /*
         * When the function Spawn is called. The player spawns or respawns.
         * The spawn is picked randomly from the level.spawnPositions.
         */
        public void Spawn()
        {
            int spawnPositionsCount = 0;
            foreach (SpawnPosition s in level.spawnPositions)
            {
                if (s != null)
                    spawnPositionsCount++;
            }
            SpawnPosition spawn = level.spawnPositions[Game1.INSTANCE.random.Next(spawnPositionsCount)];
            position = new Vector2(spawn.x, spawn.y);
            
            health = STARTHEALTH;

            if (spawn.gravity == "down")
            {
                gravitySide = GravitySide.Down;
            }
            else if (spawn.gravity == "up")
            {
                gravitySide = GravitySide.Up;
            }
            else if (spawn.gravity == "left")
            {
                gravitySide = GravitySide.Left;
            }
            else if (spawn.gravity == "right")
            {
                gravitySide = GravitySide.Right;
            }
            weapon.currentWeapon = "Pistol";
            weapon.machinegun_available = true;
            weapon.grenadelauncher_available = false;
            weapon.rocketlauncher_available = false;
        }

        /*
         * The Input function is getting associating all the keys
         * to different actions.
         */
        private void Input()
        {
            //GamePad Controls

            currentGamePadState = GamePad.GetState(playerIndex);

            /*if (GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed)
                Game1.INSTANCE.Exit();*/

            if (currentGamePadState.DPad.Down == ButtonState.Pressed && currentGravityCoolDownTime <= 0)
            {
                gravitySide = GravitySide.Down;
                currentGravityCoolDownTime = gravityCoolDownTime;
            }
            if (currentGamePadState.DPad.Up == ButtonState.Pressed && currentGravityCoolDownTime <= 0)
            {
                gravitySide = GravitySide.Up;
                currentGravityCoolDownTime = gravityCoolDownTime;
            }
            if (currentGamePadState.DPad.Right == ButtonState.Pressed && currentGravityCoolDownTime <= 0)
            {
                gravitySide = GravitySide.Right;
                currentGravityCoolDownTime = gravityCoolDownTime;
            }
            if (currentGamePadState.DPad.Left == ButtonState.Pressed && currentGravityCoolDownTime <= 0)
            {
                gravitySide = GravitySide.Left;
                currentGravityCoolDownTime = gravityCoolDownTime;
            }

            //Keyboard Controls
            if (playerIndex == PlayerIndex.One)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && currentGravityCoolDownTime <= 0)
                {
                    gravitySide = GravitySide.Down;
                    currentGravityCoolDownTime = gravityCoolDownTime;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && currentGravityCoolDownTime <= 0)
                {
                    gravitySide = GravitySide.Up;
                    currentGravityCoolDownTime = gravityCoolDownTime;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && currentGravityCoolDownTime <= 0)
                {
                    gravitySide = GravitySide.Right;
                    currentGravityCoolDownTime = gravityCoolDownTime;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && currentGravityCoolDownTime <= 0)
                {
                    gravitySide = GravitySide.Left;
                    currentGravityCoolDownTime = gravityCoolDownTime;
                }

                if (Mouse.GetState().ScrollWheelValue > currentMouseWheelValue)
                {
                    weapon.Previousweapon();
                    currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;
                }

                if (Mouse.GetState().ScrollWheelValue < currentMouseWheelValue)
                {
                    weapon.Nextweapon();
                    currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Q)) { weapon.Previousweapon(); }
                if (Keyboard.GetState().IsKeyDown(Keys.E)) { weapon.Nextweapon(); }
                
                if (Keyboard.GetState().IsKeyDown(Keys.W)) { MovePlayer(MovementDirection.Up, speed); }
                if (Keyboard.GetState().IsKeyDown(Keys.S)) { MovePlayer(MovementDirection.Down, speed); }
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { MovePlayer(MovementDirection.Left, speed); }
                if (Keyboard.GetState().IsKeyDown(Keys.D)) { MovePlayer(MovementDirection.Right, speed); }
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) { MovePlayer(MovementDirection.Jump, jumpPower); }

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
                    weapon.Shoot(new Vector2(0, -1));
                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                    weapon.Shoot(new Vector2(0, 1));
                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                    weapon.Shoot(new Vector2(-1, 0)); 
                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                    weapon.Shoot(new Vector2(1, 0));
            }
            
            MovePlayer(MovementDirection.AnalogX, currentGamePadState.ThumbSticks.Left.X * speed);
            MovePlayer(MovementDirection.AnalogY, currentGamePadState.ThumbSticks.Left.Y * speed * -1);

            if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed && currentGamePadState != previousGamePadState)
                MovePlayer(MovementDirection.Jump, jumpPower);

            //if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
            weapon.Shoot();

            if (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && currentGamePadState != previousGamePadState)
                weapon.Previousweapon();

            /*if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                weapon.Nextweapon();*/

            previousGamePadState = currentGamePadState;
        }

        /*
         * The GravitySwitch function is checking
         * what the gravity should be and how it should
         * act if the gravity is switched.
         */
        private void GravitySwitch()
        {
            moveSpeedX = (int)(friction * moveSpeedX);
            moveSpeedY = (int)(friction * moveSpeedY);

            if(currentGravityCoolDownTime > 0)
                currentGravityCoolDownTime--;

            if (onFloor)
                moveGravity = 0;
            else
            {
                moveGravity = gravity;
            }

            if (gravitySide == GravitySide.Up)
            {
                moveSpeedY -= moveGravity;
                moveSpeedY = (int)(friction2 * moveSpeedY);
                currentRotation = (float)Math.PI;
                currentHitbox.Width = playerHitbox.Width;
                currentHitbox.Height = playerHitbox.Height;
            }
            else if (gravitySide == GravitySide.Down)
            {
                moveSpeedY += moveGravity;
                moveSpeedY = (int)(friction2 * moveSpeedY);
                currentRotation = 0.0f;
                currentHitbox.Width = playerHitbox.Width;
                currentHitbox.Height = playerHitbox.Height;
            }
            else if (gravitySide == GravitySide.Left)
            {
                moveSpeedX -= moveGravity;
                moveSpeedX = (int)(friction2 * moveSpeedX);
                currentRotation = (float)Math.PI/2;
                currentHitbox.Width = playerHitbox.Height;
                currentHitbox.Height = playerHitbox.Width;
            }
            else if (gravitySide == GravitySide.Right)
            {
                moveSpeedX += moveGravity;
                moveSpeedX = (int)(friction2 * moveSpeedX);
                currentRotation = (float)Math.PI + (float)Math.PI / 2;
                currentHitbox.Width = playerHitbox.Height;
                currentHitbox.Height = playerHitbox.Width;
            }

            position.X += moveSpeedX;
            position.Y += moveSpeedY;
        }

        /*
         * This function actually moves the player if input asks for it.
         * The movementDirection can be Up, Left or Jump for example.
         * It can also receive analog input from the gamepad.
         */
        private void MovePlayer(MovementDirection movementDirection, int movementValue)
        {
            MovePlayer(movementDirection, (float)movementValue);
        }

        private void MovePlayer(MovementDirection movementDirection, float movementValue)
        {
            if (gravitySide == GravitySide.Left || gravitySide == GravitySide.Right)
            {
                if (movementDirection == MovementDirection.Up)
                {
                    moveSpeedY -= movementValue;
                }
                if (movementDirection == MovementDirection.Down)
                {
                    moveSpeedY += movementValue;
                }
            }
            else if (gravitySide == GravitySide.Up || gravitySide == GravitySide.Down)
            {
                if (movementDirection == MovementDirection.Left)
                {
                    moveSpeedX -= movementValue;
                }
                if (movementDirection == MovementDirection.Right)
                {
                    moveSpeedX += movementValue;
                }
            }
            if (movementDirection == MovementDirection.Jump)
            {
                if (gravitySide == GravitySide.Up && jumpAllowed)
                {
                    moveSpeedY += movementValue;
                    jumpAllowed = false;
                }
                else if (gravitySide == GravitySide.Down && jumpAllowed)
                {
                    moveSpeedY -= movementValue;
                    jumpAllowed = false;
                }
                else if (gravitySide == GravitySide.Left && jumpAllowed)
                {
                    moveSpeedX += movementValue;
                    jumpAllowed = false;
                }
                else if (gravitySide == GravitySide.Right && jumpAllowed)
                {
                    moveSpeedX -= movementValue;
                    jumpAllowed = false;
                }
            }
            if (movementDirection == MovementDirection.AnalogX)
            {
                moveSpeedX += movementValue;
            }
            if (movementDirection == MovementDirection.AnalogY)
            {
                moveSpeedY += movementValue;
            }
            onFloor = false;
        }

        /*
         * This function checks if this player is colliding with
         * with another player that's given.
         */
        public void CheckPlayerCollision(Player player)
        {
            Rectangle currentPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            Rectangle otherPlayerRect = new Rectangle((int)player.position.X - player.currentHitbox.Width / 2, (int)player.position.Y - player.currentHitbox.Height / 2, player.currentHitbox.Width, player.currentHitbox.Height);
            if (otherPlayerRect.Intersects(currentPlayerRect))
            {
                if (currentPlayerRect.Right > otherPlayerRect.Left && currentPlayerRect.Right < otherPlayerRect.Left + moveSpeedX + 1)
                {
                    moveSpeedX = 0;
                    position.X = otherPlayerRect.Left - currentHitbox.Width / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Right)
                        jumpAllowed = true;
                }

                else if (currentPlayerRect.Left < otherPlayerRect.Right && currentPlayerRect.Left > otherPlayerRect.Right + moveSpeedX - 1)
                {
                    moveSpeedX = 0;
                    position.X = otherPlayerRect.Right + currentHitbox.Width / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Left)
                        jumpAllowed = true;
                }

                else if (currentPlayerRect.Top < otherPlayerRect.Bottom && currentPlayerRect.Top > otherPlayerRect.Bottom + moveSpeedY - 1)
                {
                    moveSpeedY = 0;
                    position.Y = otherPlayerRect.Bottom + currentHitbox.Height / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Up)
                        jumpAllowed = true;
                }

                else if (currentPlayerRect.Bottom > otherPlayerRect.Top && currentPlayerRect.Bottom < otherPlayerRect.Top + moveSpeedY + 1)
                {
                    moveSpeedY = 0;
                    position.Y = otherPlayerRect.Top - currentHitbox.Height / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Down)
                        jumpAllowed = true;
                }
            }
        }

        /*
         * This function checks if the player is colliding with the object that's given.
         */
        public void CheckObjectCollision(Object obj)
        {
            Rectangle hitBoxPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            if(obj.position.Intersects(hitBoxPlayerRect))
            {
                if(hitBoxPlayerRect.Right > obj.position.Left && hitBoxPlayerRect.Right < obj.position.Left + moveSpeedX + 1)
                {
                    moveSpeedX = 0;
                    position.X = obj.position.Left - currentHitbox.Width / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Right)
                        jumpAllowed = true;
                }

                else if (hitBoxPlayerRect.Left < obj.position.Right && hitBoxPlayerRect.Left > obj.position.Right + moveSpeedX - 1)
                {
                    moveSpeedX = 0;
                    position.X = obj.position.Right + currentHitbox.Width / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Left)
                        jumpAllowed = true;
                }

                else if (hitBoxPlayerRect.Top < obj.position.Bottom && hitBoxPlayerRect.Top > obj.position.Bottom + moveSpeedY - 1)
                {
                    moveSpeedY = 0;
                    position.Y = obj.position.Bottom + currentHitbox.Height / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Up)
                        jumpAllowed = true;
                }

                else if (hitBoxPlayerRect.Bottom > obj.position.Top && hitBoxPlayerRect.Bottom < obj.position.Top + moveSpeedY + 1)
                {
                    moveSpeedY = 0;
                    position.Y = obj.position.Top - currentHitbox.Height / 2;
                    onFloor = true;
                    if (gravitySide == GravitySide.Down)
                     jumpAllowed = true;
                }

                if (obj.movementdown)
                {
                    position.Y += obj.movespeed;
                }
                else if (obj.movementup)
                {
                    position.Y -= obj.movespeed;
                }
                else if (obj.movementleft)
                {
                    position.X -= obj.movespeed;
                }
                else if (obj.movementright)
                {
                    position.X += obj.movespeed;
                }
            }
        }

        /*
         * This function checks if the player is grabbing an item
         */
        public void CheckItemCollision(Items i)
        {
            Rectangle hitBoxPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            if(hitBoxPlayerRect.Intersects(i.position))
            {
                if (i.type != null && i.spawnTime == 0)
                {
                    if (i.type == "rocketlauncher")
                    {
                        weapon.rocketlauncher_available = true;
                    }
                    else if (i.type == "grenadelauncher")
                    {
                        weapon.grenadelauncher_available = true;
                    }
                }
                i.ItemTaken();
            }
        }

        /*
         * The CheckOutOfLevel function checks if the player
         * is still in the level. If the player isn't, then
         * the player fell out of the level and died.
         */
        public void CheckOutOfLevel()
        {
            Rectangle hitBoxPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);
            Rectangle playField = new Rectangle(0,0,1280,720);

            if (!hitBoxPlayerRect.Intersects(playField))
            {
                kills -= 1;
                health = 0;
            }
        }

        /*
         * The CheckBulletCollision checks if the player is
         * colliding with the given bullet and if the
         * player is colliding, the player will lose health.
         * When the health has reached 0. The other player
         * will gain an kill.
         */
        public bool CheckBulletCollision(Bullet b)
        {
            Rectangle hitBoxPlayerRect = new Rectangle((int)position.X - currentHitbox.Width / 2, (int)position.Y - currentHitbox.Height / 2, currentHitbox.Width, currentHitbox.Height);

            if (b.hitbox.Intersects(hitBoxPlayerRect))
            {
                health -= b.damage;
                if (health <= 0)
                {
                    b.player.kills++;
                }
                return true;
            }
            return false;
        }

        /*
         * If the health of the player reaches 0. The player
         * will die and respawn.
         */
        public void CheckHealth()
        {
            if (health <= 0)
            {
                audio.PlaySound("aaargh", level.soundVolume);
                deaths++;
                Spawn();
            }
        }
    }
}
