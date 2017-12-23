using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fighter
{
    public class PlayerScore
    {
        private readonly SpriteFont font;
        private readonly Rectangle gameBoundaries;

        private int score;
        private int level;

        public int Score { get { return this.score; } }

        public int Level { get { return this.level; } }

        public PlayerScore(SpriteFont font, Rectangle gameBoundaries)
        {
            this.font = font;
            this.gameBoundaries = gameBoundaries;
            this.score = 0;
            this.level = 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var levelText = string.Format("Level: {0}", this.level);
            var scoreText = string.Format("Score: {0}", this.score);
            
            spriteBatch.DrawString(font, levelText, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font, scoreText, new Vector2(10, 10 + this.font.LineSpacing), Color.White);
        }

        public void Reset()
        {
            this.score = 0;
            this.level = 1;
        }

        public void IncreaseScore()
        {
            this.score++;
        }

        public void IncreaseLevel()
        {
            this.level++;
        }

        public void Update(GameTime gameTime, GameObjects gameObjects)
        {
            //if (gameObjects.Ball.Location.X + gameObjects.Ball.Width < 0)
            //{
            //    ComputerScore++;
            //    gameObjects.Ball.AttachTo(gameObjects.PlayerPaddle);
            //}

            //if (gameObjects.Ball.Location.X > gameBoundaries.Width)
            //{
            //    PlayerScore++;
            //    gameObjects.Ball.AttachTo(gameObjects.PlayerPaddle);
            //}
        }
    }
}