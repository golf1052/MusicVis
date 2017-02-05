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

        private static object _lock = new object();
        private static Random random;
        public static Random Random
        {
            get
            {
                lock (_lock)
                {
                    return random;
                }
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
