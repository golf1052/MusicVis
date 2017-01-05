using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class CircleManager
    {
        private Texture2D circleTexture;

        private Dictionary<int, List<Circle>> circles = new Dictionary<int, List<Circle>>();

        public CircleManager(Texture2D circleTexture)
        {
            this.circleTexture = circleTexture;
            circles = new Dictionary<int, List<Circle>>();
            for (int i = 0; i < 220; i++)
            {
                circles.Add(i, new List<Circle>());
            }
        }

        public void Spawn(int slot, float yPosition)
        {
            if (circles[slot].Count < 10)
            {
                Circle tmp = new Circle(circleTexture, (float)World.Random.NextDouble(0.005, 0.1), yPosition, slot);
                circles[slot].Add(tmp);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var circle in circles)
            {
                for (int i = 0; i < circle.Value.Count; i++)
                {
                    if (circle.Value[i] != null)
                    {
                        circle.Value[i].Update(gameTime);
                        if (!circle.Value[i].Visible)
                        {
                            circle.Value.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var circle in circles)
            {
                for (int i = 0; i < circle.Value.Count; i++)
                {
                    if (circle.Value[i] != null)
                    {
                        circle.Value[i].Draw(spriteBatch);
                    }
                }
            }
        }
    }
}
