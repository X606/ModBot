using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Handles the fps counter in the corner
    /// </summary>
    internal class FPSCounterUI : MonoBehaviour
    {
        public const bool SHOWMAXFPS = false; // maybe implement a setting
        public const bool SHOWFPSONSTART = true;

        Text _fpsCounter;
        float _timeToRefresh;

        int _maxFPS;

        void LateUpdate()
        {
            if (Input.GetKeyDown(ModBotInputManager.GetKeyCode(ModBotInputType.ToggleFPSLabel)))
                _fpsCounter.gameObject.SetActive(!_fpsCounter.gameObject.activeSelf);

            float time = Time.unscaledTime;
            if(time < _timeToRefresh)
            {
                return;
            }
            _timeToRefresh = time + 0.5f;

            float FPS = 1f / Time.unscaledDeltaTime;
            int FPSInt = Convert.ToInt32(FPS);

            string text = ModBotLocalizationManager.FormatLocalizedStringFromID("fps_label", FPSInt);
            if (SHOWMAXFPS)
            {
                if (FPSInt > _maxFPS) _maxFPS = FPSInt;
                _fpsCounter.text = text + "\nMax: " + _maxFPS;
            }
            else
            {
                _fpsCounter.text = text;
            }
        }

        /// <summary>
        /// Sets up the FPS conter
        /// </summary>
        /// <param name="text"></param>
        public void Init(Text text)
        {
            _fpsCounter = text;
            _fpsCounter.gameObject.SetActive(SHOWFPSONSTART);
        }
    }

}
