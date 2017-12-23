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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace Fighter
{
    public class GameObjects
    {
        public Fighter PlayerFighter { get; set; }

        public List<Fighter> EnemyFighters { get; set; }

        public PlayerScore PlayerScore { get; set; }

        public TouchInput TouchInput { get; set; }
        
        public GameState GameState { get; set; }

        #region Sound Effects

        public SoundEffect FireSoundEffect { get; set; }

        public SoundEffect ExplosionSoundEffect { get; set; }

        public SoundEffect CollisionSoundEffect { get; set; }

        public SoundEffect HigherLevelSoundEffect { get; set; }

        public Song BackgroundMusic { get; set; }

        public SoundEffect AirplanSoundEffect { get; set; }

        #endregion

        public SpriteFont GameFont { get; set; }
    }

    public class TouchInput
    {
        public bool Tapped { get; set; }

        public bool Hold { get; set; }

        public Vector2 Position { get; set; }
    }

    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }
}