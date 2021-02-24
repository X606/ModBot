using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	/// <summary>
	/// Handles settings on mod-bot page of the settings
	/// </summary>
	public static class ModBotSettingsManager
	{
		static ModdedObject _settingsPageModdedObject;
		/// <summary>
		/// Sets up the <see cref="ModBotSettingsManager"/>
		/// </summary>
		/// <param name="moddedObject"></param>
		public static void Init(ModdedObject moddedObject)
		{
			_settingsPageModdedObject = moddedObject;

		}

		class ModBotSettingsBuilder
		{
			Transform _holder;

			GameObject _modBotOptionsLabelPrefab;
			GameObject _modBotOptionsLabelAndButton;
			GameObject _modBotOptionsSingleButton;

			public ModBotSettingsBuilder(ModdedObject moddedObject)
			{
				_holder = moddedObject.GetObject<GameObject>(0).transform;

				TransformUtils.DestroyAllChildren(_holder);

				_modBotOptionsLabelPrefab = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsLabel");
				_modBotOptionsLabelAndButton = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsLabelAndButton");
				_modBotOptionsSingleButton = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsSingleButton");

			}

			public void AddLabel(string label)
			{
				ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsLabelPrefab, _holder).GetComponent<ModdedObject>();
				moddedObject.GetObject<Text>(0).text = label;
			}
			public void AddLabelAndButton(string label, string buttonText, Color buttonColor, Action<Button> onClick)
			{
				ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsLabelAndButton, _holder).GetComponent<ModdedObject>();
				moddedObject.GetObject<Text>(0).text = label;
				Button button = moddedObject.GetObject<Button>(1);
				button.GetComponentInChildren<Text>().text = buttonText;
				button.GetComponent<Image>().color = buttonColor;
				button.onClick.AddListener(delegate { onClick?.Invoke(button); });
			}
			public void AddSingleButton(string buttonText, Color buttonColor, Action<Button> onClick)
			{
				ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsSingleButton, _holder).GetComponent<ModdedObject>();
				Button button = moddedObject.GetObject<Button>(0);
				button.GetComponentInChildren<Text>().text = buttonText;
				button.GetComponent<Image>().color = buttonColor;
				button.onClick.AddListener(delegate { onClick?.Invoke(button); });
			}

		}

		/// <summary>
		/// Populates the settings widow using the builder
		/// </summary>
		/// <param name="builder"></param>
		static void CreateSettingsWindow(ModBotSettingsBuilder builder)
		{
			builder.AddLabel("Controls");
			foreach (ModBotInputManager.InputOption inputOption in ModBotInputManager.InputOptions)
			{
				builder.AddLabelAndButton(inputOption.DisplayName, inputOption.Key.ToString(), new Color(0.3437611f, 0.5951038f, 0.9716981f), delegate(Button button)
				{
					StaticCoroutineRunner.StartStaticCoroutine(assignKeyFromNextInput(button, inputOption, 3f));
				});
			}
			//builder.AddLabelAndButton("Open Console", "F1", new Color(0.3437611f, 0.5951038f, 0.9716981f), null);
			//builder.AddLabelAndButton("Toggle FPS label", "F3", new Color(0.3437611f, 0.5951038f, 0.9716981f), null);
			
			builder.AddLabel("Website Integration");
			if (API.HasSession)
			{
				builder.AddSingleButton("Edit tags", Color.green, delegate
				{
					Process.Start("https://modbot.org/tagBrowsing.html");
				});
				builder.AddSingleButton("Sign out", Color.red, delegate (Button button)
				{
					debug.Log("Logging out...");
					API.SignOut(delegate (JsonObject json)
					{
						ModBotUIRoot.Instance.ModBotSignInUI.SetSession("");
						debug.Log(json["message"]);
						VersionLabelManager.Instance.SetLine(2, "Not signed in");
						CreateSettingsWindow(new ModBotSettingsBuilder(_settingsPageModdedObject));
					});
				});
			}
			else
			{
				builder.AddSingleButton("Sign in", Color.green, delegate
				{
					GameUIRoot.Instance.SettingsMenu.Hide();
					ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
				});
			}

		}

		static IEnumerator assignKeyFromNextInput(Button button, ModBotInputManager.InputOption input, float timeoutTime)
		{
			yield return new WaitForSecondsRealtime(0.1f); // wait a little so we dont pick up the mouse button

			Text buttonText = button.GetComponentInChildren<Text>();
			buttonText.text = "INPUT NEW KEY";

			float passedTime = 0;
			KeyCode[] allKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
			KeyCode? foundKey = null;
			while (foundKey == null && passedTime < timeoutTime)
			{
				for (int i = 0; i < allKeys.Length; i++)
				{
					if (Input.GetKeyDown(allKeys[i]))
					{
						foundKey = allKeys[i];
						break;
					}
				}
				passedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			if (foundKey == null) // did we time out?
			{
				buttonText.text = input.Key.ToString();
				yield break;
			}

			input.Key = foundKey.Value;

			buttonText.text = input.Key.ToString();
		}
		/// <summary>
		/// Gets called from the end of the <see cref="SettingsMenu"/>.populateSettings()
		/// </summary>
		[InjectionPostfixTarget(typeof(SettingsMenu), "populateSettings")]
		public static void OnPopulateSettings()
		{
			CreateSettingsWindow(new ModBotSettingsBuilder(_settingsPageModdedObject));
		}

	}


}
