using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary
{
    public class UIBuilder
    {
        public UIBuilder()
        {
            GameObject prefab = AssetLoader.GetObjectFromFile("uibuilder", "Canvas", "Clone Drone in the Danger Zone_Data/");
            BaseObject = (GameObject)GameObject.Instantiate(prefab).GetComponent<moddedObject>().objects[0];
        }

        public GameObject BaseObject;
    }
}
