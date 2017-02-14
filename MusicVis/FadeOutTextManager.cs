using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class FadeOutTextManager
    {
        private SpriteFont spriteFont;
        private List<FadeOutText> texts;

        public FadeOutTextManager(SpriteFont spriteFont)
        {
            this.spriteFont = spriteFont;
            texts = new List<FadeOutText>();
        }

        public void Create(string text)
        {
            FadeOutText tmp = new FadeOutText(spriteFont, text);
            texts.Add(tmp);
        }

        public void CreateState(bool state)
        {
            string text = state ? "On" : "Off";
            Vector2 position = new Vector2(Game1.WindowWidth - spriteFont.MeasureString(text).X - 32,
                Game1.WindowHeight - spriteFont.MeasureString(text).Y - 32);
            FadeOutText tmp = new FadeOutText(position, spriteFont, text);
            texts.Add(tmp);
        }

        public void Update()
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].Update();
                if (!texts[i].visible)
                {
                    texts.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].Draw(spriteBatch);
            }
        }
    }
}
