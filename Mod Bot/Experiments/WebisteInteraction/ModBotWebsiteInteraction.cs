using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace InternalModBot
{
    internal static class ModBotWebsiteInteraction
    {
        public static void RequestAllModInfos(Action<UnityWebRequest> webRequestVariable, Action<ModsHolder?> downloadedData, Action<string> onCaughtError = null)
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(requestAllModInfos(delegate (UnityWebRequest r) { webRequestVariable(r); }, downloadedData, onCaughtError));
        }

        private static IEnumerator requestAllModInfos(Action<UnityWebRequest> webRequestVariable, Action<ModsHolder?> downloadedData, Action<string> onCaughtError = null)
        {
            using(UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getAllModInfos"))
            {
                webRequest.timeout = 9;
                webRequestVariable(webRequest);

                yield return StaticCoroutineRunner.StartStaticCoroutine(updateProgressOfAsyncOperation(webRequest.SendWebRequest()));

                if(webRequest.isNetworkError || webRequest.isHttpError)
                {
                    if(onCaughtError != null)
                    {
                        onCaughtError("Cannot load mods page. Error details: " + webRequest.error + "\nTry visiting the website");
                    }
                    yield break;
                }

                ModsHolder? modsHolder = JsonConvert.DeserializeObject<ModsHolder>(webRequest.downloadHandler.text);
                downloadedData(modsHolder);
            }
            yield break;
        }

        private static IEnumerator updateProgressOfAsyncOperation(UnityWebRequestAsyncOperation operation)
        {
            ModBotUIRootNew.LoadingBar.SetProgress(0f);
            while (!operation.isDone)
            {
                ModBotUIRootNew.LoadingBar.SetProgress(operation.progress);
                yield return null;
            }
            ModBotUIRootNew.LoadingBar.SetProgress(1f);
        }
    }
}
