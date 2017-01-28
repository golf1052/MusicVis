using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class ScreenFlashManager
    {
        private Sprite sprite;
        private float fadeRate;
        public bool On { get; set; }

        public ScreenFlashManager(GraphicsDeviceManager graphics)
        {
            sprite = new Sprite(graphics);
            sprite.drawRect = new Rectangle(0, 0, Game1.WindowWidth, Game1.WindowHeight);
            fadeRate = 0.1f;
            On = false;
        }

        public void Flash(float value)
        {
            sprite.alpha = value;
        }

        public void Update(GameTime gameTime)
        {
            sprite.alpha -= fadeRate;
            if (sprite.alpha <= 0)
            {
                sprite.alpha = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.DrawRect(spriteBatch);
        }
    }
}
