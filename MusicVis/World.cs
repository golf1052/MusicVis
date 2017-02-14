using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.UI.Input;

namespace MusicVis
{
    public class World
    {
        public enum Side
        {
            Left,
            Right
        }

        private static Random random;
        private static Queue<int> lastRandomNumbers;
        public static Random Random
        {
            get
            {
                if (lastRandomNumbers.Count < 5)
                {
                    lastRandomNumbers.Enqueue(random.Next(0, 100));
                }
                else if (lastRandomNumbers.Count == 5)
                {
                    bool allNumbers0 = true;
                    foreach (var number in lastRandomNumbers)
                    {
                        if (number != 0)
                        {
                            allNumbers0 = false;
                            break;
                        }
                    }

                    if (allNumbers0)
                    {
                        lastRandomNumbers.Clear();
                        random = new Random();
                    }
                    else
                    {
                        lastRandomNumbers.Dequeue();
                        lastRandomNumbers.Enqueue(random.Next(0, 100));
                    }
                }
                return random;
            }
        }
        public static RadialController dial;
        public static RadialControllerConfiguration dialConfig;

        static World()
        {
            lastRandomNumbers = new Queue<int>(5);
            random = new Random();
            dial = RadialController.CreateForCurrentView();
            dialConfig = RadialControllerConfiguration.GetForCurrentView();
        }
    }
}
