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
            GameUIRoot.Instance.ErrorWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "Send us a screenshot over in the Clone Drone Mod-Bot discord so we can fix it!\n\nDO NOT SEND THIS TO THE REAL GAME DEVS, THIS IS PROBABLY A MODS FAULT AND NOT THEIRS.";
            
            GameUIRoot.Instance.ErrorWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "The Mod-Bot devs would love to see this!";
        }
    }
}
