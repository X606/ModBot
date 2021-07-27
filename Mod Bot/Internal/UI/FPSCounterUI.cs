using System;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	/// <summary>
	/// Handles the fps counter in the corner
	/// </summary>
	internal class FPSCounterUI : MonoBehaviour
	{
		Text _fpsCounter;

		void Update()
		{
			if (Input.GetKeyDown(ModBotInputManager.GetKeyCode(ModBotInputType.ToggleFPSLabel)))
				_fpsCounter.gameObject.SetActive(!_fpsCounter.gameObject.activeSelf);

			float FPS = 1f / Time.unscaledDeltaTime;
			int FPSInt = Convert.ToInt32(FPS);

			_fpsCounter.text = ModBotLocalizationManager.FormatLocalizedStringFromID("fps_label", FPSInt);
		}

		/// <summary>
		/// Sets up the FPS conter
		/// </summary>
		/// <param name="text"></param>
		public void Init(Text text)
		{
			_fpsCounter = text;
		}
	}

}
