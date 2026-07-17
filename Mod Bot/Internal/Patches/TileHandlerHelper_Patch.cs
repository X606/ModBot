using HarmonyLib;
using Pathfinding;

namespace InternalModBot
{
    [HarmonyPatch(typeof(TileHandlerHelper))] // fixes the crash when exiting game due to delayed unloading
    static class TileHandlerHelper_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(TileHandlerHelper.Update))]
        static bool Update_Prefix()
        {
            return !ModBotUnloader.IsUnloadingGame();
        }
    }
}