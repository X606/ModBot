using HarmonyLib;
using InternalModBot.LevelEditor;
using ModLibrary;
using System.Runtime.Serialization;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LibraryListItemDisplay))]
    static class LibraryListItemDisplay_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LibraryListItemDisplay.PNGPathToSprite))]
        private static bool PNGPathToSprite_Prefix(ref Sprite __result, string previewPathUnderResources)
        {
            Texture2D texture;
            if (CustomLevelEditorManager.HasTexture(previewPathUnderResources))
            {
                texture = CustomLevelEditorManager.GetTexture(previewPathUnderResources);
            }
            else
            {
                texture = Resources.Load<Texture2D>(previewPathUnderResources.Replace(".png", ""));
            }

            if (texture == null)
            {
                __result = null;
                return false;
            }

            __result = texture.ToSprite();
            return false;
        }
    }
}