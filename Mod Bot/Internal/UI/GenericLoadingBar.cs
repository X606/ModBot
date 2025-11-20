using UnityEngine;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
    public class GenericLoadingBar : MonoBehaviour
    {
        private const string DEFAULT_HEADER_TEXT = "Loading";

        private ModdedObject _moddedObject;
        private Text _label;
        private Slider _progressBar;

        internal void Init()
        {
            _moddedObject = base.GetComponent<ModdedObject>();
            _label = _moddedObject.GetObject<Text>(0);
            _progressBar = _moddedObject.GetObject<Slider>(1);
            SetActive(false);
        }

        public void SetActive(bool value)
        {
            base.gameObject.SetActive(value);
        }

        public void SetActive(string header, float progress)
        {
            base.gameObject.SetActive(true);
            SetHeaderText(header);
            SetProgress(progress);
        }

        public void SetProgress(float value)
        {
            if(_progressBar == null)
            {
                return;
            }
            _progressBar.value = value;
        }

        public void SetProgress(float current, float target)
        {
            SetProgress(current / target);
        }

        public void SetHeaderText(string text)
        {
            if (_label == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                _label.text = DEFAULT_HEADER_TEXT;
                return;
            }
            _label.text = text;
        }
    }
}