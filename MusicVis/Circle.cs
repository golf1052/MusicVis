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
        public Circle(Texture2D tex, float yPosition, int slot, World.Side side) : this(tex, yPosition, slot, side, false)
        {
        }

        public Circle(Texture2D tex, float yPosition, int slot, World.Side side, bool valentines) : base(tex)
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
            sprite.rotation = World.Random.Next(0, 360);
            if (valentines)
            {
                sprite.color = Color.Lerp(Color.Red, Color.White, slot / 220f);
            }
            else
            {
                sprite.color = new Color(World.Random.Next(0, 255), World.Random.Next(0, 255), World.Random.Next(0, 255));
            }
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
