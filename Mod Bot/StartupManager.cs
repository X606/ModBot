using ModLibrary;
using UnityEngine;

namespace InternalModBot
{
    public static class StartupManager
    {
        public static void OnStartUp()
        {
            ErrorChanger.ChangeError();

            GameObject gameFlowManager = GameFlowManager.Instance.gameObject;

            gameFlowManager.AddComponent<WaitThenCallClass>();
            gameFlowManager.AddComponent<moddedObjectsManager>();
            gameFlowManager.AddComponent<ModsManager>();
            gameFlowManager.AddComponent<UpdateChecker>();
            gameFlowManager.AddComponent<ModsPanelManager>();
            gameFlowManager.AddComponent<CustomUpgradeManger>();

            IgnoreCrashesManager.Start();
        }
    }
}
