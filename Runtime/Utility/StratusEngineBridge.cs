using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus
{
    /// <summary>
    /// A class which the current game engine can connect delegates to
    /// </summary>
    public static class StratusEngineBridge
    {
        public static Func<bool> isPlaying;
    }
}
