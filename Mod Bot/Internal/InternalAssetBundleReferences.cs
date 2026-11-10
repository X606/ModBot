using ModLibrary;

namespace InternalModBot
{
    internal static class InternalAssetBundleReferences
    {
        private static AssetBundleInfo _modbotBundle;
        internal static AssetBundleInfo ModBot
        {
            get
            {
                if (_modbotBundle == null)
                {
                    _modbotBundle = AssetLoader.GetAssetBundle("modbot", "ModBot/");
                }
                return _modbotBundle;
            }
        }
    }
}