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
