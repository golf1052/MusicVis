using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static int zeroRandomCheck = 0;
        private static int numbersChecked = 0;
        public static Random Random
        {
            get
            {
                if (numbersChecked < 5)
                {
                    zeroRandomCheck |= random.Next(0, 100);
                    numbersChecked++;
                }
                else
                {
                    if (zeroRandomCheck == 0)
                    {
                        Debug.WriteLine("creating new random");
                        random = new Random();
                        zeroRandomCheck = 0;
                        numbersChecked = 0;
                    }
                    else
                    {
                        zeroRandomCheck = 0;
                        numbersChecked = 0;
                    }
                }
                return random;
            }
        }
        public static RadialController dial;
        public static RadialControllerConfiguration dialConfig;

        static World()
        {
            random = new Random();
            dial = RadialController.CreateForCurrentView();
            dialConfig = RadialControllerConfiguration.GetForCurrentView();
        }
    }
}
