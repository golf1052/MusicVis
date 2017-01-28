using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class FlashManager
    {
        private Texture2D flashTexture;
        private List<Flash> flashes;
        private List<int> numBeats;
        public bool On { get; set; }

        public FlashManager(Texture2D flashTexture)
        {
            this.flashTexture = flashTexture;
            flashes = new List<Flash>();
            numBeats = new List<int>(220);
            for (int i = 0; i < 220; i++)
            {
                numBeats.Add(0);
            }
            On = true;
        }

        public void Spawn(int slot, float value, float yPosition, World.Side side)
        {
            if (value >= 0.9f)
            {
                if (numBeats[slot] < 1)
                {
                    Flash tmp = new Flash(flashTexture, slot, value, yPosition, side);
                    numBeats[slot]++;
                    flashes.Add(tmp);
                }
            }
            else
            {
                Flash tmp = new Flash(flashTexture, slot, value, yPosition, side);
                flashes.Add(tmp);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < flashes.Count; i++)
            {
                if (flashes[i] != null)
                {
                    flashes[i].Update(gameTime);
                    if (!flashes[i].Visible)
                    {
                        if (flashes[i].Color == Color.White)
                        {
                            numBeats[flashes[i].slot]--;
                        }
                        flashes.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < flashes.Count; i++)
            {
                if (flashes[i] != null)
                {
                    flashes[i].Draw(spriteBatch);
                }
            }
        }
    }
}
