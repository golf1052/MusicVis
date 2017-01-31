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
        private List<Texture2D> textures;

        private Dictionary<int, List<Circle>> leftCircles = new Dictionary<int, List<Circle>>();
        private Dictionary<int, List<Circle>> rightCircles = new Dictionary<int, List<Circle>>();
        private int capacity = 10;
        public bool On { get; set; }

        public CircleManager(List<Texture2D> textures)
        {
            this.textures = textures;
            leftCircles = new Dictionary<int, List<Circle>>();
            rightCircles = new Dictionary<int, List<Circle>>();
            for (int i = 0; i < 220; i++)
            {
                leftCircles.Add(i, new List<Circle>(10));
                rightCircles.Add(i, new List<Circle>(10));
            }
            On = false;
        }

        public void Spawn(float value, int slot, float yPosition, World.Side side)
        {
            for (int i = 0; i < value * 3; i++)
            {
                int randomTexture = World.Random.Next(0, textures.Count);
                if (side == World.Side.Left && leftCircles[slot].Count < capacity)
                {
                    Circle tmp = new Circle(textures[randomTexture], yPosition, slot, side);
                    leftCircles[slot].Add(tmp);
                }
                else if (side == World.Side.Right && rightCircles[slot].Count < capacity)
                {
                    Circle tmp = new Circle(textures[randomTexture], yPosition, slot, side);
                    rightCircles[slot].Add(tmp);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var circle in leftCircles)
            {
                UpdateCircle(circle, gameTime);
            }

            foreach (var circle in rightCircles)
            {
                UpdateCircle(circle, gameTime);
            }
        }

        private void UpdateCircle(KeyValuePair<int, List<Circle>> circle, GameTime gameTime)
        {
            for (int i = 0; i < circle.Value.Count; i++)
            {
                if (circle.Value[i] != null)
                {
                    circle.Value[i].Update(gameTime);
                    if (!circle.Value[i].Visible)
                    {
                        try
                        {
                            circle.Value.RemoveAt(i);
                            i--;
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var circle in leftCircles)
            {
                DrawCircle(circle, spriteBatch);
            }

            foreach (var circle in rightCircles)
            {
                DrawCircle(circle, spriteBatch);
            }
        }

        private void DrawCircle(KeyValuePair<int, List<Circle>> circle, SpriteBatch spriteBatch)
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
