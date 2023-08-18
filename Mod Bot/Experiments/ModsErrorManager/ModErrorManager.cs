using ModLibrary;
using System;

namespace InternalModBot
{
    internal static class ModErrorManager
    {
        /// <summary>
        /// Show <see cref="Generic2ButtonDialogeUI"/> window with described reason, why mod won't load
        /// </summary>
        /// <param name="modErrorType"></param>
        /// <param name="info"></param>
        /// <param name="caughtException"></param>
        public static void ShowModBotException(ModErrorType modErrorType, ModInfo info, Exception caughtException)
        {
            string errorString = getErrorTypeString(modErrorType) + "\n" + caughtException.ToString() + "\n\nWould you like to continue or disable the mod?";
            _ = new Generic2ButtonDialogue(errorString,
                "Continue with errors",
                null,
                "Disable mod",
                delegate
                {
                    info.IsModEnabled = false;
                    if (ModBotUIRoot.Instance.ModsWindow.WindowObject.activeSelf)
                    {
                        ModsPanelManager.Instance.ReloadModItems();
                    }
                },
                Generic2ButtonDialogeUI.ModErrorSizeDelta);
        }

        public static void ShowModBotSiteError(string error) // Todo: make some more params{
        {
            _ = new Generic2ButtonDialogue(error, "Ok", null, "Visit Website", ModBotUIRootNew.DownloadWindow.OpenWebsite);
        }

        private static string getErrorTypeString(ModErrorType errorType)
        {
            string errorText;
            switch (errorType)
            {
                case ModErrorType.IOError:
                    errorText = "Caught exception while processing mod files";
                    break;
                case ModErrorType.HarmonyError:
                    errorText = "Caught exception while appling patches";
                    break;
                case ModErrorType.LoadError:
                    errorText = "Caught exception while loading mod";
                    break;
                default:
                    errorText = "Caught exception in mod";
                    break;
            }
            return errorText;
        }
    }
}