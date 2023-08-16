﻿using System;
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
	/// <summary>
	/// Handles the UI for signing into mod-bot
	/// </summary>
	internal class ModBotSignInUI : MonoBehaviour
	{
		private static string _userName = string.Empty;
		public static string CurrentUserName { get => _userName; private set { _userName = value; } }

		public static bool HasSignedIn => !string.IsNullOrEmpty(CurrentUserName);

		InputField _usernameField;
		InputField _passwordField;
		Button _signUpButton;
		Button _signInButton;
		Text _errorText;
		Button _xButton;

		/// <summary>
		/// The base window object
		/// </summary>
		public GameObject WindowObject;

		/// <summary>
		/// Sets up the sign in UI
		/// </summary>
		/// <param name="moddedObject"></param>
		public void Init(ModdedObject moddedObject)
		{
			_usernameField = moddedObject.GetObject<InputField>(0);
			_passwordField = moddedObject.GetObject<InputField>(1);
			_signUpButton = moddedObject.GetObject<Button>(2);
			_signInButton = moddedObject.GetObject<Button>(3);
			_errorText = moddedObject.GetObject<Text>(4);
			_xButton = moddedObject.GetObject<Button>(5);
			
			WindowObject = moddedObject.gameObject;
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
						return;
					}

					onSignedIn();

				});
			} else
			{
				VersionLabelManager.Instance.SetLine(2, "Not signed in");
			}

			_signUpButton.onClick.AddListener(new UnityAction(onSignUpButtonClicked));
			_signInButton.onClick.AddListener(new UnityAction(onSignInButtonClicked));

			_xButton.onClick.AddListener(new UnityAction(onCloseButton));
		}

		/// <summary>
		/// Sets the current session in the API
		/// </summary>
		/// <param name="sessionId"></param>
		public void SetSession(string sessionId)
		{
			API.SetSessionID(sessionId);

			File.WriteAllText(_sessionIdFilePath, sessionId);
		}

		/// <summary>
		/// Opens the sign in form
		/// </summary>
		public void OpenSignInForm()
		{
			_usernameField.text = "";
			_passwordField.text = "";

			_errorText.text = "";

			_signInButton.gameObject.SetActive(true);
			_signUpButton.gameObject.SetActive(true);
			_xButton.gameObject.SetActive(true);

			WindowObject.SetActive(true);
		}

		void onSignUpButtonClicked()
		{
			System.Diagnostics.Process.Start("https://modbot.org/");
		}
		void onSignInButtonClicked()
		{
			_signInButton.gameObject.SetActive(false);
			_signUpButton.gameObject.SetActive(false);
			_xButton.gameObject.SetActive(false);
			signInFromGame();
		}

		void signInFromGame()
        {
			API.SignInFromGame(_usernameField.text, _passwordField.text, MultiplayerLoginManager.Instance.GetLocalPlayFabID(), onSignInInfoReceived);
		}

		void onSignInInfoReceived(JsonObject json)
        {
			string error;
			try
			{
				error = Convert.ToString(json["Error"]);
			}
			catch (NullReferenceException)
            {
				_errorText.text = "Could not connect to server";
				_signInButton.gameObject.SetActive(true);
				_signUpButton.gameObject.SetActive(true);
				_xButton.gameObject.SetActive(true);
				return;
            }

			if (error != "" && error != "null")
			{
				_errorText.text = error;
				_signInButton.gameObject.SetActive(true);
				_signUpButton.gameObject.SetActive(true);
				_xButton.gameObject.SetActive(true);
				return;
			}

			_errorText.text = "";
			string sessionID = Convert.ToString(json["sessionID"]).Trim('\"');
			SetSession(sessionID);

			onCloseButton();

			onSignedIn();
		}

		void onCloseButton()
		{
			WindowObject.SetActive(false);
		}

		void onSignedIn()
		{
			API.GetCurrentUser(delegate (string userId)
			{
				API.GetUser(userId, delegate (JsonObject json)
				{
					string username;
					try
					{
						username = Convert.ToString(json["username"]).Trim('\"');
						username = "<color=" + Convert.ToString(json["color"]) + ">" + username + "</color>";
					}
					catch (NullReferenceException)
                    {
						DelegateScheduler.Instance.Schedule(onSignedIn, 2f);
						return;
                    }

					debug.Log("logged in as " + username.Trim('\"'));
					VersionLabelManager.Instance.SetLine(2, "Signed in as: " + username);
					CurrentUserName = username;
                });
			});
		}
	}
}
