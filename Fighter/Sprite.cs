using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fighter
{
    public abstract class Sprite
    {
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

        public Sprite(Texture2D texture, Rectangle gameBoundaries)
        {
            this.Texture = texture;
            this.Location = Vector2.Zero;
            this.GameBoundaries = gameBoundaries;
            this.Velocity = Vector2.Zero;
        }

        public Sprite(Texture2D texture, Vector2 location, Rectangle gameBoundaries)
        {
            this.Texture = texture;
            this.Location = location;
            this.GameBoundaries = gameBoundaries;
            this.Velocity = Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Location, Color.White);
        }

        public virtual void Update(GameTime gameTime, GameObjects gameObjects)
        {
            this.Location += this.Velocity;

            this.CheckBounds();
        }

        protected abstract void CheckBounds();
    }
}