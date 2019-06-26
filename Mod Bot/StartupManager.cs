using ModLibrary;
using UnityEngine;

using System;
using System.Collections.Generic;

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
            gameFlowManager.AddComponent<GameFlowManagerStartTest>();

            IgnoreCrashesManager.Start();
        }
    }

    public class GameFlowManagerStartTest : MonoBehaviour
    {
        private void Start()
        {
            hasInitialized = false;
        }

        private void Update()
        {
            if (!hasInitialized && GameFlowManager.Instance != null)
            {
                OnGameFlowManagerStart();
                hasInitialized = true;
            }
        }

        private void OnGameFlowManagerStart()
        {
            //Action<string> callback = new Action<string>(FakeActionTest);
            //callback("Test message");
        }

        private void FakeActionTestNoArguments()
        {
            debug.Log("Test message 3");
        }

        private void FakeActionTest(string message)
        {
            debug.Log(message);
        }

        private bool hasInitialized;
    }
}
