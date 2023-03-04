using UnityEngine;
using UnityEngine.UI;

namespace ModLibrary
{
    public class GenericLoadingBar : MonoBehaviour
    {
        private const string DEFAULT_HEADER_TEXT = "Loading";

        private ModdedObject m_ModdedObject;
        private Text m_Label;
        private Slider m_ProgressBar;

        internal GenericLoadingBar Init()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            m_Label = m_ModdedObject.GetObject_Alt<Text>(0);
            m_ProgressBar = m_ModdedObject.GetObject_Alt<Slider>(1);
            SetActive(false);
            return this;
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
            if(m_ProgressBar == null)
            {
                return;
            }
            m_ProgressBar.value = value;
        }

        public void SetProgress(float current, float target)
        {
            SetProgress(current / target);
        }

        public void SetHeaderText(string text)
        {
            if (m_Label == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                m_Label.text = DEFAULT_HEADER_TEXT;
                return;
            }
            m_Label.text = text;
        }
    }
}