using HarmonyLib;
using ModBotWebsiteAPI;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

namespace InternalModBot
{
    [HarmonyPatch(typeof(API))]
    static class ModBotWebsiteAPI_API_Patch // attempt to fix unity crash on exit
    {
        [HarmonyPostfix]
        [HarmonyPatch("SendRequest", new System.Type[] { typeof(string), typeof(string), typeof(Action<string>) })]
        static void SendRequest_Postfix1(ref IEnumerator __result, string ____sessionID, string url, string data, Action<string> callback)
        {
            __result = sendRequestCoroutine1(url, data, callback, ____sessionID);
        }

        static IEnumerator sendRequestCoroutine1(string url, string data, Action<string> callback, string sessionId)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data)),
                downloadHandler = new DownloadHandlerBuffer(),
                method = "POST"
            })
            {
                webRequest.timeout = 20;

                if (sessionId != null) webRequest.SetRequestHeader("Cookie", "SessionID=" + sessionId);

                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success) yield break;

                callback(webRequest.downloadHandler.text);
            }
            yield break;
        }

        [HarmonyPostfix]
        [HarmonyPatch("SendRequest", new System.Type[] { typeof(string), typeof(string), typeof(Action<JsonObject>) })]
        static void SendRequest_Postfix2(ref IEnumerator __result, string ____sessionID, string url, string data, Action<JsonObject> callback)
        {
            __result = sendRequestCoroutine2(url, data, callback, ____sessionID);
        }

        static IEnumerator sendRequestCoroutine2(string url, string data, Action<JsonObject> callback, string sessionId)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data)),
                downloadHandler = new DownloadHandlerBuffer(),
                method = "POST"
            })
            {
                webRequest.timeout = 20;

                if (sessionId != null) webRequest.SetRequestHeader("Cookie", "SessionID=" + sessionId);

                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success) yield break;

                callback(new JsonObject(webRequest.downloadHandler.text));
            }
            yield break;
        }
    }
}