using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;

namespace MusicVis
{
    public class World
    {
        public static Random Random;

        static World()
        {
            Random = new Random();
        }
    }
}
