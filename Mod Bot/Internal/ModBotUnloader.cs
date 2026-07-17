using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternalModBot
{
    // temp workaround for weird unity crashes on exit
    // unity 6 is sensitive to any loaded assemblies that weren't bundled with the game
    internal class ModBotUnloader : MonoBehaviour
    {
        private static bool s_hasAddedQuitHandler;

        private static bool s_isUnloadingGame;

        public static void AddQuitHandler()
        {
            if (s_hasAddedQuitHandler) return;

            Application.wantsToQuit += wantToQuit;

            s_hasAddedQuitHandler = true;
        }

        private static bool wantToQuit()
        {
            if (IsUnloadingGame()) // just for the case if the game doesn't unload for some reason, it should still be possible to close the game normally
            {
                return true;
            }

            new GameObject().AddComponent<ModBotUnloader>();
            return false;
        }

        private void Awake()
        {
            s_isUnloadingGame = true;
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

        public static bool IsUnloadingGame() => s_isUnloadingGame;
    }
}
