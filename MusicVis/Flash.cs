using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class Flash : Spawnable
    {
        private float fadeRate;
        public int slot;

        public Color Color
        {
            get
            {
                return sprite.color;
            }
        }

        public Flash(Texture2D tex, int slot, float value, float yPosition, World.Side side, bool valentines) : base(tex)
        {
            this.slot = slot;
            float xPos;
            if (side == World.Side.Left)
            {
                xPos = World.Random.Next(0, Game1.WindowWidth / 2);
            }
            else
            {
                xPos = World.Random.Next(Game1.WindowWidth / 2, Game1.WindowWidth);
            }
            sprite.position = new Vector2(xPos,
                World.Random.Next((int)yPosition - 100, (int)yPosition + 100));
            //sprite.velocity = new Vector2((float)World.Random.NextDouble(0.1, 5), 0);
            sprite.scale = 5 * value;
            if (valentines)
            {
                sprite.color = Color.Lerp(Color.Red, Color.White, slot / 220f);
            }
            else
            {
                int spriteColor = World.Random.Next(0, 3);
                if (spriteColor == 0)
                {
                    sprite.color = Color.Red;
                }
                else if (spriteColor == 1)
                {
                    sprite.color = Color.Green;
                }
                else if (spriteColor == 2)
                {
                    sprite.color = Color.Blue;
                }
            }
            fadeRate = (float)World.Random.NextDouble(0.05, 0.1);
            if (value >= 0.9f)
            {
                sprite.color = Color.White;
                fadeRate = 0.1f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            sprite.alpha -= fadeRate;
            if (sprite.alpha <= 0)
            {
                sprite.visible = false;
                return;
            }
            sprite.Update();
            if (!Game1.WindowRectangle.Intersects(sprite.rect))
            {
                sprite.visible = false;
                return;
            }
        }
    }
}
