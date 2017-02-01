using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class FadeOutText : TextItem
    {
        public FadeOutText(SpriteFont loadedFont, string spriteText = "") : base(loadedFont, spriteText)
        {
            position = new Vector2(32, Game1.WindowHeight - loadedFont.MeasureString(spriteText).Y - 32);
            velocity = new Vector2(0, -0.5f);
            origin = Vector2.Zero;
        }

        public override void Update()
        {
            alpha -= 0.01f;
            if (alpha <= 0)
            {
                visible = false;
            }
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);
            }
        }
    }
}
