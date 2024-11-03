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
    class LethalObject : Component
    {
        /// <summary>
        /// When the player collides with a lethal object
        /// it dies and respawns to its starting point
        /// </summary>
        /// <param name="otherCollider"></param>
        public override void OnCollision(Collision otherCollider)
        {
            // If there is a collision between the two objects, check to see if it is the player
            if (otherCollider.CallingCollider != null)
            {
                // If the colliding object is the player and the player object exists, then set HitLethal to true and respawn the player
                if (otherCollider.CallingCollider.GameObject is Player p && p != null)
                {
                    p.HitLethal = true;
                    p.Respawn();
                }
            }
        }
    }
}
