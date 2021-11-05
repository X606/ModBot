using ModLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// The generic message box UI
    /// </summary>
    internal class GenericMessageBoxUI : MonoBehaviour
	{
		Text _messageLabel;
        RectTransform _buttonsRoot;
		GameObject _uiRoot;

		PooledPrefab _buttonPool;

		public void Hide()
        {
			_uiRoot.SetActive(false);

			GameUIRoot.Instance.RefreshCursorEnabled();
        }

		public void Show(SimpleMessageBox messageBox)
        {
			_uiRoot.SetActive(true);
			ClearButtons();

			SetMessageText(messageBox.Message);
			SetButtons(messageBox.Buttons);

			GameUIRoot.Instance.RefreshCursorEnabled();
		}

		public void SetButtons(List<MessageBoxButton> buttons)
        {
            foreach (MessageBoxButton button in buttons)
			{
				Transform buttonTransform = _buttonPool.InstantiateNewObject();
				buttonTransform.SetParent(_buttonsRoot);

				GenericButton genericButton = buttonTransform.GetComponent<GenericButton>();
				if (genericButton == null)
                {
					genericButton = buttonTransform.gameObject.AddComponent<GenericButton>();
                }

				genericButton.ButtonTextLabel.text = button.ButtonText;
				genericButton.ButtonImage.color = button.ButtonColor;

                genericButton.Button.onClick.RemoveAllListeners();
                genericButton.Button.onClick.AddListener(button.OnPressed);
			}
		}

		public void SetMessageText(string message)
		{
			_messageLabel.text = message;
		}

		public void ClearButtons()
        {
			_buttonPool.DestroyAllObjects();
        }

		/// <summary>
		/// Sets up the generic 2 button dialoge from a modded object
		/// </summary>
		/// <param name="moddedObject"></param>
		public void Init(ModdedObject moddedObject)
		{
			_messageLabel = moddedObject.GetObject<Text>(0);
			_buttonsRoot = moddedObject.GetObject<RectTransform>(1);

			_uiRoot = moddedObject.gameObject;

			_buttonPool = new PooledPrefab
            {
                Prefab = InternalAssetBundleReferences.ModBot.GetObject("GenericButton").transform,
                UnparentOnDisable = true,
                MaxCount = -1,
                AddPoolReference = false
			};
		}
    }
}
