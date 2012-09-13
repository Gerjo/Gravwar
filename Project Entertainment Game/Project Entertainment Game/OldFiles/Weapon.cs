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
     
    public class Weapon
    {
        //Set the properties of different kinds of ammo
        public const int PISTOLSPEED = 50;
        public const int PISTOLDELAY = 30;
        public const int PISTOLDAMAGE = 20;

        public const int MACHINEGUNSPEED = 50;
        public const int MACHINEGUNDELAY = 5;
        public const int MACHINEGUNDAMAGE = 10;

        public const int ROCKETSPEED = 30;
        public const int ROCKETDELAY = 50;
        public const int ROCKETDAMAGE = 50;

        public const int GRENADESPEED = 50;
        public const int GRENADEDELAY = 30;
        public const int GRENADEDAMAGE = 50;

        public const int BULLETDESTROYTIME = 5 * 60;

        //Variables
        private int weaponSlot = 1;
        private int weaponCount = 4;

        public bool pistol_available = true;
        public bool machinegun_available = true;
        public bool rocketlauncher_available = true;
        public bool grenadelauncher_available = true;

        private Vector2 position;
        private float rotation = 0;
        private Player player;
        private PlayerIndex playerIndex;
        private Vector2 weaponOffset = Vector2.Zero;

        public List<Bullet> bullets = new List<Bullet>();

        public string currentWeapon;

        private Texture2D pistolTex;
        private Texture2D machinegunTex;
        private Texture2D rocketlauncherTex;
        private Texture2D grenadelauncherTex;

        private int currentTime = 0;
        private int setTime;

        private Audio audio = new Audio();
                
        public Weapon(Player player, PlayerIndex playerIndex)
        {
            this.player = player;
            this.playerIndex = playerIndex;

            currentWeapon = "Pistol";

            pistolTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/gun");
            machinegunTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/gun");
            rocketlauncherTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/gun");
            grenadelauncherTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Weapons/gun");
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            
            this.position = Vector2.Add(weaponOffset, position);
            for (int i = bullets.Count-1; i >= 0; i--)
            {
                if (bullets[i] != null)
                {
                    bullets[i].Update(gameTime);
                   
                    //Check if hitting an player
                    /*foreach (Player p in player.level.player)
                    {
                        if (p != null && p != player)
                        {
                            if (p.CheckBulletCollision(bullets[i]))
                            {
                                if (bullets[i].weapon == "Rocketlauncher")
                                {
                                    audio.PlaySound("Rocket_Hit", );
                                }
                                else if (bullets[i].weapon == "Grenadelauncher")
                                {
                                    audio.PlaySound("Grenade_Hit", 0.6f);
                                }
                                bullets[i].destroyed = true;
                            }
                        }
                    }*/

                    //Check if hitting an object
                    foreach (Object o in player.level.objects)
                    {
                        if (o != null && bullets[i] != null)
                        {
                            if (o.position.Intersects(bullets[i].hitbox))
                            {
                                if (bullets[i].weapon == "Rocketlauncher")
                                {
                                    audio.PlaySound("Rocket_Hit", 0.6f);
                                }
                                else if (bullets[i].weapon == "Grenadelauncher")
                                {
                                    //audio.PlaySound("Grenade_Bounce", 0.6f);
                                    bullets[i].direction.Y *= -1;
                                    bullets[i].speed = (int)(0.7 * bullets[i].speed);
                                }
                                if (bullets[i].weapon != "Grenadelauncher")
                                {
                                    bullets[i].destroyed = true;
                                }
                            }
                        }
                    }

                    if (bullets[i].timeInGame == BULLETDESTROYTIME)
                    {
                        bullets[i].destroyed = true;
                    }

                    if (bullets[i].destroyed)
                    {
                        bullets.Remove(bullets[i]);
                    }
                }
            }

            currentTime++;

            SwitchWeapon();
             
        }
        
        public void Shoot()
        {
            Vector2 directionVector = new Vector2(GamePad.GetState(playerIndex).ThumbSticks.Right.X, GamePad.GetState(playerIndex).ThumbSticks.Right.Y * -1);
            if(Math.Sqrt((GamePad.GetState(playerIndex).ThumbSticks.Right.X * GamePad.GetState(playerIndex).ThumbSticks.Right.X) +
                (GamePad.GetState(playerIndex).ThumbSticks.Right.Y * GamePad.GetState(playerIndex).ThumbSticks.Right.Y)) > 0.8)
                Shoot(directionVector);
        }

        public void Shoot(Vector2 directionVector)
        {
            if (currentTime > setTime)
            {
                if (currentWeapon == "Pistol")
                {
                    audio.PlaySound("MachineGun_Fire", 0.4f);
                    bullets.Add(new Bullet(player, position, directionVector, PISTOLSPEED, PISTOLDAMAGE, currentWeapon));
                    setTime = PISTOLDELAY;
                }

                if (currentWeapon == "Machinegun")
                {
                    audio.PlaySound("MachineGun_Fire", 0.4f);
                    bullets.Add(new Bullet(player, position, directionVector, MACHINEGUNSPEED, MACHINEGUNDAMAGE, currentWeapon));
                    setTime = MACHINEGUNDELAY;
                }

                if (currentWeapon == "Rocketlauncher")
                {
                    audio.PlaySound("Rocket_Fire", 0.4f);
                    bullets.Add(new Bullet(player, position, directionVector, ROCKETSPEED, ROCKETDAMAGE, currentWeapon));
                    setTime = ROCKETDELAY;
                }

                if (currentWeapon == "Grenadelauncher")
                {
                    bullets.Add(new Bullet(player, position, directionVector, GRENADESPEED, GRENADEDAMAGE, currentWeapon));
                    setTime = GRENADEDELAY;
                }
                currentTime = 0;
            }
        }

        public void Nextweapon()
        {
            weaponSlot++;
        }

        public void Previousweapon()
        {
            weaponSlot--;
        }

        private void SwitchWeapon()
        {
            if (weaponSlot == 0)
            {
                weaponSlot = weaponCount;
            }
            if (weaponSlot == 1)
            {
                currentWeapon = "Pistol";
            }
            if (weaponSlot == 2 && machinegun_available)
            {
                currentWeapon = "Machinegun";
            }
            if (weaponSlot == 3 && rocketlauncher_available)
            {
                currentWeapon = "Rocketlauncher";
            }
            if (weaponSlot == 4 && grenadelauncher_available)
            {
                currentWeapon = "Grenadelauncher";
            }
            if (weaponSlot == weaponCount+1)
            {
                weaponSlot = 1;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (currentWeapon == "Pistol")
                spriteBatch.Draw(pistolTex, new Rectangle((int)position.X, (int)position.Y, pistolTex.Width, pistolTex.Height), null, Color.White, rotation, new Vector2(pistolTex.Width / 2, pistolTex.Height / 2), SpriteEffects.None, 0);
            else if (currentWeapon == "Machinegun")
                spriteBatch.Draw(machinegunTex, new Rectangle((int)position.X, (int)position.Y, machinegunTex.Width, machinegunTex.Height), null, Color.White, rotation, new Vector2(machinegunTex.Width / 2, machinegunTex.Height / 2), SpriteEffects.None, 0);
            else if (currentWeapon == "Rocketlauncher")
                spriteBatch.Draw(rocketlauncherTex, new Rectangle((int)position.X, (int)position.Y, rocketlauncherTex.Width, rocketlauncherTex.Height), null, Color.Red, rotation, new Vector2(rocketlauncherTex.Width / 2, rocketlauncherTex.Height / 2), SpriteEffects.None, 0); 
            else if (currentWeapon == "Grenadelauncher")
                spriteBatch.Draw(grenadelauncherTex, new Rectangle((int)position.X, (int)position.Y, grenadelauncherTex.Width, grenadelauncherTex.Height), null, Color.Green, rotation, new Vector2(grenadelauncherTex.Width / 2, grenadelauncherTex.Height / 2), SpriteEffects.None, 0);

            foreach (Bullet bullet in bullets)
            {
                if (bullet != null)
                {
                    bullet.Draw(gameTime, spriteBatch);
                }
            }
        }      
      }
}
