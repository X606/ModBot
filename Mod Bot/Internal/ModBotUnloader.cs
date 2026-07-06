using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternalModBot
{
    // temp workaround for weird unity crashes on exit
    // idk why, but it seems that asset bundles cause this for some reason
    // setting the right unity version, re-saving prefabs didn't help
    // triggered by ModsManager

    internal class ModBotUnloader : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(unloadSceneThenQuit());
        }

        private IEnumerator unloadSceneThenQuit()
        {
            Scene exitScene = SceneManager.CreateScene("Exit"); // create empty scene and make it the main one
            SceneManager.SetActiveScene(exitScene);
            yield return SceneManager.UnloadSceneAsync("Gameplay"); // unload gameplay scene to trigger OnDisable and OnDestroy on MonoBehaviours
            yield return new WaitForEndOfFrame();
            Process.GetCurrentProcess().Kill();
            yield break;
        }
    }
}
