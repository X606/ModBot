using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Controls the FPS counter in the corner of the screen
    /// </summary>
    public class FPSCount : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
                Counter.gameObject.SetActive(!Counter.gameObject.activeSelf);

            float FPS = 1f / Time.unscaledDeltaTime;
            int FPSInt = Convert.ToInt32(FPS);

            Counter.text = ModBotLocalizationManager.FormatLocalizedStringFromID("fps_label", FPSInt);
        }

        /// <summary>
        /// The text that displays the numbers
        /// </summary>
        internal Text Counter;
    }
}
