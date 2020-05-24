using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    internal static class InternalAssetBundleCache
    {
        internal static AssetBundleInfo ModsWindow;
        internal static AssetBundleInfo TwitchMode;

        internal static void Initialize()
        {
            ModsWindow = AssetLoader.GetAssetBundle("modswindow", "Clone Drone in the Danger Zone_Data/");
            TwitchMode = AssetLoader.GetAssetBundle("twitchmode", "Clone Drone in the Danger Zone_Data/");
        }
    }
}
