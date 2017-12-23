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
using Microsoft.Xna.Framework.Media;

namespace Fighter
{
    public class Fighter
    {
        #region Defintions

        private int enemyFighterNumber;
        private float verticalFlightSpeed = 25f;
        private float horizontalFlightSpeed = 20f;

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

        public float EnemyFlightSpeed { get; set; }

        public PlayerType PlayerType { get; set; }

        public bool IsGameOver { get; set; }

        public int CurrentRocketNumber { get; set; }

        public List<Rocket> Rockets { get; set; }

        public bool IsDragging = false;

        #endregion

        #region Construction

        public Fighter(Texture2D texture, Rectangle gameBoundaries, PlayerType playerType, int enemyFighterNumber)
        {
            this.enemyFighterNumber = enemyFighterNumber;

            this.Texture = texture;

            this.GameBoundaries = gameBoundaries;

            this.PlayerType = playerType;

            this.Velocity = Vector2.Zero;

            this.Location = this.GetLocation();

            this.Rockets = new List<Rocket>();
            
            //if (playerType == PlayerType.Human)
            //{
            //    var instance = FighterGame.GameObjects.AirplanSoundEffect.CreateInstance();
            //    instance.IsLooped = true;
            //    instance.Play();
            //}
        }

        #endregion

        #region Draw & Update
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Location, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            if (this.PlayerType == PlayerType.Computer)
            {
                this.Velocity = new Vector2(0, this.EnemyFlightSpeed);

                this.Location += this.Velocity;
            }

            if (this.PlayerType == PlayerType.Human)
            {
                if(FighterGame.GameObjects.TouchInput.Position == Vector2.Zero)
                {
                    this.IsDragging = false;
                }
                
                int locationX = Convert.ToInt32(FighterGame.GameObjects.TouchInput.Position.X);
                int locationY = Convert.ToInt32(FighterGame.GameObjects.TouchInput.Position.Y);
                var point = new Point(locationX, locationY);

                if (this.BoundingBox.Contains(point) && !FighterGame.GameObjects.TouchInput.Tapped)
                {
                    this.IsDragging = true;
                }

                if (this.IsDragging)
                {
                    this.Location = new Vector2((locationX - this.Width / 2), (locationY - this.Height / 2));
                }
            }

            this.CheckBounds();

            this.CheckCollision();
        }

        #endregion

        #region Helper Methods

        private Vector2 GetLocation()
        {
            if (this.PlayerType == PlayerType.Human)
            {
                return new Vector2(this.GameBoundaries.Center.X - (this.Width / 2),
                     this.GameBoundaries.Height - this.Height - 50);
            }

            // Computer fighter --> return Random location
            //int x = this.GetLocationX();
            //float y = this.GetLocationY();

            int x = new Random().Next(0, this.GameBoundaries.Width - this.Width);
            float y = -200;

            if (FighterGame.GameObjects.EnemyFighters.Count > 0)
            {
                var minY = 0f;
                foreach (Fighter f in FighterGame.GameObjects.EnemyFighters)
                {
                    if (f.Location.Y < minY)
                        minY = f.Location.Y;
                }

                y = minY - 300;
            }

            return new Vector2(x, y);
        }

        private int GetLocationX()
        {
            int cellWidth = this.GameBoundaries.Width / 8;

            switch (this.enemyFighterNumber)
            {
                case 1:
                case 5:
                case 7:
                case 11:
                case 13:
                case 17:
                    return (4 * cellWidth) - (this.Width / 2);
                case 2:
                case 8:
                case 14:
                    return (2 * cellWidth) - (this.Width / 2);
                case 3:
                case 9:
                case 15:
                    return (6 * cellWidth) - (this.Width / 2);
                case 4:
                case 10:
                case 16:
                    return (1 * cellWidth) - (this.Width / 2);
                case 6:
                case 12:
                case 18:
                    return (7 * cellWidth) - (this.Width / 2);
                default:
                    return 0;
            }
            
        }

        private float GetLocationY()
        {
            float cellHeight = -200;

            switch (this.enemyFighterNumber)
            {
                case 1:
                    return 2 * cellHeight + this.GetEnemyFighterLocationY(16);
                case 2:
                case 3:
                    return cellHeight + this.GetEnemyFighterLocationY(1);
                case 4:
                case 5:
                case 6:
                    return cellHeight + this.GetEnemyFighterLocationY(2);
                case 7:
                    return 2 * cellHeight + this.GetEnemyFighterLocationY(4);
                case 8:
                case 9:
                    return cellHeight + this.GetEnemyFighterLocationY(7);
                case 10:
                case 11:
                case 12:
                    return cellHeight + this.GetEnemyFighterLocationY(8);
                case 13:
                    return 2 * cellHeight + this.GetEnemyFighterLocationY(10);
                case 14:
                case 15:
                    return cellHeight + this.GetEnemyFighterLocationY(13);
                case 16:
                case 17:
                case 18:
                    return cellHeight + this.GetEnemyFighterLocationY(14);
                default:
                    return 0;
            }

        }

        private float GetEnemyFighterLocationY(int enemyFighterNumber)
        {
            if(FighterGame.GameObjects.EnemyFighters.Count<=0)
            {
                return 0;
            }

            return FighterGame.GameObjects.EnemyFighters[enemyFighterNumber - 1].Location.Y;
        }

        public void Reset()
        {
            if (this.PlayerType == PlayerType.Human)
            {
                this.IsGameOver = false;
                this.Velocity = Vector2.Zero;
            }
            else
            {
                this.EnemyFlightSpeed = FighterGame.EnemyFighterSpeed;
            }

            this.Location = this.GetLocation();
        }

        protected void CheckBounds()
        {
            if (this.PlayerType == PlayerType.Computer)
            {
                if (this.Location.Y > GameBoundaries.Height)
                {
                    this.Reset();
                }
                return;
            }

            this.Location.X = MathHelper.Clamp(Location.X, 0, GameBoundaries.Width - Width);
            this.Location.Y = MathHelper.Clamp(Location.Y, 0, GameBoundaries.Height - Height);
        }

        private void CheckCollision()
        {
            return;

            if (this.PlayerType == PlayerType.Computer)
            {
                return;
            }

            foreach (Fighter enemyFighter in FighterGame.GameObjects.EnemyFighters)
            {
                if (this.BoundingBox.Intersects(enemyFighter.BoundingBox))
                {
                    this.IsGameOver = true;

                    // play explosion sound effect
                    FighterGame.GameObjects.CollisionSoundEffect.Play();
                    break;
                }
            }
        }
        #endregion
    }

    public enum PlayerType
    {
        Human,
        Computer
    }
}