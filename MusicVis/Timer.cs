using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MusicVis
{
    public class Timer
    {
        private TimeSpan timer;
        private TimeSpan startingTime;
        private float resetValue;
        private Action<float> resetFunc;

        public Timer(TimeSpan startingTime, Action<float> resetFunc, float resetValue)
        {
            this.startingTime = startingTime;
            timer = startingTime;
            this.resetValue = resetValue;
            this.resetFunc = resetFunc;
        }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime;
            if (timer <= TimeSpan.Zero)
            {
                Debug.WriteLine($"timer of {startingTime.TotalSeconds} invoked");
                timer = startingTime;
                resetFunc.Invoke(resetValue);
            }
        }

        public void Reset()
        {
            timer = startingTime;
        }
    }
}
