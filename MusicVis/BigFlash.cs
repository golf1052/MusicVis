using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class BigFlash : Spawnable
    {
        private const float MinScale = 0.5f;
        private const float MaxScale = 2.0f;
        public bool On { get; set; }

        public BigFlash(Texture2D tex) : base(tex)
        {
            sprite.position = new Vector2(Game1.WindowWidth / 2, Game1.WindowHeight / 2);
            sprite.scale = 0.5f;
            On = true;
        }

        public void Pump(float value)
        {
            sprite.scale = value * 3f;
            int randomColor = World.Random.Next(0, 4);
            if (randomColor == 0)
            {
                sprite.color = Color.White;
            }
            else if (randomColor == 1)
            {
                sprite.color = Color.Red;
            }
            else if (randomColor == 2)
            {
                sprite.color = Color.Green;
            }
            else if (randomColor == 3)
            {
                sprite.color = Color.Blue;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (sprite.scale > 0.5f)
            {
                sprite.scale -= 0.1f;
            }
            sprite.scale = MathHelper.Clamp(sprite.scale, 0.5f, 2);
            sprite.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (On)
            {
                base.Draw(spriteBatch);
            }
        }
    }
}
