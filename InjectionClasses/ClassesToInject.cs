using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

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