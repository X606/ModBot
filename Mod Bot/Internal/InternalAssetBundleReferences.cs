using ModLibrary;
using System.IO;
using UnityEngine;

namespace InternalModBot
{
    internal static class InternalAssetBundleReferences
    {
        private static AssetBundleInfo _modbotBundle;
        internal static AssetBundleInfo ModBot
        {
            get
            {
                if (_modbotBundle != null) return _modbotBundle;

                string mainDirectory = InternalUtils.GetSubdomain(Application.dataPath);
                string dataFolderName = "Clone Drone in the Danger Zone_Data/";
                if (!Directory.Exists(Path.Combine(mainDirectory, dataFolderName)))
                {
                    dataFolderName = "Clone Drone in the Danger Zone.exe_Data/";
                    if (!Directory.Exists(Path.Combine(mainDirectory, dataFolderName)))
                    {
                        throw new System.IO.DirectoryNotFoundException("Could not found game data directory!");
                    }
                }

                _modbotBundle = AssetLoader.GetAssetBundle("modbot", dataFolderName);
                return _modbotBundle;
            }
        }
    }
}
