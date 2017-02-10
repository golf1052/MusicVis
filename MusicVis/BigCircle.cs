using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class BigCircle : Spawnable
    {
        private const float SpawnScale = 0.01f;
        private const float DespawnScale = 3.75f;

        private float scaleRate;
        private float fadeRate;

        public BigCircle(Texture2D tex) : base(tex)
        {
            sprite.position = new Vector2(Game1.WindowWidth / 2, Game1.WindowHeight / 2);
            sprite.scale = SpawnScale;
            scaleRate = 0.05f;
            fadeRate = (float)World.Random.NextDouble(0.003, 0.009);
            //int randomColor = World.Random.Next(0, 4);
            //if (randomColor == 0)
            //{
            //    sprite.color = Color.White;
            //}
            //else if (randomColor == 1)
            //{
            //    sprite.color = Color.Red;
            //}
            //else if (randomColor == 2)
            //{
            //    sprite.color = Color.Green;
            //}
            //else if (randomColor == 3)
            //{
            //    sprite.color = Color.Blue;
            //}
        }

        public override void Update(GameTime gameTime)
        {
            sprite.alpha -= fadeRate;
            if (sprite.alpha <= 0)
            {
                sprite.visible = false;
            }
            sprite.scale += scaleRate;
            if (sprite.scale >= DespawnScale)
            {
                sprite.visible = false;
            }
            sprite.Update();
        }
    }
}
