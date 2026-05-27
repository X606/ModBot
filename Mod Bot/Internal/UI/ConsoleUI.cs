using ModLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot as the low level level of the debug console system
    /// </summary>
    internal class ConsoleUI : MonoBehaviour
    {
        /// <summary>
        /// The amount of lines we should allow in the console before we start removing lines
        /// </summary>
        public const int MAX_LINES_COUNT = 200;

        const float ANIMATION_DURATION_SECONDS = 0.5f;

        const float EXTRA_HEIGHT = 10f;

        RectTransform _rectTransform;

        Transform _content;

        InputField _input;

        ScrollRect _scroll;

        InputField _consoleTextElementPrefab;

        List<TextLine> _lines;

        bool _isShown;
        float _showProgress;

        public void Init()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();

            _scroll = moddedObject.GetObject<ScrollRect>(0);
            _content = moddedObject.GetObject<Transform>(1);
            _input = moddedObject.GetObject<InputField>(2);
            _input.text = string.Empty;
            _input.onEndEdit.AddListener(OnEndEdit);

            _consoleTextElementPrefab = InternalAssetBundleReferences.ModBot.GetObject("ConsoleTextElement").GetComponent<InputField>();
            _rectTransform = transform as RectTransform;

            _lines = new List<TextLine>(MAX_LINES_COUNT);

            refreshPosition();

            gameObject.SetActive(false);
        }

        private void OnEndEdit(string arg0)
        {
            // If the console is not up, dont run any commands
            if (!_isShown)
                return;

            // If the edit ended because we clicked away, don't do anything extra
            if (!Input.GetKeyDown(KeyCode.Return))
                return;

            RunCommand(_input.text);
            _input.text = "";
        }

        void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            if (_isShown)
            {
                _showProgress = Mathf.Min(1f, _showProgress + (deltaTime / ANIMATION_DURATION_SECONDS));
            }
            else
            {
                _showProgress = Mathf.Max(0f, _showProgress - (deltaTime / ANIMATION_DURATION_SECONDS));
                if (_showProgress == 0f) gameObject.SetActive(false);
            }

            refreshPosition();
        }

        internal void Flip()
        {
            if (_isShown)
            {
                HideConsole();
                return;
            }
            ShowConsole();
        }

        internal void HideConsole()
        {
            _input.DeactivateInputField();
            _isShown = false;
        }

        internal void ShowConsole()
        {
            gameObject.SetActive(true);
            _isShown = true;

            scrollToBottomNextFrame();
        }

        public void Clear()
        {
            TransformUtils.DestroyAllChildren(_content);
            _lines.Clear();
        }

        /// <summary>
        /// Writes the specified text to the console
        /// </summary>
        /// <param name="whatToLog"></param>
        public void Log(string whatToLog)
        {
            Log(whatToLog, Color.white);
            Console.WriteLine(whatToLog);
        }

        /// <summary>
        /// Writes the specified text to the console, now in color!
        /// </summary>
        /// <param name="whatToLog"></param>
        /// <param name="color"></param>
        public void Log(string whatToLog, Color color)
        {
            while (_lines.Count + 1 > MAX_LINES_COUNT)
            {
                _lines[0].DestroyThis();
                _lines.RemoveAt(0);
            }

            InputField spawnedField = Instantiate(_consoleTextElementPrefab, _content.transform);
            TextLine textLine = spawnedField.gameObject.AddComponent<TextLine>();
            textLine.Init(spawnedField, whatToLog, color);
            _lines.Add(textLine);

            textLine.RefreshHeight();

            if (isActiveAndEnabled)
            {
                scrollToBottomNextFrame();
            }
        }

        /// <summary>
        /// Gets called when the user types in a command into the input field and presses enter
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(string command)
        {
            if(command == "clear")
            {
                Clear();
                return;
            }

            Log(command);
            try
            {
                ConsoleInputManager.OnCommandRan(command);
                ModsManager.Instance.PassOnMod.OnCommandRan(command);
            }
            catch (Exception ex)
            {
                Log(ModBotLocalizationManager.FormatLocalizedStringFromID("command_failed_message", command, ex.Message), Color.red);
                Log(ex.StackTrace, Color.red);
            }
        }

        private void scrollToBottomNextFrame()
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                _scroll.ScrollToBottom();
            }, -1f);
        }

        private void refreshPosition()
        {
            Vector2 anchoredPosition = _rectTransform.anchoredPosition;
            anchoredPosition.y = Mathf.LerpUnclamped(_rectTransform.sizeDelta.y + EXTRA_HEIGHT, 0f, EasingFunctions.OutBack(_showProgress));
            _rectTransform.anchoredPosition = anchoredPosition;
        }

        class TextLine : MonoBehaviour
        {
            InputField _inputField;

            RectTransform _rectTransform;

            public void Init(InputField field, string text, Color color)
            {
                _inputField = field;
                _inputField.text = text;
                _inputField.textComponent.color = color;
                _rectTransform = field.transform as RectTransform;
            }

            public void DestroyThis()
            {
                if (_inputField.caretRectTrans) Destroy(_inputField.caretRectTrans.gameObject); // caret does not get destroyed if input field does
                Destroy(gameObject);
            }

            public void RefreshHeight()
            {
                Vector2 size = _rectTransform.sizeDelta;
                size.y = _inputField.preferredHeight;
                _rectTransform.sizeDelta = size;
            }
        }
    }
}