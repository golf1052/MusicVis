using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class RowLine
    {
        public List<Line> lines;
        int lineCount = 10;
        float yPosition;

        public RowLine(GraphicsDeviceManager graphics, float yPosition)
        {
            lines = new List<Line>();
            this.yPosition = yPosition;
            for (int i = 0; i < lineCount; i++)
            {
                Line line = new Line(graphics);
                float lineWidth = Game1.WindowWidth / (float)lineCount;
                line.point1 = new Vector2(i * 100, yPosition);
                line.point2 = new Vector2(line.point1.X + 100, yPosition);
                line.thickness = 1;
                Vector2 vector = new Vector2(1, 0);
                float magnitude = 0;
                line.velocity = vector * magnitude;
                lines.Add(line);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < lineCount; i++)
            {
                lines[i].Update();
            }
        }

        private void Reset()
        {
            for (int i = 0; i < lineCount; i++)
            {
                lines[i].point1 = new Vector2(i * 100, yPosition);
                lines[i].point2 = new Vector2(lines[i].point1.X + 100, yPosition);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var line in lines)
            {
                line.Draw(spriteBatch);
            }
        }
    }
}
