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
        /// The animator of the console
        /// </summary>
        public Animator Animator;
        GameObject _consoleTextElementPrefab;
        GameObject _content;
        GameObject _innerHolder;
        InputField _input;
        ScrollRect _scroll;

        /// <summary>
        /// The amount of lines we should allow in the console before we start removing lines
        /// </summary>
        public const int MAX_LINES_COUNT = 100;

        Queue<Text> _lines = new Queue<Text>();

        bool _isInitialized = false;

        bool _isShownOnScreen = false;

        /// <summary>
        /// Initialized the <see cref="ConsoleUI"/>
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="content"></param>
        /// <param name="innerHolder"></param>
        /// <param name="input"></param>
        public void Init(Animator animator, GameObject content, GameObject innerHolder, InputField input)
        {
            Animator = animator;
            _content = content;
            _innerHolder = innerHolder;
            _input = input;
            _scroll = innerHolder.GetComponentInChildren<ScrollRect>();

            _consoleTextElementPrefab = InternalAssetBundleReferences.ModBot.GetObject("ConsoleTextElement");

            _input.onEndEdit.AddListener(OnEndEdit);

            _isInitialized = true;
        }

		private void OnEndEdit(string arg0)
		{
            // If the console is not up, dont run any commands
            if (!_innerHolder.activeSelf)
                return;

            // If the edit ended because we clicked away, don't do anything extra
            if (!Input.GetKeyDown(KeyCode.Return))
                return;

            RunCommand(_input.text);
            _input.text = "";
        }

        void Update()
        {
            if (!_isInitialized)
                return;

            if (Input.GetKeyDown(ModBotInputManager.GetKeyCode(ModBotInputType.OpenConsole)))
				Flip();
        }

        internal void Flip()
        {
            if (_isShownOnScreen)
            {
                HideConsole();
                return;
            }

            ShowConsole();
        }

        internal void HideConsole()
        {
            Animator.Play("hideConsole");
            _isShownOnScreen = false;
            _input.DeactivateInputField();
        }

        internal void ShowConsole()
        {
            Animator.Play("showConsole");
            _isShownOnScreen = true;
        }

        /// <summary>
        /// Writes the specified text to the console
        /// </summary>
        /// <param name="whatToLog"></param>
        public void Log(string whatToLog)
        {
            log(whatToLog);

            Console.WriteLine(whatToLog);
        }

        void log(string whatToLog, string prefix = "", string postfix = "")
		{
            if (!_isInitialized)
                return;

            Text spawnedText = Instantiate(_consoleTextElementPrefab, _content.transform).GetComponent<Text>();
            _lines.Enqueue(spawnedText);

            spawnedText.text = whatToLog;

            Canvas.ForceUpdateCanvases();

            Stack<TagHolder> stack = new Stack<TagHolder>();

            for (int i = 0; i < spawnedText.cachedTextGenerator.lines.Count; i++)
            {
                int startIndex = spawnedText.cachedTextGenerator.lines[i].startCharIdx;
                int endIndex = (i == (spawnedText.cachedTextGenerator.lines.Count - 1)) ? whatToLog.Length
                    : spawnedText.cachedTextGenerator.lines[i + 1].startCharIdx;

                int length = endIndex - startIndex;
                string lineText = whatToLog.Substring(startIndex, length);

                string tagPrefix = "";
                Stack<TagHolder> tagsToOpen = new Stack<TagHolder>(stack);
                while (tagsToOpen.Count > 0)
                {
                    tagPrefix += tagsToOpen.Pop().GetStartTag();
                }

                for (int j = 0; j < lineText.Length; j++)
				{
                    if (containsStringAt(j, lineText, "<color="))
					{
                        if ((j + "<color=".Length) < lineText.Length)
                        {
                            string value = lineText.Substring(j + "<color=".Length, "#ff00ff".Length);
                            stack.Push(new TagHolder(TagHolder.TagTypes.Color, false, value));
                        }
					}
                    else if(containsStringAt(j, lineText, "<b>"))
					{
                        stack.Push(new TagHolder(TagHolder.TagTypes.Bold, false, null));
                    }
                    else if (containsStringAt(j, lineText, "<i>"))
                    {
                        stack.Push(new TagHolder(TagHolder.TagTypes.Italics, false, null));
                    }
                    else if (containsStringAt(j, lineText, "</i>") || containsStringAt(j, lineText, "</b>") || containsStringAt(j, lineText, "</color>"))
					{
                        if (stack.Count > 0)
						{
                            stack.Pop();

                        }
                            
                    }
                }

                lineText = prefix + tagPrefix + lineText;

                Queue<TagHolder> tagsToClose = new Queue<TagHolder>(stack);
				while (tagsToClose.Count > 0)
				{
                    lineText += tagsToClose.Dequeue().GetEndTag();
				}
                lineText += postfix;

                lineText = lineText.Replace("\n", ""); // we are already splitting by newlines, no need to have the newline characters anymore

                if (i == 0)
                {
                    spawnedText.text = lineText;
                }
                else
                {
                    Text newLine = Instantiate(_consoleTextElementPrefab, _content.transform).GetComponent<Text>();
                    newLine.text = lineText;
                    _lines.Enqueue(newLine);
                }
            }

            while(_lines.Count > MAX_LINES_COUNT)
			{
                Destroy(_lines.Dequeue().gameObject);
            }

            DelegateScheduler.Instance.Schedule(delegate
            {
                _scroll.ScrollToBottom();
            }, -1f); // Run this next frame
        }

        /// <summary>
        /// Writes the specified text to the console, now in color!
        /// </summary>
        /// <param name="whatToLog"></param>
        /// <param name="color"></param>
        public void Log(string whatToLog, Color color)
        {
            string colorText = ColorUtility.ToHtmlStringRGB(color);
            log(whatToLog, "<color=#" + colorText + ">", "</color>");

            Console.WriteLine(whatToLog);
        }

        /// <summary>
        /// Gets called when the user types in a command into the input field and presses enter
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(string command)
        {
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

        bool containsStringAt(int index, string str, string substr)
		{
			for (int i = index; i < str.Length; i++)
			{
                int subStrIndex = i - index;

                if (substr.Length <= subStrIndex)
                    return true;

                if (str[i] != substr[subStrIndex])
                    return false;
            }

            return true;
		}

        class TagHolder
		{
            public TagHolder(TagTypes tagType, bool isEndTag, string content)
			{
                TagType = tagType;
                Content = content;
			}

            public TagTypes TagType;
            public string Content;

            public enum TagTypes
            {
                Color,
                Bold,
                Italics
            }

            public string GetStartTag()
            {
                switch (TagType)
                {
                    case TagTypes.Color:
                        return "<color=" + Content + ">";
                    case TagTypes.Bold:
                        return "<b>";
                    case TagTypes.Italics:
                        return "<i>";
                    default:
                        return "";
                }
            }
            public string GetEndTag()
			{
                switch (TagType)
                {
                    case TagTypes.Color:
                        return "</color>";
                    case TagTypes.Bold:
                        return "</b>";
                    case TagTypes.Italics:
                        return "</i>";
                    default:
                        return "";
                }
            }
        }
    }

    /// <summary>
    /// Adds a few extension methods to the <see cref="ScrollRect"/> class
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scrolls the <see cref="ScrollRect"/> to the top
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollRect"/> to the bottom
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}