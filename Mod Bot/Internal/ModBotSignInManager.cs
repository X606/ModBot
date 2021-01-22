using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ModBotWebsiteAPI;

namespace InternalModBot
{
	public class ModBotSignInManager : Singleton<ModBotSignInManager>
	{
		public GameObject SignInFormGameObject;
		public InputField UsernameField;
		public InputField PasswordField;
		public Button SignUpButton;
		public Button SignInButton;
		public Text ErrorText;
		public Button XButton;

		readonly string _sessionIdFilePath = Application.persistentDataPath + "/SessionID.txt";

		private string _sessionID = "";

		void Start()
		{
			if (File.Exists(_sessionIdFilePath))
			{
				string sessionId = File.ReadAllText(_sessionIdFilePath);
				_sessionID = sessionId;
				API.SetSessionID(sessionId);

				API.IsValidSession(sessionId, delegate (string data)
				{
					if(data == "false")
					{
						_sessionID = "";
						API.SetSessionID("");

						File.Delete(_sessionIdFilePath);

						VersionLabelManager.Instance.SetLine(2, "Not signed in");

						OpenSignInForm();
						return;
					}

					OnSignedIn();

				});
			} else
			{
				VersionLabelManager.Instance.SetLine(2, "Not signed in");

				OpenSignInForm();
			}

			//API.getcu

			SignUpButton.onClick.AddListener(new UnityAction(OnSignUpButtonClicked));
			SignInButton.onClick.AddListener(new UnityAction(OnSignInButtonClicked));

			XButton.onClick.AddListener(new UnityAction(OnCloseButton));
		}

		void setSession(string sessionId)
		{
			_sessionID = sessionId;
			API.SetSessionID(sessionId);

			File.WriteAllText(_sessionIdFilePath, sessionId);
		}

		void OpenSignInForm()
		{
			UsernameField.text = "";
			PasswordField.text = "";

			ErrorText.text = "";

			SignInFormGameObject.SetActive(true);
		}

		void OnSignUpButtonClicked()
		{
			System.Diagnostics.Process.Start("https://modbot.org/");
		}
		void OnSignInButtonClicked()
		{
			string playfabId = MultiplayerLoginManager.Instance.GetLocalPlayFabID();

			debug.Log(playfabId);

			SignInButton.gameObject.SetActive(false);
			SignUpButton.gameObject.SetActive(false);
			XButton.gameObject.SetActive(false);
			API.SignInFromGame(UsernameField.text, PasswordField.text, playfabId, delegate(JsonObject json)
			{
				string error = Convert.ToString(json["error"]);
				if (error != "" && error != "null")
				{
					ErrorText.text = error;
					SignInButton.gameObject.SetActive(true);
					SignUpButton.gameObject.SetActive(true);
					XButton.gameObject.SetActive(true);
					return;
				}
				if (Convert.ToString(json["isError"]) == "true")
				{
					ErrorText.text = "Unknown error";
					SignInButton.gameObject.SetActive(true);
					SignUpButton.gameObject.SetActive(true);
					return;
				}

				ErrorText.text = "";
				string sessionID = Convert.ToString(json["sessionID"]).Trim('\"');
				setSession(sessionID);

				OnCloseButton();

				OnSignedIn();
			});
		}

		void OnCloseButton()
		{
			SignInFormGameObject.SetActive(false);
		}

		void OnSignedIn()
		{
			API.GetCurrentUser(delegate (string userId)
			{
				API.GetUser(userId, delegate (JsonObject json)
				{
					string username = Convert.ToString(json["username"]).Trim('\"');

					username = "<color=" + Convert.ToString(json["color"]) +">" + username + "</color>";
					debug.Log("logged in as " + username.Trim('\"'));
					VersionLabelManager.Instance.SetLine(2, "Signed in as: " + username);
				});

			});
		}

	}
}
