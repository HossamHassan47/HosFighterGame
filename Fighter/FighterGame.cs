using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Fighter
{
    public class FighterGame : Game
    {
        #region Definitions

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static GameObjects GameObjects;

        // Text to display to user
        string gameOverText = "Game Over";
        string tapToStartText = "Tap to Start Fighting";
        string scoreText = "Score : {0}";

        ScrollingBackground scrollingBackground;

        // Timers: Calculate when events should occur in our game
        TimeSpan gameTimer = TimeSpan.FromMilliseconds(0);

        // Define the delay between game ending and new game beginning
        TimeSpan tapToRestartTimer = TimeSpan.FromSeconds(2);

        // Define how often the level difficulty increases
        TimeSpan increaseLevelTimer = TimeSpan.FromMilliseconds(0);
        
        public static float EnemyFighterSpeed = 1f;
        private int levelTotalSeconds = 60;

        #endregion


        public FighterGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            //graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            TouchPanel.EnabledGestures = GestureType.Flick | GestureType.HorizontalDrag | GestureType.VerticalDrag | GestureType.Tap;

            base.Initialize();
        }

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            GameObjects = new GameObjects();

            // Background
            this.scrollingBackground = new ScrollingBackground();

            this.scrollingBackground.Load(GraphicsDevice, Content.Load<Texture2D>("background"));

            // Font
            GameObjects.GameFont = Content.Load<SpriteFont>("font");

            // Sound Effects
            GameObjects.FireSoundEffect = Content.Load<SoundEffect>("fire");
            GameObjects.ExplosionSoundEffect = Content.Load<SoundEffect>("explosion");
            GameObjects.CollisionSoundEffect = Content.Load<SoundEffect>("collision");
            GameObjects.HigherLevelSoundEffect = Content.Load<SoundEffect>("higher_level");

            // Background music
            GameObjects.BackgroundMusic = Content.Load<Song>("army_strong");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(GameObjects.BackgroundMusic);

            // Background music
            GameObjects.AirplanSoundEffect = Content.Load<SoundEffect>("airplane");
            
            // Player fighter
            var gameBoundaries = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

            GameObjects.PlayerFighter = new Fighter(Content.Load<Texture2D>("fighter"), gameBoundaries, PlayerType.Human, 0);

            // Player rockets
            var playerRockets = new List<Rocket>();
            var rocketTexture = Content.Load<Texture2D>("rocket");
            for (int i = 1; i <= 5; i++)
            {
                var rocket = new Rocket(rocketTexture, gameBoundaries, i, PlayerType.Human, GameObjects.PlayerFighter);

                playerRockets.Add(rocket);
            }

            GameObjects.PlayerFighter.Rockets = playerRockets;
            GameObjects.PlayerFighter.CurrentRocketNumber = 1;

            // Enemy fighters
            GameObjects.EnemyFighters = new List<Fighter>();
            var enemyRocketTexture = Content.Load<Texture2D>("enemy_rocket");

            for (int i = 1; i <= 18; i++)
            {
                var enemyFighter = this.GetEnemyFighter(i);

                var enemyRocket = new Rocket(enemyRocketTexture, gameBoundaries, i, PlayerType.Computer, enemyFighter);

                enemyFighter.Rockets.Add(enemyRocket);

                GameObjects.EnemyFighters.Add(enemyFighter);
                Thread.Sleep(1);
            }

            // game objects

            GameObjects.PlayerScore = new PlayerScore(Content.Load<SpriteFont>("score_font"), gameBoundaries);
            GameObjects.GameState = GameState.Start;
        }

        private Fighter GetEnemyFighter(int fighterNumber)
        {
            // enemy fighter
            var gameBoundaries = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            var enemyFighterTexture = Content.Load<Texture2D>("enemy_fighter");

            var fighter = new Fighter(enemyFighterTexture, gameBoundaries, PlayerType.Computer, fighterNumber);

            fighter.EnemyFlightSpeed = EnemyFighterSpeed;
            
            return fighter;
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // background
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            scrollingBackground.Update(elapsed * 50);

            var touchState = TouchPanel.GetState();

            switch (GameObjects.GameState)
            {
                case GameState.Start:
                    if (touchState.Count > 0)
                    {
                        GameObjects.GameState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    // Play the game
                    this.PlayGame(gameTime, touchState);
                    break;
                case GameState.GameOver:
                    tapToRestartTimer -= gameTime.ElapsedGameTime;
                    if (touchState.Count > 0 && tapToRestartTimer.TotalMilliseconds < 0)
                    {
                        this.RestartGame();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private void RestartGame()
        {
            GameObjects.GameState = GameState.Start;
            GameObjects.PlayerScore.Reset();
            
            EnemyFighterSpeed = 1f;

            this.ResetEnemyFighters();

            this.increaseLevelTimer = TimeSpan.FromMilliseconds(0);
            this.gameTimer = TimeSpan.FromMilliseconds(0);
        }

        private void ResetEnemyFighters()
        {
            var locationY = -200f;

            foreach(Fighter f in GameObjects.EnemyFighters)
            {
                // Reset flight speed
                f.EnemyFlightSpeed = EnemyFighterSpeed;

                // Reset flight location
                f.Location.Y = locationY;

                locationY = locationY - 200f;

                foreach(Rocket r in f.Rockets)
                {
                    r.Reset();
                }
            }
        }

        private void PlayGame(GameTime gameTime, TouchCollection touchState)
        {
            GameObjects.TouchInput = new TouchInput();

            this.GetTouchInput();

            // Update player fighter
            GameObjects.PlayerFighter.Update(gameTime);

            // Update player fighter rockets
            foreach (Rocket rocket in GameObjects.PlayerFighter.Rockets)
            {
                rocket.Update(gameTime);
            }

            // Update enemy fighters
            foreach (Fighter enemyFighter in GameObjects.EnemyFighters)
            {
                // Update enemy fighter
                enemyFighter.Update(gameTime);

                // Update enemy fighter rockets
                foreach (Rocket rocket in enemyFighter.Rockets)
                {
                    rocket.Update(gameTime);
                }
            }

            //score.Update(gameTime, gameObjects);

            this.CheckForGameOver(gameTime);
            
            this.IncreaseLevel(gameTime);
        }

        private void CheckForGameOver(GameTime gameTime)
        {
            if (GameObjects.PlayerFighter.IsGameOver)
            {
                GameObjects.GameState = GameState.GameOver;

                this.tapToRestartTimer = TimeSpan.FromSeconds(2);
                
                GameObjects.PlayerFighter.Reset();
            }
        }

        private void IncreaseLevel(GameTime gameTime)
        {
            this.increaseLevelTimer += gameTime.ElapsedGameTime;

            if (this.increaseLevelTimer.TotalSeconds <= levelTotalSeconds)
            {
                return;
            }

            GameObjects.HigherLevelSoundEffect.Play();
            GameObjects.PlayerScore.IncreaseLevel();

            this.increaseLevelTimer = TimeSpan.FromMilliseconds(0);

            // Increase enemy speed
            EnemyFighterSpeed = EnemyFighterSpeed + 1f;

            foreach (Fighter enemyFighter in GameObjects.EnemyFighters)
            {
                // Update enemy fighter
                enemyFighter.EnemyFlightSpeed = EnemyFighterSpeed;
            }

        }
        
        private void GetTouchInput()
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();

                GameObjects.TouchInput.Position = gesture.Position;

                if (gesture.GestureType == GestureType.Tap)
                {
                    // Set current rocket number to fire
                    if (GameObjects.PlayerFighter.CurrentRocketNumber > 5)
                    {
                        GameObjects.PlayerFighter.CurrentRocketNumber = 1;
                    }
                    else
                    {
                        GameObjects.PlayerFighter.CurrentRocketNumber++;
                    }

                    GameObjects.TouchInput.Tapped = true;
                }

                if (gesture.GestureType == GestureType.HorizontalDrag)
                {
                    // Set current rocket number to fire
                    if (GameObjects.PlayerFighter.CurrentRocketNumber > 5)
                    {
                        GameObjects.PlayerFighter.CurrentRocketNumber = 1;
                    }
                    else
                    {
                        GameObjects.PlayerFighter.CurrentRocketNumber++;
                    }

                    GameObjects.TouchInput.Hold = true;
                }
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            var center = graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            // Draw the background
            this.scrollingBackground.Draw(spriteBatch);
            
            if (GameObjects.GameState == GameState.Playing)
            {
                // Player rockets                
                foreach (Rocket rocket in GameObjects.PlayerFighter.Rockets)
                {
                    rocket.Draw(spriteBatch);
                }

                // Draw enemy fighters
                foreach (Fighter enemyFighter in GameObjects.EnemyFighters)
                {
                    foreach (Rocket r in enemyFighter.Rockets)
                    {
                        r.Draw(spriteBatch);
                    }

                    enemyFighter.Draw(spriteBatch);
                }

                // Player score
                GameObjects.PlayerScore.Draw(spriteBatch);
            }
            else if (GameObjects.GameState == GameState.GameOver)
            {
                // If the game is over, draw the score and game over text in the center of screen.

                // Measure the text so we can center it correctly
                var v = new Vector2(GameObjects.GameFont.MeasureString(gameOverText).X / 2, 0);
                spriteBatch.DrawString(GameObjects.GameFont, gameOverText, center - v, Color.Red);

                var t = string.Format(scoreText, GameObjects.PlayerScore.Score);

                // Measure the text so we can center it correctly
                v = new Vector2(GameObjects.GameFont.MeasureString(t).X / 2, 0);

                // We can use the font.LineSpacing to draw on the line underneath the "Game Over" text
                spriteBatch.DrawString(GameObjects.GameFont, t, center + new Vector2(-v.X, GameObjects.GameFont.LineSpacing), Color.White);
            }
            else if (GameObjects.GameState == GameState.Start)
            {
                // If the game is starting over, add "Tap to Start" text
                // Measure the text so we can center it correctly
                var v = new Vector2(GameObjects.GameFont.MeasureString(tapToStartText).X / 2, 0);
                spriteBatch.DrawString(GameObjects.GameFont, tapToStartText, center - v, Color.White);
            }

            // Draw player fighter
            GameObjects.PlayerFighter.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
