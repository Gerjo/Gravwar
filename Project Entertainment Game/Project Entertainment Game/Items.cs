using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Entertainment_Game
{
    public class Items
    {
        public Rectangle position;
        public int spawnTime;
        private Texture2D texture;
        public string type;

        public Items(int x, int y, string texture)
        {
            this.position = new Rectangle(x,y,32,32);
            this.texture = Game1.INSTANCE.Content.Load<Texture2D>("Images/" + texture);
            if (texture.ToLower() == "items/rocketlauncher")
            {
                type = "rocketlauncher";
            }
            else if (texture.ToLower() == "items/grenadelauncher")
            {
                type = "grenadelauncher";
            }
            else if (texture.ToLower() == "items/machinegun")
            {
                type = "machinegun";
            }
            else if (texture.ToLower() == "items/gw")
            {
                type = "gw";
            }
            ItemTaken();
        }

        public Weapons.AbstractWeapon getPickUpWeapon()
        {

            //TODO: implement the correct weapons here for this pickup.
            switch (type)
            {
                case "machinegun":
                    return new Weapons.MachineGun();

                case "pistol":
                    return new Weapons.Pistol();   
   
                case "rocketlauncher":
                    return new Weapons.RocketLauncher();

                case "grenadelauncher":
                    return new Weapons.BouncyGun();
                    //return new Weapons.GrenadeLauncher();
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            if (spawnTime != 0)
                spawnTime--;
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (spawnTime == 0)
                spriteBatch.Draw(texture, position, Color.White);
            else if (spawnTime > 0)
                spriteBatch.Draw(texture, position, new Color(0,0,0,0));
        }

        public void ItemTaken()
        {
            if (spawnTime == 0)
            {
                if (type == "gw")
                {
                    spawnTime = 90 * 60;
                }
                else
                {
                    spawnTime = 5 * 60;
                }
            }
        }
    }
}
