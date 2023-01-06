//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace BasicGameScreens
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            this.CurrentKeyboardStates = new KeyboardState[MaxInputs];
            this.CurrentGamePadStates = new GamePadState[MaxInputs];

            this.LastKeyboardStates = new KeyboardState[MaxInputs];
            this.LastGamePadStates = new GamePadState[MaxInputs];

            this.GamePadWasConnected = new bool[MaxInputs];
        }


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (var i = 0; i < MaxInputs; i++)
            {
                this.LastKeyboardStates[i] = this.CurrentKeyboardStates[i];
                this.LastGamePadStates[i] = this.CurrentGamePadStates[i];

                this.CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                this.CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (this.CurrentGamePadStates[i].IsConnected)
                {
                    this.GamePadWasConnected[i] = true;
                }
            }

            this.TouchState = TouchPanel.GetState();

            this.Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                this.Gestures.Add(TouchPanel.ReadGesture());
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                var i = (int)playerIndex;

                return (this.CurrentKeyboardStates[i].IsKeyDown(key) && this.LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (this.IsNewKeyPress(key, PlayerIndex.One, out playerIndex) || this.IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        this.IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) || this.IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                var i = (int)playerIndex;

                return (this.CurrentGamePadStates[i].IsButtonDown(button) && this.LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (this.IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   this.IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return this.IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return this.IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }
    }
}