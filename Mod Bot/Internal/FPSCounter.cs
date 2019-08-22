using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
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

        public Text counter;
    }
}
