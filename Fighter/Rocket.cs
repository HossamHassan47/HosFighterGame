using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fighter
{
    public class Rocket
    {
        #region Definitions

        public Texture2D Texture { get; set; }

        public Vector2 Location;

        public Vector2 Velocity { get; set; }

        public Rectangle GameBoundaries { get; set; }

        public int Width { get { return this.Texture.Width; } }

        public int Height { get { return this.Texture.Height; } }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)this.Location.X, (int)this.Location.Y, this.Width, this.Height);
            }
        }
        
        public PlayerType PlayerType { get; set; }

        public int RocketNumber { get; set; }

        private float playerRocketSpeed = -25f;
        private int enemyRocketSpeed = 5;
        private Fighter attachedToFighter;
        private bool isFired = false;
        private TimeSpan fireTimer = TimeSpan.FromMilliseconds(0);

        #endregion

        #region CTOR

        public Rocket(Texture2D texture, Rectangle gameBoundaries, int rocketNumber, PlayerType playerType, Fighter fighter)
        {
            this.attachedToFighter = fighter;

            this.Texture = texture;

            this.GameBoundaries = gameBoundaries;

            this.RocketNumber = rocketNumber;

            this.PlayerType = playerType;
            
            this.Velocity = Vector2.Zero;

            // Set location
            this.SetLocation();
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Location, Color.White);
        }
        
        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (this.PlayerType == PlayerType.Human)
            {
                this.UpdateHumanRocket(gameTime);
            }
            else
            {
                this.UpdateComputerRocket(gameTime);
            }
        }

        TimeSpan humanFireTimer = TimeSpan.FromSeconds(0);
        private void UpdateHumanRocket(GameTime gameTime)
        {
            if (FighterGame.GameObjects.TouchInput.Tapped
                && !this.isFired
                && this.RocketNumber == FighterGame.GameObjects.PlayerFighter.CurrentRocketNumber)
            {
                // Fire
                this.Fire();
            }

            //if (FighterGame.GameObjects.TouchInput.Hold
            //    && !this.isFired
            //    && this.RocketNumber == FighterGame.GameObjects.PlayerFighter.CurrentRocketNumber)
            //{
            //    this.humanFireTimer += gameTime.ElapsedGameTime;
                
            //    if (this.humanFireTimer.TotalSeconds > 1)
            //    {
            //        this.humanFireTimer = TimeSpan.FromMilliseconds(0);

            //        // Fire
            //        this.Fire();
            //    }
            //}

            if (this.isFired)
            {
                // Hit enemy fighter --> attach to player fighter
                foreach (Fighter enemyFighter in FighterGame.GameObjects.EnemyFighters)
                {
                    // Hit the enemy
                    if (this.BoundingBox.Intersects(enemyFighter.BoundingBox))
                    {
                        // play explosion sound effect
                        FighterGame.GameObjects.ExplosionSoundEffect.Play();

                        enemyFighter.Reset();

                        FighterGame.GameObjects.PlayerScore.IncreaseScore();

                        // Re-attach to player fighter
                        this.Reset();
                    }

                    // Hit the enemy rocket
                    foreach(Rocket r in enemyFighter.Rockets)
                    {
                        if (this.BoundingBox.Intersects(r.BoundingBox))
                        {
                            // play explosion sound effect
                            FighterGame.GameObjects.ExplosionSoundEffect.Play();

                            // Reset rocket (player & enemy)
                            r.Reset();
                            
                            this.Reset();
                        }
                    }
                }

                this.Location += this.Velocity;

                if (this.Location.Y <= 0)
                {
                    this.Reset();
                }
            }
            else
            {
                // attach to fighter
                this.SetLocation();
            }
        }
        
        private void UpdateComputerRocket(GameTime gameTime)
        {
            if (this.BoundingBox.Intersects(this.GameBoundaries))
            {
                // Within game boundraies --> Count 2 seconds 
                this.fireTimer += gameTime.ElapsedGameTime;
            }

            if (!this.isFired && this.fireTimer.TotalSeconds > 3 )
            {
                this.fireTimer = TimeSpan.FromMilliseconds(0);

                // Fire
                this.Fire();
            }

            if (this.isFired)
            {
                // Hit player fighter --> attach to player fighter
                if (this.BoundingBox.Intersects(FighterGame.GameObjects.PlayerFighter.BoundingBox))
                {
                    // play explosion sound effect
                    FighterGame.GameObjects.ExplosionSoundEffect.Play();
                    FighterGame.GameObjects.CollisionSoundEffect.Play();

                    //TO-DO: Game Over after hitten by enemy fire
                    FighterGame.GameObjects.PlayerFighter.IsGameOver = true;

                    // Re-attach to player fighter
                    this.Reset();
                }

                this.Location += this.Velocity;

                if (this.Location.Y > this.GameBoundaries.Height)
                {
                    this.Reset();
                }
            }
            else
            {
                // attach to fighter
                this.SetLocation();
            }
        }
        
        #endregion

        #region Helper Methods
        
        private void Fire()
        {
            // set velocity
            this.Velocity = new Vector2(0, this.PlayerType == PlayerType.Human ? this.playerRocketSpeed :
                (FighterGame.EnemyFighterSpeed + this.enemyRocketSpeed ));

            // Play fire sound effect
            FighterGame.GameObjects.FireSoundEffect.Play();

            this.isFired = true;
        }

        public void Reset()
        {
            this.isFired = false;

            //this.fireTimer = TimeSpan.FromSeconds(0);

            this.Velocity = Vector2.Zero;
        }

        private void SetLocation()
        {
            this.Location.X = this.attachedToFighter.Location.X + (this.attachedToFighter.Width / 2) - (this.Width / 2);

            this.Location.Y = this.PlayerType == PlayerType.Human ?
                this.attachedToFighter.Location.Y : this.attachedToFighter.Location.Y + this.attachedToFighter.Height - this.Height;
        }
        #endregion
    }
}