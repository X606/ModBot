using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModLibrary;

namespace InternalModBot
{
    public static class StartupManager
    {
        public static void OnStartUp()
        {
            ErrorChanger.ChangeError();

            GameObject gameObject = GameFlowManager.Instance.gameObject;

            gameObject.AddComponent<WaitThenCallClass>();
            gameObject.AddComponent<moddedObjectsManager>();
            gameObject.AddComponent<ModsManager>();
            gameObject.AddComponent<UpdateChecker>();
            gameObject.AddComponent<ModsPanelManager>();
            gameObject.AddComponent<CustomUpgradeManger>();

            IgnoreCrashesManager.Start();
            
            
        }

    }
}
