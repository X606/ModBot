using System;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	public class FPSCounterUI : MonoBehaviour
	{
		public Text FpsCounter;

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F3))
				FpsCounter.gameObject.SetActive(!FpsCounter.gameObject.activeSelf);

			float FPS = 1f / Time.unscaledDeltaTime;
			int FPSInt = Convert.ToInt32(FPS);

			FpsCounter.text = ModBotLocalizationManager.FormatLocalizedStringFromID("fps_label", FPSInt);
		}

		public void Init(Text text)
		{
			FpsCounter = text;
		}
	}

}
