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
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                counter.gameObject.SetActive(!counter.gameObject.activeSelf);
            }

            float FPS = 1f / Time.unscaledDeltaTime;
            int FPSInt = Convert.ToInt32(FPS);

            counter.text = FPSInt + " FPS";
        }

        /// <summary>
        /// The text that displays the numbers
        /// </summary>
        public Text counter;
    }
}
