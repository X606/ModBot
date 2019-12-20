using ModLibrary;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to change the error on the error screen when the game crashes
    /// </summary>
    internal static class ErrorChanger
    {
        /// <summary>
        /// Changes the error on the crash screen so that it no longer says to tell doborog of crashes
        /// </summary>
        public static void ChangeError()
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                GameUIRoot.Instance.ErrorWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = LocalizationManager.Instance.GetTranslatedString("crashscreen_customdescription");

                GameUIRoot.Instance.ErrorWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = LocalizationManager.Instance.GetTranslatedString("crashscreen_customtitle");

            }, 0f); // We want a delay here because the LocalizationManager hasn't been initialized at this point in time

            /* NOTE: This causes us to not be able to connect to multiplayer servers
            string versionString = VersionNumberManager.Instance.GetVersionString();
            versionString += " - Modded Client";
            Accessor.SetPrivateField("_versionString", VersionNumberManager.Instance, versionString);
            */
        }
    }
}
