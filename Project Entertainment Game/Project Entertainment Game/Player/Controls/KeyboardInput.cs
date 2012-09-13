using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Project_Entertainment_Game
{
    public class KeyboardInput : GamePadInput
    {
        protected KeyboardState prevKeyboardState;  // Used to determine whether a key is held, or not.

        public KeyboardInput(AnimPlayer ownerPlayer) : base(ownerPlayer) { }

        public override bool isLeftWalk()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) return true;
            return base.isLeftWalk();
        }
        public override bool isRightWalk()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D)) return true;
            return base.isRightWalk();
        }
        public override bool isUpWalk()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W)) return true;
            return base.isUpWalk();
        }
        public override bool isDownWalk()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.S)) return true;
            return base.isDownWalk();

        }

        public override bool isSwitchGravLeft()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) return true;
            return base.isSwitchGravLeft();
        }
        public override bool isSwitchGravRight()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) return true;
            return base.isSwitchGravRight();
        }
        public override bool isSwitchGravUp()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) return true;
            return base.isSwitchGravUp();
        }
        public override bool isSwitchGravDown()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) return true;
            return base.isSwitchGravDown();
        }

        public override bool isFireBullet()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) return true;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) return true;

            return base.isFireBullet();
        }
        public override bool isJumpButton()
        {
            KeyboardState currentKeyBoardState = Keyboard.GetState();

            if (prevKeyboardState != currentKeyBoardState)
            {
                if (currentKeyBoardState.IsKeyDown(Keys.Space))
                {
                    prevKeyboardState = currentKeyBoardState;
                    return true;
                }
            }

            prevKeyboardState = currentKeyBoardState;
            return base.isJumpButton();
        }

        public override Vector2 getAimDirectionNormalized()
        {
            // Read the mouse location: (this is the shooting direction)
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            // Calculate the direction. Vector AB = B - A
            Vector2 direction = Vector2.Subtract(mousePosition, ownerPlayer.position);

            // For those who still have to do SimTec; we calculate the normalized vector as following:
            // length = sqrt(direction.x*direction.x + direction.y*direction.y); -- length of this vector according to the pythagorean theorem
            // direction.x = direction.x / length; -- devide the x axis by the length.
            // direction.y = direction.y / length; -- devide the y axis by the length.
            // The direction vector now has a length of 1.

            // Or simply use the build in function given to us by XNA:
            direction.Normalize();

            return direction;
        }
        public override float getAimDirectionAngle()
        {
            Vector2 normalizedDirection = getAimDirectionNormalized();

            // NOTICE: The Y,X sequence of arguments is intentional. (rather than X,Y which is expected)
            return (float)Math.Atan2(normalizedDirection.Y, normalizedDirection.X); // - 1.57 = 90 degree extra rotation.
        }

        public override bool isStartPause()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P)) return true;
            return base.isStartPause();
        }
        public override bool isStopPause()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.O)) return true;
            return base.isStopPause();
        }
        public override bool isAbutton()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) return true;
            return base.isAbutton();
        }
        public override bool isBbutton()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.B)) return true;
            return base.isBbutton();
        }

        public override bool isGobackToMenu()
        {
            return isBbutton();
        }
    }
}
