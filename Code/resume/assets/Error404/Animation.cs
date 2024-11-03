using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Error404
{
    public class Animation : Component
    {
        /// <summary>
        /// current animation to be played
        /// </summary>
        private Texture2D[] frameArray;
        private Dictionary<string, Texture2D[]> animationDictionary;
        private SpriteEffects flip;

        private double timeCounter;
        private int currentFrame;
        private double secondsPerFrame;
        private bool isPlaying;
        private string previousInput; // Holds the string of the key last input into Play
                                      // so when it's called if the animation is already being played
                                      // it doesn't reset it

        public Animation(Dictionary<string, Texture2D[]> animationDictionary, double secondsPerFrame)
        {
            timeCounter = 0;
            isPlaying = false;
            previousInput = "";
            flip = SpriteEffects.None;
            this.secondsPerFrame = secondsPerFrame;
            this.animationDictionary = animationDictionary;
        }

        /// <summary>
        /// Sets isPlaying to true and sets the frameArray to the specified array
        /// </summary>
        /// <param name="animationName">The key of the frameArray to be played</param>
        /// <param name="flip">Flips as needed for different facing directions
        ///                    Default facing direction is to the right, so if the
        ///                    sprite needs to be turned to the left, enter SpriteEffects.FlipHorizontally
        ///                    else SpriteEffects.None</param>
        public void Play(string animationName, SpriteEffects flip)
        {
            if (!animationDictionary.ContainsKey(animationName))
            {
                return;
            }
            if (animationName.Equals(previousInput))
            {
                return;
            }

            currentFrame = 0;
            timeCounter = 0;
            isPlaying = true;
            this.flip = flip;
            previousInput = animationName;
            frameArray = animationDictionary[animationName];
        }
        public void Play(string animationName)
        {
            if (!animationDictionary.ContainsKey(animationName))
            {
                return;
            }
            currentFrame = 0;
            timeCounter = 0;
            isPlaying = true;
            flip = SpriteEffects.None;
            frameArray = animationDictionary[animationName];
        }

        public void Stop()
        {
            timeCounter = 0;
            currentFrame = 0;
            isPlaying = false;
            previousInput = "";
        }

        private void UpdateAnimation()
        {
            timeCounter += TimeManager.GetInstance().Time.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= secondsPerFrame)
            {
                currentFrame++;
                if (currentFrame == frameArray.Length)
                {
                    currentFrame = 0;
                }
                timeCounter -= secondsPerFrame;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isPlaying)
            {
                Animate(sb);
                UpdateAnimation();
            }
        }

        private void Animate(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                frameArray[currentFrame],
                GameObject.Position,
                null,
                Color.White,    // Color
                0.0f,           // Rotation
                Vector2.Zero,   // Origin
                Vector2.One,    // Scale
                flip,           // Flip
                0               // Layer depth
                );
        }
    }
}