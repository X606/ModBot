using System;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class ProgressBar : MonoBehaviour
    {
        Image _image;

        public float Progress
        {
            get => _image.fillAmount;
            set => _image.fillAmount = value;
        }

        public void Initialize(Image image)
        {
            _image = image;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Progress = 0f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
