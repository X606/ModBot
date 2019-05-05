using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    static class ErrorChanger
    {
        public static void ChangeError()
        {
            GameUIRoot.Instance.ErrorWindow.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "Send us a screenshot over in the clone drone Mod-Bot discord so we can fix it!\n\nDO NOT SEND THIS TO THE REAL GAME DEVS, THIS IS PROBABLY A MODS FAULT AND NOT THEIRS.";
            
            GameUIRoot.Instance.ErrorWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "The Mod-Bot devs would love to see this!";
            
        }
    }
}
