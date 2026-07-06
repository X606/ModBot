using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Handles the UI for signing into mod-bot
    /// </summary>
    internal class ModBotSignInUI : MonoBehaviour
    {
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
            _signUpButton.onClick.AddListener(onSignUpButtonClicked);
            _signInButton = moddedObject.GetObject<Button>(3);
            _signInButton.onClick.AddListener(onSignInButtonClicked);
            _errorText = moddedObject.GetObject<Text>(4);
            _xButton = moddedObject.GetObject<Button>(5);
            _xButton.onClick.AddListener(onCloseButtonClicked);

            WindowObject = moddedObject.gameObject;

            makeInteractable();

            GlobalEventManager.Instance.AddEventListener(ModBotUserIdentifier.USER_SIGN_IN_ATTEMPT_EVENT, onSignInAttempt);
        }

        /// <summary>
        /// Opens the sign in form
        /// </summary>
        public void OpenSignInForm()
        {
            _usernameField.text = string.Empty;
            _passwordField.text = string.Empty;
            _errorText.text = string.Empty;
            WindowObject.SetActive(true);
            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        void makeNotInteractable()
        {
            _signInButton.gameObject.SetActive(false);
            _signUpButton.gameObject.SetActive(false);
            _xButton.gameObject.SetActive(false);
        }

        void makeInteractable()
        {
            _signInButton.gameObject.SetActive(true);
            _signUpButton.gameObject.SetActive(true);
            _xButton.gameObject.SetActive(true);
        }

        void onSignUpButtonClicked()
        {
            Application.OpenURL("https://modbot.org/");
        }

        void onSignInButtonClicked()
        {
            makeNotInteractable();
            ModBotUserIdentifier.Instance.TrySignInWithCredentials(_usernameField.text, _passwordField.text);
        }

        void onCloseButtonClicked()
        {
            WindowObject.SetActive(false);
            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        void onSignInAttempt()
        {
            makeInteractable();

            ModBotUserIdentifier userIdentifier = ModBotUserIdentifier.Instance;
            if (userIdentifier.HasFailedToSignIn())
            {
                _errorText.text = userIdentifier.GetSignInError();
            }
            else
            {
                onCloseButtonClicked();
                _errorText.text = string.Empty;
            }
        }
    }
}