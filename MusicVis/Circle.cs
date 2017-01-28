using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MusicVis
{
    public class Circle : Spawnable
    {
        private float FadeRate { get; set; }
        public Circle(Texture2D tex, float yPosition, int slot, World.Side side) : base(tex)
        {
            FadeRate = (float)World.Random.NextDouble(0.005, 0.1);
            float xPos;
            if (side == World.Side.Left)
            {
                xPos = World.Random.Next(0, Game1.WindowWidth / 2);
            }
            else
            {
                xPos = World.Random.Next(Game1.WindowWidth / 2, Game1.WindowWidth);
            }
            sprite.position = new Vector2(xPos, yPosition);
            sprite.velocity = new Vector2(0, (float)World.Random.NextDouble(-2, 0));
            sprite.color = new Color(World.Random.Next(0, 255), World.Random.Next(0, 255), World.Random.Next(0, 255));
            sprite.scale = 2;
            //sprite.color = Color.Lerp(Color.Red, Color.Green, slot / 220f);
        }

        public override void Update(GameTime gameTime)
        {
            sprite.alpha -= FadeRate;
            if (sprite.alpha <= 0)
            {
                sprite.visible = false;
                return;
            }
            sprite.Update();
            if (sprite.position.Y - sprite.tex.Height / 2 < 0)
            {
                sprite.visible = false;
            }
        }
    }
}
