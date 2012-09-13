using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project_Entertainment_Game
{


    public class Menu
    {
        private GamePadState gamepadState;

        private float foo = 0;
        private float fooLimit = 200;

        private KeyboardState keyBoardState;
        private KeyboardState previousKeyBoardState;

        private Audio audio = new Audio();

        //alle enums (het zijn er nogal wat =o )
        public enum menuState
        {
            DEFAULT,
            START,
            OPTIONS,
            CREDITS,
            CHANGEPLATFORM,
            PHOTOS,
            EXIT
        };

        public enum buttonsPressed
        {
            NONE,
            DEFAULTMENUSTATE,
            STARTGAMEPRESSED,
            CREDITSPRESSED,
            OPTIONSPRESSED,
            EXITPRESSED,
            CHANGEPLATFORMPRESSED,
            BACKPRESSED,
        };

        public enum buttonSelect
        {
            BUTTON1,
            BUTTON2,
            BUTTON3,
            BUTTON4,
            BUTTON5,
            BUTTON6,
            BUTTON7
        };

        public enum dPadState
        {
            BUTTON1,
            BUTTON2,
            BUTTON3,
            BUTTON4,
            BUTTON5,
            BUTTON6,
            BUTTON7
        };
        private menuState _menuState = menuState.CREDITS;
        private buttonsPressed _buttonPressed = buttonsPressed.DEFAULTMENUSTATE;
        private buttonSelect _buttonSelect = buttonSelect.BUTTON1;
        private dPadState _dPadState = dPadState.BUTTON1;

        private Texture2D background;

        //de kleuren van de knoppen, knoppenselect en texten
        private Color textColor = new Color(184, 113, 0);
        private Color knopColor = Color.White;
        private Color knopSelectColor = Color.Gray;

        //de geluiden en muziek volume (Waarde van 0 tot 1)
        public float musicVol = 1;
        public float soundVol = 1;
        private String _music = "ON";
        private String _sound = "ON";

        //de SpriteFont
        private SpriteFont menuSpriteFont;

        //Deze Texture is die van de buttons in 'default'-state, wanneer de muis er niet overheen vliegt.
        private Texture2D _knop;
        private Texture2D _knopArrows;

        //deze Texture geeft de maps aan, die geresized zijn, en kunnen worden bekeken in het "change levels menu"
        private Texture2D mapTex;
        private Texture2D photos;

        //de Maps 
        public int maps = 0;
        public int numPlayers = 2;

        //de specificaties van de levels
        private int _frags = 1;
        private int _time = 1;

        private string fragString;
        private string timeString;

        public int fragLimit = 5;
        public int timeLimit = 5;

        public string levelName;
        public string names;
        private int nameInt = 0;

        private string[] levels;
        private string[] _names;

        public Boolean startGame = false;

        //hier worden de X en Y positie van de buttons en de text aangegeven.
        private Vector2 _menuWindowPosition = new Vector2(813, 180);

        private Rectangle _knop1Position = new Rectangle(588, 305, 741, 106);
        private Rectangle _knop2Position = new Rectangle(588, 415, 741, 106);
        private Rectangle _knop3Position = new Rectangle(588, 525, 741, 106);
        private Rectangle _knop4Position = new Rectangle(588, 635, 741, 106);
        private Rectangle _knop5Position = new Rectangle(588, 745, 741, 106);
        private Rectangle _knop6Position = new Rectangle(588, 865, 741, 106);
        private Rectangle _knop7Position = new Rectangle(588, 975, 741, 106);

        private Vector2 _text1Position = new Vector2(693, 316);
        private Vector2 _text2Position = new Vector2(693, 426);
        private Vector2 _text3Position = new Vector2(693, 536);
        private Vector2 _text4Position = new Vector2(693, 646);
        private Vector2 _text5Position = new Vector2(693, 756);
        private Vector2 _text6Position = new Vector2(693, 876);
        private Vector2 _text7Position = new Vector2(693, 986);

        private Rectangle _mapsPosition = new Rectangle(558, 315, 801, 524);

        private Vector2 _hitBox = new Vector2(200, 50);

        public Menu()
        {
            audio.LoadMusic("TitleScreen");

            _menuWindowPosition.X = (int)(_menuWindowPosition.X / 1.5);
            _menuWindowPosition.Y = (int)(_menuWindowPosition.Y / 1.5);

            _knop1Position.X = (int)(_knop1Position.X / 1.5);
            _knop1Position.Y = (int)(_knop1Position.Y / 1.5);
            _knop1Position.Width = (int)(_knop1Position.Width / 1.5);
            _knop1Position.Height = (int)(_knop1Position.Height / 1.5);

            _knop2Position.X = (int)(_knop2Position.X / 1.5);
            _knop2Position.Y = (int)(_knop2Position.Y / 1.5);
            _knop2Position.Width = (int)(_knop2Position.Width / 1.5);
            _knop2Position.Height = (int)(_knop2Position.Height / 1.5);

            _knop3Position.X = (int)(_knop3Position.X / 1.5);
            _knop3Position.Y = (int)(_knop3Position.Y / 1.5);
            _knop3Position.Width = (int)(_knop3Position.Width / 1.5);
            _knop3Position.Height = (int)(_knop3Position.Height / 1.5);

            _knop4Position.X = (int)(_knop4Position.X / 1.5);
            _knop4Position.Y = (int)(_knop4Position.Y / 1.5);
            _knop4Position.Width = (int)(_knop4Position.Width / 1.5);
            _knop4Position.Height = (int)(_knop4Position.Height / 1.5);

            _knop5Position.X = (int)(_knop5Position.X / 1.5);
            _knop5Position.Y = (int)(_knop5Position.Y / 1.5);
            _knop5Position.Width = (int)(_knop5Position.Width / 1.5);
            _knop5Position.Height = (int)(_knop5Position.Height / 1.5);

            _knop6Position.X = (int)(_knop6Position.X / 1.5);
            _knop6Position.Y = (int)(_knop6Position.Y / 1.5);
            _knop6Position.Width = (int)(_knop6Position.Width / 1.5);
            _knop6Position.Height = (int)(_knop6Position.Height / 1.5);

            _knop7Position.X = (int)(_knop7Position.X / 1.5);
            _knop7Position.Y = (int)(_knop7Position.Y / 1.5);
            _knop7Position.Width = (int)(_knop7Position.Width / 1.5);
            _knop7Position.Height = (int)(_knop7Position.Height / 1.5);

            _text1Position.X = (int)(_text1Position.X / 1.5);
            _text1Position.Y = (int)(_text1Position.Y / 1.5);

            _text2Position.X = (int)(_text2Position.X / 1.5);
            _text2Position.Y = (int)(_text2Position.Y / 1.5);

            _text3Position.X = (int)(_text3Position.X / 1.5);
            _text3Position.Y = (int)(_text3Position.Y / 1.5);

            _text4Position.X = (int)(_text4Position.X / 1.5);
            _text4Position.Y = (int)(_text4Position.Y / 1.5);

            _text5Position.X = (int)(_text5Position.X / 1.5);
            _text5Position.Y = (int)(_text5Position.Y / 1.5);

            _text6Position.X = (int)(_text6Position.X / 1.5);
            _text6Position.Y = (int)(_text6Position.Y / 1.5);

            _text7Position.X = (int)(_text7Position.X / 1.5);
            _text7Position.Y = (int)(_text7Position.Y / 1.5);

            _mapsPosition.X = (int)(_mapsPosition.X / 1.5);
            _mapsPosition.Y = (int)(_mapsPosition.Y / 1.5);
            _mapsPosition.Width = (int)(_mapsPosition.Width / 1.5);
            _mapsPosition.Height = (int)(_mapsPosition.Height / 1.5);

            LoadContent();
        }

        public void LoadContent()
        {
            menuSpriteFont = Game1.INSTANCE.Content.Load<SpriteFont>("menuSpriteFont");
            background = Game1.INSTANCE.Content.Load<Texture2D>("Images/Menu/background");
            _knop = Game1.INSTANCE.Content.Load<Texture2D>("Images/Menu/Knoppen/knop");
            _knopArrows = Game1.INSTANCE.Content.Load<Texture2D>("Images/Menu/Knoppen/knopArrow");
        }

        public void StartMusic()
        {
            audio.PlayMusic();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foo += gameTime.ElapsedGameTime.Milliseconds;
            gamepadState = GamePad.GetState(PlayerIndex.One);

            keyBoardState = Keyboard.GetState();
            #region MENUENUM

            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.INSTANCE.GraphicsDevice.Viewport.Width, Game1.INSTANCE.GraphicsDevice.Viewport.Height), Color.White);

            switch (_buttonPressed)
            {
                case buttonsPressed.DEFAULTMENUSTATE:
                    _menuState = menuState.DEFAULT;
                    break;
                case buttonsPressed.OPTIONSPRESSED:
                    _menuState = menuState.OPTIONS;
                    break;
                case buttonsPressed.STARTGAMEPRESSED:
                    _menuState = menuState.START;
                    break;
                case buttonsPressed.CHANGEPLATFORMPRESSED:
                    _menuState = menuState.CHANGEPLATFORM;
                    break;
                case buttonsPressed.CREDITSPRESSED:
                    _menuState = menuState.CREDITS;
                    break;
                case buttonsPressed.EXITPRESSED:
                    _menuState = menuState.EXIT;
                    break;
            }

            switch (_menuState)
            {
                case menuState.START:
                    if (_buttonPressed == buttonsPressed.BACKPRESSED)
                    {
                        _menuState = menuState.DEFAULT;
                    }
                    break;
                case menuState.OPTIONS:
                    if (_buttonPressed == buttonsPressed.BACKPRESSED)
                    {
                        _menuState = menuState.DEFAULT;
                    }
                    break;
                case menuState.CREDITS:
                    if (_buttonPressed == buttonsPressed.BACKPRESSED)
                    {
                        _menuState = menuState.DEFAULT;
                    }
                    break;
                case menuState.CHANGEPLATFORM:
                    if (_buttonPressed == buttonsPressed.BACKPRESSED)
                    {
                        _buttonPressed = buttonsPressed.NONE;
                        _menuState = menuState.START;
                    }
                    break;
                case menuState.EXIT:
                    if (_buttonPressed == buttonsPressed.BACKPRESSED)
                    {
                        _menuState = menuState.DEFAULT;
                    }
                    break;
            }

            switch (_dPadState)
            {
                case dPadState.BUTTON1:
                    _buttonSelect = buttonSelect.BUTTON1;
                    break;
                case dPadState.BUTTON2:
                    _buttonSelect = buttonSelect.BUTTON2;
                    break;
                case dPadState.BUTTON3:
                    _buttonSelect = buttonSelect.BUTTON3;
                    break;
                case dPadState.BUTTON4:
                    _buttonSelect = buttonSelect.BUTTON4;
                    break;
                case dPadState.BUTTON5:
                    _buttonSelect = buttonSelect.BUTTON5;
                    break;
                case dPadState.BUTTON6:
                    _buttonSelect = buttonSelect.BUTTON6;
                    break;
                case dPadState.BUTTON7:
                    _buttonSelect = buttonSelect.BUTTON7;
                    break;
            }

            #endregion
            #region DEFAULT
            if (_menuState == menuState.DEFAULT)
            {
                spriteBatch.Draw(_knop, _knop1Position, knopColor);
                spriteBatch.Draw(_knop, _knop2Position, knopColor);
                spriteBatch.Draw(_knop, _knop3Position, knopColor);
                spriteBatch.Draw(_knop, _knop4Position, knopColor);


                if (_buttonSelect == buttonSelect.BUTTON1)
                {
                    spriteBatch.Draw(_knop, _knop1Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.STARTGAMEPRESSED;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.STARTGAMEPRESSED;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON2)
                {
                    spriteBatch.Draw(_knop, _knop2Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.OPTIONSPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.OPTIONSPRESSED;
                            _dPadState = dPadState.BUTTON1;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON3)
                {
                    spriteBatch.Draw(_knop, _knop3Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.CREDITSPRESSED;
                            _dPadState = dPadState.BUTTON6;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.CREDITSPRESSED;
                            _dPadState = dPadState.BUTTON6;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON4)
                {
                    spriteBatch.Draw(_knop, _knop4Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.EXITPRESSED;
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.EXITPRESSED;
                            _dPadState = dPadState.BUTTON2;
                        }
                    }
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.EXITPRESSED;
                        _dPadState = dPadState.BUTTON2;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, "Start New Game", _text1Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Options", _text2Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "About us", _text3Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Exit", _text4Position, textColor);
            }
            #endregion
            #region STARTGAME
            else if (_menuState == menuState.START)
            {
                spriteBatch.Draw(_knop, _knop1Position, knopColor);
                spriteBatch.Draw(_knopArrows, _knop2Position, knopColor);
                spriteBatch.Draw(_knopArrows, _knop3Position, knopColor);
                spriteBatch.Draw(_knopArrows, _knop4Position, knopColor);
                spriteBatch.Draw(_knop, _knop5Position, knopColor);

                fragString = fragLimit.ToString();
                timeString = timeLimit.ToString();

                if (fragLimit >= 0 && fragLimit < 10) { _frags = 1; } else if (fragLimit >= 10 && fragLimit < 30) { _frags = 5; } else if (fragLimit >= 30) { _frags = 10; }
                if (fragLimit > 100) { fragLimit = 0; } else if (fragLimit < 0) { fragLimit = 100; }
                if (timeLimit >= 0 && timeLimit < 10) { _time = 1; } if (timeLimit >= 10 && timeLimit < 30) { _time = 5; } else if (timeLimit >= 30) { _time = 10; }
                if (timeLimit > 100) { timeLimit = 0; } else if (timeLimit < 0) { timeLimit = 100; }
                if (fragLimit == 0) { fragString = "Infinite"; } if (timeLimit == 0) { timeString = "Infinite"; }

                if (_buttonSelect == buttonSelect.BUTTON1)
                {
                    spriteBatch.Draw(_knop, _knop1Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.CHANGEPLATFORMPRESSED;
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.CHANGEPLATFORMPRESSED;
                            _dPadState = dPadState.BUTTON5;
                        }
                    }
                }
                else if (_buttonSelect == buttonSelect.BUTTON2)
                {
                    spriteBatch.Draw(_knopArrows, _knop2Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            if (numPlayers > 2)
                            {
                                numPlayers--;
                            }
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            if (numPlayers < 4)
                            {
                                numPlayers++;
                            }
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            if (numPlayers > 2)
                                numPlayers--;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            if (numPlayers < 4)
                                numPlayers++;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON3)
                {
                    spriteBatch.Draw(_knopArrows, _knop3Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            if (fragLimit == 30) { _frags = 5; } else if (fragLimit == 10) { _frags = 1; }
                            fragLimit -= _frags;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            fragLimit += _frags;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            fragLimit -= _frags;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            fragLimit += _frags;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON4)
                {
                    spriteBatch.Draw(_knopArrows, _knop4Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            if (timeLimit == 30) { _time = 5; } else if (timeLimit == 10) { _time = 1; }
                            timeLimit -= _time;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            timeLimit += _time;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            timeLimit -= _time;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            timeLimit += _time;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON5)
                {
                    spriteBatch.Draw(_knop, _knop5Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                        }
                    }
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.BACKPRESSED;
                        _dPadState = dPadState.BUTTON1;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, "Start new Game", _menuWindowPosition, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Start Game", _text1Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Players : " + numPlayers, _text2Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Frag Limit: " + fragString, _text3Position, textColor);
                if (timeLimit != 0) { spriteBatch.DrawString(menuSpriteFont, "Time Limit: " + timeString + " mins.", _text4Position, textColor); }
                else { spriteBatch.DrawString(menuSpriteFont, "Time Limit: " + timeString, _text4Position, textColor); }
                spriteBatch.DrawString(menuSpriteFont, "Back", _text5Position, textColor);
            }
            #endregion
            #region CHANGEMAP
            else if (_menuState == menuState.CHANGEPLATFORM)
            {
                spriteBatch.DrawString(menuSpriteFont, "Change Map", _menuWindowPosition, textColor);

                levels = Directory.GetFiles("Maps");

                if (maps > levels.Length - 1)
                {
                    maps = 0;
                }
                else if (maps < 0)
                {
                    maps = levels.Length - 1;
                }

                string mapName = levels[maps];
                levelName = mapName;

                int first = mapName.IndexOf("\\");
                mapName = mapName.Remove(0, first + 1);
                int last = mapName.IndexOf(".");
                mapName = mapName.Remove(last, 4);

                //Create an filestream.
                FileStream fstream = new FileStream(levelName, FileMode.Open, FileAccess.Read);
                byte[] filecontent = new byte[fstream.Length];

                //Create an array to put our data in.
                string[][] leveldata = new string[filecontent.Length][];

                //Read the file and put data in the filecontent.
                for (int i = 0; i < fstream.Length; i++)
                {
                    fstream.Read(filecontent, i, 1);
                }

                //Split up the file in "words".
                int currentAt = 0;
                int currentAt2 = 0;
                for (int i = 0; i < filecontent.Length; i++)
                {

                    if (leveldata[currentAt] == null)
                    {
                        leveldata[currentAt] = new string[255];
                    }
                    if ((char)filecontent[i] == ' ')
                    {
                        currentAt2++;
                    }
                    else if ((char)filecontent[i] == (char)10)
                    {
                        currentAt2 = 0;
                        currentAt++;
                    }
                    else
                    {
                        leveldata[currentAt][currentAt2] += (char)filecontent[i];
                    }
                }

                //Go through every data in leveldata.
                for (int i = 0; i < leveldata.Length; ++i)
                {
                    if (leveldata[i] != null)
                    {
                        if (leveldata[i][0].Contains("background:"))
                        {
                            mapTex = Game1.INSTANCE.Content.Load<Texture2D>("Images/Maps/" + leveldata[i][1]);
                        }
                    }
                }

                spriteBatch.Draw(mapTex, _mapsPosition, Color.White);
                spriteBatch.Draw(_knopArrows, _knop5Position, knopColor);
                spriteBatch.Draw(_knop, _knop6Position, knopColor);
                spriteBatch.Draw(_knop, _knop7Position, knopColor);

                if (_buttonSelect == buttonSelect.BUTTON5)
                {
                    spriteBatch.Draw(_knopArrows, _knop5Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON6;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON7;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            maps--;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            maps++;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON6;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON7;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            maps--;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            maps++;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON6)
                {
                    spriteBatch.Draw(_knop, _knop6Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON7;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            audio.StopMusic();
                            startGame = true;
                            foo = 0;
                        }

                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON7;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            audio.StopMusic();
                            startGame = true;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON7)
                {
                    spriteBatch.Draw(_knop, _knop7Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON6;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON6;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.BACKPRESSED;
                        _dPadState = dPadState.BUTTON1;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, mapName, _text5Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Start Game", _text6Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Back", _text7Position, textColor);
            }
            #endregion
            #region OPTIONS
            else if (_menuState == menuState.OPTIONS)
            {
                spriteBatch.DrawString(menuSpriteFont, "Options", _menuWindowPosition, textColor);

                spriteBatch.Draw(_knop, _knop1Position, Color.White);
                spriteBatch.Draw(_knop, _knop2Position, Color.White);
                spriteBatch.Draw(_knopArrows, _knop3Position, Color.White);
                spriteBatch.Draw(_knopArrows, _knop4Position, Color.White);
                spriteBatch.Draw(_knop, _knop5Position, Color.White);

                if (musicVol <= 0) { _music = "OFF"; } else { _music = "ON"; }
                if (soundVol <= 0) { _sound = "OFF"; } else { _sound = "ON"; }

                if (_dPadState == dPadState.BUTTON1)
                {
                    spriteBatch.Draw(_knop, _knop1Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            if (_music == "ON") { _music = "OFF"; musicVol = 0; }
                            else { _music = "ON"; musicVol = 1; }
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            if (_music == "ON") { _music = "OFF"; musicVol = 0; }
                            else { _music = "ON"; musicVol = 1; }
                        }
                    }
                }

                else if (_dPadState == dPadState.BUTTON2)
                {
                    spriteBatch.Draw(_knop, _knop2Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            if (_sound == "ON") { _sound = "OFF"; soundVol = 0; }
                            else { _sound = "ON"; soundVol = 1; }
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            if (_sound == "ON") { _sound = "OFF"; soundVol = 0; }
                            else { _sound = "ON"; soundVol = 1; }
                        }
                    }
                }

                else if (_dPadState == dPadState.BUTTON3)
                {
                    spriteBatch.Draw(_knopArrows, _knop3Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            if (musicVol > 0) { musicVol -= 0.1f; }
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            if (musicVol < 1) { musicVol += 0.1f; }
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            if (musicVol > 0) { musicVol -= 0.1f; }
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            if (musicVol < 1) { musicVol += 0.1f; }
                        }
                    }
                }
                else if (_dPadState == dPadState.BUTTON4)
                {
                    spriteBatch.Draw(_knopArrows, _knop4Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON5;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON3;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            if (soundVol > 0) { soundVol -= 0.1f; }
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            if (soundVol < 1) { soundVol += 0.1f; }
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON5;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON3;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            if (soundVol > 0) { soundVol -= 0.1f; }
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            if (soundVol < 1) { soundVol += 0.1f; }
                        }
                    }
                }
                else if (_dPadState == dPadState.BUTTON5)
                {
                    spriteBatch.Draw(_knop, _knop5Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON4;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON4;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON2;
                        }
                    }
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.BACKPRESSED;
                        _dPadState = dPadState.BUTTON2;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, "Music: " + _music, _text1Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Sound: " + _sound, _text2Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Musicvolume: " + (int)(musicVol * 100) + "%", _text3Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Soundvolume: " + (int)(soundVol * 100) + "%", _text4Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Back", _text5Position, textColor);
                audio.setMusicVolume(musicVol);
            }
            #endregion
            #region CREDITS
            else if (_menuState == menuState.CREDITS)
            {
                spriteBatch.DrawString(menuSpriteFont, "Credits", _menuWindowPosition, textColor);

                _names = Directory.GetFiles("Content\\Images\\Photos");

                if (nameInt > _names.Length - 1)
                {
                    nameInt = 0;
                }
                else if (nameInt < 0)
                {
                    nameInt = _names.Length - 1;
                }

                string creditName = _names[nameInt];
                names = creditName;

                int first = creditName.IndexOf("Images\\Photos\\");
                creditName = creditName.Remove(0, first + 14);
                int last = creditName.IndexOf(".");
                creditName = creditName.Remove(last, 4);

                for (int i = 0; i < names.Length; i++)
                {
                    photos = Game1.INSTANCE.Content.Load<Texture2D>("Images\\Photos\\" + creditName);
                }

                spriteBatch.Draw(photos, _mapsPosition, Color.White);
                spriteBatch.Draw(_knopArrows, _knop6Position, knopColor);
                spriteBatch.Draw(_knop, _knop7Position, knopColor);

                if (_buttonSelect == buttonSelect.BUTTON6)
                {
                    spriteBatch.Draw(_knopArrows, _knop6Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON7;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON7;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Left == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X <= -0.1)
                        {
                            nameInt--;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Right == ButtonState.Pressed || gamepadState.ThumbSticks.Left.X >= 0.1)
                        {
                            nameInt++;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON7;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON7;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Left))
                        {
                            nameInt--;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Right))
                        {
                            nameInt++;
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON7)
                {
                    spriteBatch.Draw(_knop, _knop7Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON6;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON6;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON6;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON6;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                    }
                }

                string taak = "Blaat";
                Vector2 textOffset = new Vector2();
                switch (creditName)
                {
                    case "Daan":
                        taak = "Designer";
                        break;
                    case "Erwin":
                        taak = "Project Leader";
                        break;
                    case "Gerjo":
                        taak = "Programmer&Graphic Design";
                        textOffset = new Vector2(90, 0);
                        break;
                    case "Jur":
                        taak = "Programmer";
                        break;
                    case "Robby":
                        taak = "Programmer";
                        break;
                    case "Sander":
                        taak = "Lead Programmer";
                        break;
                    case "Stephan":
                        taak = "Designer & Audio Design";
                        textOffset = new Vector2(40, 0);
                        break;
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.BACKPRESSED;
                        _dPadState = dPadState.BUTTON1;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, taak, _text1Position - textOffset, textColor);
                spriteBatch.DrawString(menuSpriteFont, creditName, _text6Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Back", _text7Position, textColor);
            }
            #endregion
            #region EXITGAME
            else if (_menuState == menuState.EXIT)
            {
                spriteBatch.Draw(_knop, _knop1Position, knopColor);
                spriteBatch.Draw(_knop, _knop2Position, knopColor);


                if (_buttonSelect == buttonSelect.BUTTON1)
                {
                    spriteBatch.Draw(_knop, _knop1Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            Game1.INSTANCE.Exit();
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON2;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            Game1.INSTANCE.Exit();
                        }
                    }
                }

                else if (_buttonSelect == buttonSelect.BUTTON2)
                {
                    spriteBatch.Draw(_knop, _knop2Position, knopSelectColor);
                    if (foo >= fooLimit)
                    {
                        if (gamepadState.DPad.Down == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y <= -0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.DPad.Up == ButtonState.Pressed || gamepadState.ThumbSticks.Left.Y >= 0.1)
                        {
                            _dPadState = dPadState.BUTTON1;
                            foo = 0;
                        }
                        else if (gamepadState.Buttons.A == ButtonState.Pressed)
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON2;
                            foo = 0;
                        }
                    }
                    if (previousKeyBoardState != keyBoardState)
                    {
                        if (keyBoardState.IsKeyDown(Keys.Up))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Down))
                        {
                            _dPadState = dPadState.BUTTON1;
                        }
                        else if (keyBoardState.IsKeyDown(Keys.Enter))
                        {
                            _buttonPressed = buttonsPressed.BACKPRESSED;
                            _dPadState = dPadState.BUTTON2;
                        }
                    }
                }

                if (foo >= fooLimit)
                {
                    if (gamepadState.Buttons.B == ButtonState.Pressed)
                    {
                        _buttonPressed = buttonsPressed.BACKPRESSED;
                        _dPadState = dPadState.BUTTON4;
                        foo = 0;
                    }
                }

                spriteBatch.DrawString(menuSpriteFont, "Exit. Are you sure?", _menuWindowPosition, textColor);
                spriteBatch.DrawString(menuSpriteFont, "Yes", _text1Position, textColor);
                spriteBatch.DrawString(menuSpriteFont, "NO", _text2Position, textColor);
            }
            #endregion
            previousKeyBoardState = keyBoardState;
        }
    }
}