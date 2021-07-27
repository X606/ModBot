using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using UnityEngine.Networking;

namespace ModBotWebsiteAPI
{
	static class HttpUtils
	{
		static HttpUtils()
		{
			_httpClientHandler = new HttpClientHandler()
			{
				UseCookies = false
			};
			_httpClient = new HttpClient(_httpClientHandler);
		}
		static readonly HttpClientHandler _httpClientHandler;
		static readonly HttpClient _httpClient;

		static string sessionID = null;

		public static IEnumerator Get(string url, Action<string> callback)
		{
			using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
			{
				if (sessionID != null)
					unityWebRequest.SetRequestHeader("Cookie", "SessionID=" + sessionID);

				yield return unityWebRequest.SendWebRequest();

				callback(unityWebRequest.downloadHandler.text);
				yield return null;
			}
		}
		public static IEnumerator Post(string url, string body, Action<string> callback)
		{
			using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, body))
			{
				if (sessionID != null)
					unityWebRequest.SetRequestHeader("Cookie", "SessionID=" + sessionID);

				yield return unityWebRequest.SendWebRequest();

				callback(unityWebRequest.downloadHandler.text);
			}
		}

	}
}
