using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class ModTagButton : MonoBehaviour
    {
        public string ID;

        public void Init(string id)
        {
            ID = id;
            base.GetComponent<Button>().onClick.AddListener(PressButton);
        }

        public void PressButton()
        {
            ModBotUIRoot.Instance.ModCreationWindow.TrySelectTag(ID);

            string hex = ModBotUIRoot.Instance.ModCreationWindow.HasSelectedTag(ID) ? "#73ADFF" : "#808080";
            Color col;
            ColorUtility.TryParseHtmlString(hex, out col);
            base.GetComponent<Image>().color = col;
        }
    }
}