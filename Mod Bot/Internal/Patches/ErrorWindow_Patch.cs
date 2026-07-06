using HarmonyLib;
using System.Text;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ErrorWindow))]
    static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ErrorWindow.Show))]
        static bool Show_Prefix(ErrorWindow __instance, string errorMessage)
        {
            __instance.gameObject.SetActive(true);
            __instance.stackTraceWindow.gameObject.SetActive(true); // always show the stack trace
            __instance.stackTraceLabel.text = errorMessage;

            // versions
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Mod-Bot ");
            stringBuilder.Append(ModLibrary.Properties.Resources.ModBotVersion);
            stringBuilder.Append(" | ");

            stringBuilder.Append("Clone Drone ");
            VersionNumberManager versionNumberManager = VersionNumberManager.Instance;
            if (versionNumberManager)
            {
                stringBuilder.Append(versionNumberManager.GetVersionString());
            }
            else
            {
                stringBuilder.Append(" - ");
            }
            stringBuilder.Append(" | ");

            stringBuilder.Append("Unity ");
            stringBuilder.Append(Application.unityVersion);
            stringBuilder.Append(" | ");

            // platform
            stringBuilder.Append(GameVersionManager.GetPlatformName());
            stringBuilder.Append(" | ");

            // language
            if (SettingsManager.Instance.IsInitialized())
            {
                stringBuilder.Append(LocalizationManager.Instance.GetCurrentLanguageCode());
            }
            else
            {
                stringBuilder.Append("N/A");
            }
            stringBuilder.Append(" | ");

            // game level info
            GameFlowManager gameFlowManager = GameFlowManager.Instance;
            if (gameFlowManager)
            {
                stringBuilder.Append(gameFlowManager.GetCurrentGameMode());
            }
            else
            {
                stringBuilder.Append("N/A");
            }
            stringBuilder.Append(" | ");

            LevelManager levelManager = LevelManager.Instance;
            if (levelManager)
            {
                stringBuilder.Append(levelManager.GetLastSpawnedLevelID());
            }
            else
            {
                stringBuilder.Append("N/A");
            }
            stringBuilder.Append(" | ");

            ArenaLiftManager arenaLiftManager = ArenaLiftManager.Instance;
            if (arenaLiftManager && arenaLiftManager.Lift)
            {
                stringBuilder.Append(arenaLiftManager.GetLiftTarget());
            }
            else
            {
                stringBuilder.Append("N/A");
            }

            __instance.contextInfoLabel.text = stringBuilder.ToString();
            return false;
        }
    }
}