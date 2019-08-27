using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            
            counter.text = ((int)FPS).ToString() + " FPS";
        }
        /// <summary>
        /// The text that displays the numbers
        /// </summary>
        public Text counter;
    }
}
