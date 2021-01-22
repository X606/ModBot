using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

namespace ModBotWebsiteAPI
{
	public static partial class API
	{
		static void CreateAccount(string username, string password, Action<CreateAccountResponse> callback)
		{
			string json = JsonConvert.SerializeObject(new CreateAccountData
			{
				username=username,
				password=password
			});

			StaticCoroutineRunner.StartStaticCoroutine(HttpUtils.Post(MODBOT_WEBSITE_URL_PREFIX + "createAccout", json, delegate (string data)
			{
				callback(CreateAccountResponse.FromJson(data));
			}));

		}
	}

	[Serializable]
	class CreateAccountData
	{
		public string username;
		public string password;

		public bool IsValidRequest()
		{
			return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
		}
	}
	[Serializable]
	public class CreateAccountResponse
	{
		public string sessionID;
		public string error;
		public bool isError;

		internal static CreateAccountResponse FromJson(string json)
		{
			return JsonConvert.DeserializeObject<CreateAccountResponse>(json);
		}
	}

}
