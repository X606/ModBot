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
		static void DeleteComment(string targetModId, string targetCommentId, Action<DeleteCommentResponse> callback)
		{
			string json = JsonConvert.SerializeObject(new DeleteCommentRequest
			{
				targetModId = targetModId,
				targetCommentId = targetCommentId
			});

			StaticCoroutineRunner.StartStaticCoroutine(HttpUtils.Post(MODBOT_WEBSITE_URL_PREFIX + "deleteComment", json, delegate (string data)
			{
				callback(DeleteCommentResponse.FromJson(data));
			}));

		}
	}

	[Serializable]
	class DeleteCommentRequest
	{
		public string targetModId;
		public string targetCommentId;

		public bool IsValidRequest()
		{
			return !string.IsNullOrWhiteSpace(targetModId);
		}
	}
	[Serializable]
	public class DeleteCommentResponse
	{
		public string message;
		public bool isError;

		public static DeleteCommentResponse FromJson(string json)
		{
			return JsonConvert.DeserializeObject<DeleteCommentResponse>(json);
		}
	}

}
