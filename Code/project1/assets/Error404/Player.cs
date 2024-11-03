using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Error404
{
    /// <summary>
    /// Enumeration that tracks what state the player is in
    /// </summary>
    public enum PlayerState
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight,
        JumpLeft,
        JumpRight,
        FallingLeft,
        FallingRight,
        DyingLeft,
        DyingRight
    }

    class Player : GameObject
    {
        private int deathCount;
        private PlayerState prevState;
        private int coinsCollected;
        private Dictionary<string, Texture2D[]> animations;
        private Animation animator;

        private Collider collider;

        private int speed;
        public Vector2 Size;
        public Vector2 Offset = Vector2.Zero;
        private bool hitLethal;
        private int gravity = 2;
        private double fps = 60;
        private double time = 1;
        private Vector2 velocity;
        private PlayerState state;
        private Vector2 acceleration;
        private Vector2 startPosition;

        public int DeathCount { get { return deathCount; } set { deathCount = value; } }
        public int CoinsCollected { get { return coinsCollected; } set { coinsCollected = value; } }
        public bool IsGrounded
        {
            get
            {
                return CollisionManager.GetInstance().BoxCast(new Rectangle(this.position.ToPoint() + Offset.ToPoint() + new Point(0, 1), this.Size.ToPoint()));
            }
        }
        public new Rectangle Position
        {
            get
            {
                return new Rectangle(this.position.ToPoint(), Size.ToPoint());
            }
        }
        public int Speed { get { return speed; } set { speed = value; } }
        public PlayerState State { get { return state; } set { state = value; } }
        public bool HitLethal { get { return hitLethal; } set { hitLethal = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Acceleration { get { return acceleration; } set { acceleration = value; } }

        public Player(Rectangle position, Dictionary<string, Texture2D[]> animations) : base(new Vector2(position.X, position.Y))
        {
            speed = 4;
            deathCount = 0;
            coinsCollected = 0;
            velocity = new Vector2(0, 1);
            state = PlayerState.FaceRight;
            acceleration = new Vector2(0, 0.5f);
            this.position += new Vector2(0, -5);
            startPosition = new Vector2(position.X, position.Y);
            this.animations = animations;
            this.animator = new Animation(animations, 1);
            this.Size = position.Size.ToVector2();
            this.AddComponent<Animation>(animator);
            collider = this.GetComponent<Collider>();

        }

        public void MovePlayer(KeyboardState input, GameTime gt)
        {
            if (input.IsKeyDown(Keys.A))
            {
                velocity.X = -speed;
                state = PlayerState.WalkLeft;
                prevState = PlayerState.WalkLeft;
            }
            else if (input.IsKeyDown(Keys.D))
            {
                state = PlayerState.WalkRight;
                prevState = PlayerState.WalkRight;
                velocity.X = speed;
            }
            else
            {
                if (prevState == PlayerState.WalkLeft || prevState == PlayerState.FallingLeft)
                {
                    state = PlayerState.FaceLeft;
                }
                velocity.X = 0;
            }

            if (IsGrounded)
            {
                this.velocity.Y = 0;
                if (input.IsKeyDown(Keys.W))
                {
                    if (prevState == PlayerState.WalkLeft || prevState == PlayerState.FallingLeft)
                    {
                        state = PlayerState.JumpLeft;
                        prevState = PlayerState.JumpLeft;
                    }
                    else
                    {
                        state = PlayerState.JumpRight;
                        prevState = PlayerState.JumpRight;
                    }
                    velocity.Y = -10;
                }
            }
            else
            {
                this.velocity += acceleration;
                if (velocity.Y >= 0)
                {
                    if (prevState == PlayerState.JumpLeft)
                    {
                        state = PlayerState.FallingLeft;
                        prevState = PlayerState.FallingLeft;
                    }
                    else
                    {
                        state = PlayerState.FallingRight;
                        prevState = PlayerState.FallingRight;
                    }
                }
            }

            if (velocity.Y > 10)
            {
                velocity.Y = 10;
            }
            this.position += velocity;
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawPlayer();
            base.Draw(sb);
        }

        public override void Update()
        {
            //This Line of Code is for Testing Purposes, do not delete
            //this.position -= new Vector2(0, 1);
            MovePlayer(ControlsManager.GetInstance().CurrentKeyboardState, TimeManager.GetInstance().Time);
            DrawPlayer();
            base.Update();
        }

        public void DrawPlayer()
        {
            switch (state)
            {
                case PlayerState.FaceRight:
                    if (animations.ContainsKey("Idle"))
                    {
                        animator.Play("Idle", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.FaceLeft:
                    if (animations.ContainsKey("Idle"))
                    {
                        animator.Play("Idle", SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.WalkRight:
                    if (animations.ContainsKey("Walking"))
                    {
                        animator.Play("Walking", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.WalkLeft:
                    if (animations.ContainsKey("Walking"))
                    {
                        animator.Play("Walking", SpriteEffects.FlipHorizontally);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.JumpRight:
                    if (animations.ContainsKey("Jumping"))
                    {
                        // "Falling" should be the second half of the jump cycle
                        // Need to split up states for when player is ascending and descending
                        animator.Play("Jumping", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.JumpLeft:
                    if (animations.ContainsKey("Jumping"))
                    {
                        animator.Play("Jumping", SpriteEffects.FlipVertically);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.FallingRight:
                    if (animations.ContainsKey("Falling"))
                    {
                        animator.Play("Falling", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.FallingLeft:
                    if (animations.ContainsKey("Falling"))
                    {
                        animator.Play("Falling", SpriteEffects.FlipVertically);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.DyingRight:
                    if (animations.ContainsKey("Dying"))
                    {
                        animator.Play("Dying", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                case PlayerState.DyingLeft:
                    if (animations.ContainsKey("Dying"))
                    {
                        animator.Play("Dying", SpriteEffects.FlipVertically);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
                default:
                    if (animations.ContainsKey("Idle"))
                    {
                        animator.Play("Idle", SpriteEffects.None);
                    }
                    else
                    {
                        animator.Stop();
                    }
                    break;
            }
        }

        public override void OnCollision(Collision col)
        {
            PlayerCollision(col);
            base.OnCollision(col);
        }

        /// <summary>
        /// On Collision Move Player to the Nearest Safe Collision Point as Specified
        /// </summary>
        /// <param name="col"></param>
        public void PlayerCollision(Collision col)
        {
            //If the NearestSafeCOllisionPoint exists, go to it, else go to your current position, when you collide.
            //this.position = col.NearestSafeCollisionPoint ?? this.position;

            //Collisions with rectangles of less than 3 pixels in both directions miiiiight be determined to be neglegible???
            //Just an idea in case things go wonky??? Just keeping it as an idea to throw around

            //Determines which direction to push the player
            if (col.Intersect.Width >= col.Intersect.Height)
            {//if moving vertically
                if (col.ActiveColliderPos.Y < col.StaticColliderPos.Y)
                {//colliding from above
                    this.position.Y -= col.Intersect.Height;
                    this.velocity.Y -= col.Intersect.Height;
                }
                else if (col.ActiveColliderPos.Y > col.StaticColliderPos.Y)
                {//colliding from below
                    this.position.Y += col.Intersect.Height;
                    this.velocity.Y += col.Intersect.Height;
                }
            }
            else if (col.Intersect.Height > col.Intersect.Width)
            {//colliding horizontally
                if (col.ActiveColliderPos.X < col.StaticColliderPos.X)
                {//colliding from above
                    this.position.X -= col.Intersect.Width;
                }
                else if (col.ActiveColliderPos.X > col.StaticColliderPos.X)
                {//colliding from below
                    this.position.X += col.Intersect.Width;
                }
            }
        }

        /// <summary>
        /// Respawns player after death at its start position
        /// </summary>
        public void Respawn()
        {
            this.position = startPosition;
            hitLethal = false;
            deathCount++;
        }

        public void SetSpawnPoint(Vector2 pos)
        {
            startPosition = pos;
            this.position = startPosition;
        }
    }
}