using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class LineManager
    {
        public List<RowLine> rows;

        public LineManager(GraphicsDeviceManager graphics)
        {
            rows = new List<RowLine>(220);
            for (int i = 0; i < 220; i++)
            {
                int slot = i * (Game1.WindowHeight / 220);
                int inverseSlot = Game1.WindowHeight - slot;
                rows.Add(new RowLine(graphics, slot));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var row in rows)
            {
                row.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var row in rows)
            {
                row.Draw(spriteBatch);
            }
        }
    }
}
