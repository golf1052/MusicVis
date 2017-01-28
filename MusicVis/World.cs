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

        public static Random Random;
        public static RadialController dial;
        public static RadialControllerConfiguration dialConfig;

        static World()
        {
            Random = new Random();
            dial = RadialController.CreateForCurrentView();
            dialConfig = RadialControllerConfiguration.GetForCurrentView();
        }
    }
}
