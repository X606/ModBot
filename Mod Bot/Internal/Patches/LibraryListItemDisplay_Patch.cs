using HarmonyLib;
using ModLibrary;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LibraryListItemDisplay))]
    static class LibraryListItemDisplay_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LibraryListItemDisplay.PNGPathToSprite))]
        private static bool PNGPathToSprite_Prefix(ref Sprite __result, string previewPathUnderResources) // makes the game not crash if some object doesn't have a preview image
        {
            Texture2D texture = Resources.Load<Texture2D>(previewPathUnderResources.Replace(".png", ""));
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