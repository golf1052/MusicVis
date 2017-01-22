using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class Flash
    {
        private Sprite sprite;
        private float fadeRate;
        public int slot;

        public bool Visible
        {
            get
            {
                return sprite.visible;
            }
        }

        public Color Color
        {
            get
            {
                return sprite.color;
            }
        }

        public Flash(Texture2D tex, int slot, float value, float yPosition)
        {
            sprite = new Sprite(tex);
            this.slot = slot;
            sprite.position = new Vector2(World.Random.Next(0, Game1.WindowWidth),
                World.Random.Next((int)yPosition - 100, (int)yPosition + 100));
            //sprite.velocity = new Vector2((float)World.Random.NextDouble(0.1, 5), 0);
            sprite.scale = 5 * value;
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
            fadeRate = (float)World.Random.NextDouble(0.05, 0.1);
            if (value >= 0.9f)
            {
                sprite.color = Color.White;
                fadeRate = 0.1f;
            }
        }

        public void Update(GameTime gameTime)
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (sprite.visible)
            {
                sprite.Draw(spriteBatch);
            }
        }
    }
}
