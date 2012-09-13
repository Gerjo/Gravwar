using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Project_Entertainment_Game
{
    public class GamePadInput
    {
        protected AnimPlayer    ownerPlayer; // Reference to the player which owns this controller. Leave as-is
        protected GamePadState  prevGamePadState;    // Used to determine whether a button is held, or not.

        // Settings, edit these freely
        protected float walkTreshold  = 0.1f; // How far should the joystick be moved before the player starts walking.
        protected float shootTreshold = 0.1f; // how far should the joystick be moved before a bullet is shot.

        public GamePadInput(AnimPlayer ownerPlayer)
        {
            this.ownerPlayer = ownerPlayer;
        }

        public virtual bool isLeftWalk()
        {
            return (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Left.X < walkTreshold*-1);
        }
        public virtual bool isRightWalk()
        {
            return (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Left.X > walkTreshold);
        }
        public virtual bool isUpWalk()
        {
            return (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Left.Y > walkTreshold);
        }
        public virtual bool isDownWalk()
        {
            return (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Left.Y < walkTreshold * -1);
        }

        public virtual bool isSwitchGravLeft()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).DPad.Left == ButtonState.Pressed;
        }
        public virtual bool isSwitchGravRight()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).DPad.Right == ButtonState.Pressed;
        }
        public virtual bool isSwitchGravUp()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).DPad.Up == ButtonState.Pressed;
        }
        public virtual bool isSwitchGravDown()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).DPad.Down == ButtonState.Pressed;
        }

        public virtual bool isFireBullet()
        {
            return
                (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.X < shootTreshold * -1) ||
                (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.X > shootTreshold) ||
                (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.Y < shootTreshold * -1) ||
                (GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.Y > shootTreshold);

        }        
        public virtual bool isJumpButton()
        {
            GamePadState currentGamePadState = GamePad.GetState(ownerPlayer.playerIndex);

            if (prevGamePadState.Buttons.A != currentGamePadState.Buttons.A)
            {
                if (currentGamePadState.Buttons.A == ButtonState.Pressed)
                {
                    prevGamePadState = currentGamePadState;
                    return true;
                }
            }

            if (currentGamePadState.Buttons.RightShoulder != prevGamePadState.Buttons.RightShoulder)
            {
                if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    prevGamePadState = currentGamePadState;
                    return true;
                }
            }

            prevGamePadState = currentGamePadState;
            return false;
        }

        public virtual Vector2 getAimDirectionNormalized()
        {
            Vector2 aimDirection = new Vector2(GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.X, GamePad.GetState(ownerPlayer.playerIndex).ThumbSticks.Right.Y*-1);
            aimDirection.Normalize();
            return aimDirection;
        }
        public virtual float getAimDirectionAngle()
        {
            Vector2 normalizedDirection = getAimDirectionNormalized();

            float angle = (float)Math.Atan2(normalizedDirection.Y, normalizedDirection.X);

            // Set the default angle if we're not pointing at anything.
            if (float.IsNaN(angle)) angle = ownerPlayer.getBaseRotation();

            return angle;
        }

        public virtual bool isStartPause()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).IsButtonDown(Buttons.Start);
        }
        public virtual bool isStopPause()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).IsButtonDown(Buttons.A);
        }
        public virtual bool isGobackToMenu()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).IsButtonDown(Buttons.B);
        }

        public virtual bool isBbutton()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).IsButtonDown(Buttons.B);
        }
        public virtual bool isAbutton()
        {
            return GamePad.GetState(ownerPlayer.playerIndex).IsButtonDown(Buttons.A);
        }
    }
}
