using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public abstract class Spawnable
    {
        protected Sprite sprite;
        public bool Visible
        {
            get
            {
                return sprite.visible;
            }
        }

        public Spawnable(Texture2D tex)
        {
            sprite = new Sprite(tex);
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (sprite.visible)
            {
                sprite.Draw(spriteBatch);
            }   
        }
    }
}
