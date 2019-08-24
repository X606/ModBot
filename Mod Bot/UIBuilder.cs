using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ModLibrary
{
    public class UIBuilder
    {
        public UIBuilder(float size = 100)
        {
            GameObject prefab = AssetLoader.GetObjectFromFile("uibuilder", "Canvas", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(prefab).GetComponent<ModdedObject>();
            BaseCanvas = moddedObject.gameObject;
            moddedObject.GetObject<Button>(0).onClick.AddListener(Close);
            BasePanel = moddedObject.GetObject<GameObject>(1).GetComponent<Image>();

            RectTransform rectTransform = BasePanel.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size);
        }


        public void Close()
        {
            GameObject.Destroy(BaseCanvas);
        }
        public GameObject BaseCanvas;
        public Image BasePanel;
    }
}
