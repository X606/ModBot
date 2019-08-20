using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class FPSCount : MonoBehaviour
    {
        private void Start()
        {
            frames = new List<float>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                counter.gameObject.SetActive(!counter.gameObject.activeSelf);
            }
            float num = 1f / Time.unscaledDeltaTime;
            frames.Add(num);
            if (frames.Count > 10)
            {
                frames.RemoveAt(0);
            }
            float FPS = 0f;
            for (int i = 0; i < frames.Count; i++)
            {
                FPS += frames[i];
            }
            FPS /= frames.Count;
            counter.text = ((int)FPS).ToString();
        }

        public Text counter;

        private List<float> frames;
    }
}
