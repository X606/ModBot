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
	public class ModBotSignInUI : MonoBehaviour
	{
		public InputField UsernameField;
		public InputField PasswordField;
		public Button SignUpButton;
		public Button SignInButton;
		public Text ErrorText;
		public Button XButton;
		public GameObject SignInFormGameObject;

		public void Init(ModdedObject moddedObject)
		{
			UsernameField = moddedObject.GetObject<InputField>(0);
			PasswordField = moddedObject.GetObject<InputField>(1);
			SignUpButton = moddedObject.GetObject<Button>(2);
			SignInButton = moddedObject.GetObject<Button>(3);
			ErrorText = moddedObject.GetObject<Text>(4);
			XButton = moddedObject.GetObject<Button>(5);
			
			SignInFormGameObject = moddedObject.gameObject;
		}

		readonly string _sessionIdFilePath = Application.persistentDataPath + "/SessionID.txt";
			
		void Start()
		{
			if (File.Exists(_sessionIdFilePath))
			{
				string sessionId = File.ReadAllText(_sessionIdFilePath);
				API.SetSessionID(sessionId);

				API.IsValidSession(sessionId, delegate (string data)
				{
					if(data == "false")
					{
						API.SetSessionID("");

						File.Delete(_sessionIdFilePath);

						VersionLabelManager.Instance.SetLine(2, "Not signed in");

						OpenSignInForm();
						return;
					}

					onSignedIn();

				});
			} else
			{
				VersionLabelManager.Instance.SetLine(2, "Not signed in");

				OpenSignInForm();
			}

			SignUpButton.onClick.AddListener(new UnityAction(onSignUpButtonClicked));
			SignInButton.onClick.AddListener(new UnityAction(onSignInButtonClicked));

			XButton.onClick.AddListener(new UnityAction(onCloseButton));
		}

		public void SetSession(string sessionId)
		{
			API.SetSessionID(sessionId);

			File.WriteAllText(_sessionIdFilePath, sessionId);
		}

		public void OpenSignInForm()
		{
			UsernameField.text = "";
			PasswordField.text = "";

			ErrorText.text = "";

			SignInButton.gameObject.SetActive(true);
			SignUpButton.gameObject.SetActive(true);
			XButton.gameObject.SetActive(true);

			SignInFormGameObject.SetActive(true);
		}

		void onSignUpButtonClicked()
		{
			System.Diagnostics.Process.Start("https://modbot.org/");
		}
		void onSignInButtonClicked()
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
				SetSession(sessionID);

				onCloseButton();

				onSignedIn();
			});
		}

		void onCloseButton()
		{
			SignInFormGameObject.SetActive(false);
		}

		void onSignedIn()
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
