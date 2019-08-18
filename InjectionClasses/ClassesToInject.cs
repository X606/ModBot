using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using TwitchChatter;
using UnityEngine;
using UnityEngine.UI;

namespace InjectionClasses
{
    public class ModdedObject : MonoBehaviour
    {
        public string ID;

        public List<UnityEngine.Object> objects;
    }
    
    public class XButton : MonoBehaviour
    {
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}