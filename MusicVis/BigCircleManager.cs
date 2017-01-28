using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class BigCircleManager
    {
        private Texture2D circleTexture;
        private List<BigCircle> circles;
        public bool On { get; set; }

        public BigCircleManager(Texture2D tex)
        {
            circleTexture = tex;
            circles = new List<BigCircle>();
            On = true;
        }

        public void Spawn()
        {
            BigCircle tmp = new BigCircle(circleTexture);
            circles.Add(tmp);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < circles.Count; i++)
            {
                if (circles[i] != null)
                {
                    circles[i].Update(gameTime);
                    if (!circles[i].Visible)
                    {
                        circles.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < circles.Count; i++)
            {
                if (circles[i] != null)
                {
                    circles[i].Draw(spriteBatch);
                }
            }
        }
    }
}
