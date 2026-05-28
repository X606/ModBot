using ModLibrary.LevelEditor;
using UnityEngine;

namespace InternalModBot
{
    internal static class OverrideResourceManager
    {
        public static Object GetObjectOverride(string path, System.Type type)
        {
            Object overrideResource;

            // get transform or preview image of level editor custom object
            if (type == typeof(Transform))
            {
                overrideResource = CustomLevelEditorManager.GetTransform(path);
            }
            else if (type == typeof(Texture2D))
            {
                overrideResource = CustomLevelEditorManager.GetTexture(path);
            }
            else
            {
                overrideResource = null;
            }

            if (overrideResource != null)
            {
                return overrideResource;
            }

            if (ModsManager.Instance)
            {
                PassOnToModsManager passOnMod = ModsManager.Instance.PassOnMod;
                overrideResource = passOnMod.OnResourcesLoad(path, type);
                if (overrideResource == null)
                {
                    overrideResource = passOnMod.OnResourcesLoad(path);
                }

                if (overrideResource != null)
                {
                    return overrideResource;
                }
            }

            return null;
        }
    }
}