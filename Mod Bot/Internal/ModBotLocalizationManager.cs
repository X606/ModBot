using HarmonyLib;
using ModLibrary;
using ModLibrary.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    /// <summary>
    /// Handles localization of string added in Mod-Bot
    /// </summary>
    internal static class ModBotLocalizationManager
    {
        static Dictionary<string, string> _currentLocalizationDictionary = null;

        static Dictionary<string, string> _fallbackModdedUpgradeLocalization = new Dictionary<string, string>();

        static List<Tuple<string, string>> _localizedStringsToAdd = new List<Tuple<string, string>>();
        static bool _isAddStringsCoroutineRunning;

        const char ID_STRING_SEPARATOR = ':';

        const string LANGUAGE_ID_ENGLISH = "en";
        const string LANGUAGE_ID_ITALIAN = "it";
        const string LANGUAGE_ID_SPANISH_LATIN_AMERICA = "es-419"; // Currently normal Spanish, Google Translate does not support this variant of Spanish
        const string LANGUAGE_ID_RUSSIAN = "ru";
        const string LANGUAGE_ID_GERMAN = "de";
        const string LANGUAGE_ID_FRENCH = "fr";
        const string LANGUAGE_ID_SPANISH_SPAIN = "es-ES";
        const string LANGUAGE_ID_SIMPLIFIED_CHINESE = "zh-CN";
        const string LANGUAGE_ID_BRAZILIAN_PORTUGUESE = "pt-BR"; // Currently normal Portuguese, Google Translate does not support Brazilian Portuguese
        const string LANGUAGE_ID_JAPANESE = "ja";
        const string LANGUAGE_ID_KOREAN = "ko";

        static Queue<string> _localizationIDsToLog;
        static bool _isLogQueueCoroutineRunning;

        static string currentLanguageID
        {
            get
            {
                if (SettingsManager.Instance != null && SettingsManager.Instance.GetPrivateField<SettingsData>("_data") != null)
                {
                    return SettingsManager.Instance.GetCurrentLanguageID();
                }
                else
                {
                    return LANGUAGE_ID_ENGLISH; // Default to english if game is not fully initialized
                }
            }
        }

        static string getLocalizationFileContentsForCurrentLanguage()
        {
            switch (currentLanguageID)
            {
                case LANGUAGE_ID_ENGLISH:
                    return Resources.ModBot_English;
                case LANGUAGE_ID_ITALIAN:
                    return Resources.ModBot_Italian;
                case LANGUAGE_ID_SPANISH_LATIN_AMERICA:
                    return Resources.ModBot_Spanish_LatinAmerica;
                case LANGUAGE_ID_RUSSIAN:
                    return Resources.ModBot_Russian;
                case LANGUAGE_ID_GERMAN:
                    return Resources.ModBot_German;
                case LANGUAGE_ID_FRENCH:
                    return Resources.ModBot_French;
                case LANGUAGE_ID_SPANISH_SPAIN:
                    return Resources.ModBot_Spanish_Spain;
                case LANGUAGE_ID_SIMPLIFIED_CHINESE:
                    return Resources.ModBot_Simplified_Chinese;
                case LANGUAGE_ID_BRAZILIAN_PORTUGUESE:
                    return Resources.ModBot_Brazilian_Portuguese;
                case LANGUAGE_ID_JAPANESE:
                    return Resources.ModBot_Japanese;
                case LANGUAGE_ID_KOREAN:
                    return Resources.ModBot_Korean;
                default:
                    return Resources.ModBot_English; // if the language is unknown, just default to english
                    //throw new ArgumentException("Unknown language ID: " + languageID);
            }
        }

        static Dictionary<string, string> getLocalizationDictionaryForCurrentLanguage()
        {
            if (_currentLocalizationDictionary == null)
            {
                _currentLocalizationDictionary = new Dictionary<string, string>();

                string[] translations = getLocalizationFileContentsForCurrentLanguage().Split('\n');
                foreach (string translationLine in translations)
                {
                    if (string.IsNullOrWhiteSpace(translationLine))
                        continue;

                    string[] splitLine = translationLine.Split(new char[] { ID_STRING_SEPARATOR }, 2, StringSplitOptions.RemoveEmptyEntries); // This will split the string by the first occurence of the ID_STRING_SEPARATOR character, just in case there are some in the translated string

                    if (splitLine.Length != 2)
                        continue;

                    string id = splitLine[0];
                    string translatedText = splitLine[1].Replace("\\n", "\n"); // Replace "\n" with an actual newline

                    _currentLocalizationDictionary.Add(id, translatedText);
                }
            }

            return _currentLocalizationDictionary;
        }

        /// <summary>
        /// Gets the translated string via <see cref="LocalizationManager.GetTranslatedString(string, int)"/> and formats the returned <see langword="string"/> with the given arguments
        /// </summary>
        /// <param name="ID">The localization ID to get the translated string of</param>
        /// <param name="arguments">The arguments to format into the string</param>
        /// <returns>The translated and formatted string</returns>
        public static string FormatLocalizedStringFromID(string ID, params object[] arguments)
        {
            return string.Format(LocalizationManager.Instance.GetTranslatedString(ID), arguments);
        }

        public static void OnLocalizationDictionaryUpdated()
        {
            _currentLocalizationDictionary = null;
        }

        /// <summary>
        /// Adds all Mod-Bot localization IDs and translated text for the current language into the given dictionary
        /// </summary>
        /// <param name="languageDictionary"></param>
        public static void AddAllLocalizationStringsToDictionary(Dictionary<string, string> languageDictionary)
        {
            if (languageDictionary == null)
                throw new ArgumentNullException(nameof(languageDictionary));

            Dictionary<string, string> modBotLocalizationDictionary = getLocalizationDictionaryForCurrentLanguage();
            foreach (KeyValuePair<string, string> localizedPair in modBotLocalizationDictionary)
            {
                languageDictionary.Add(localizedPair.Key, localizedPair.Value);
            }

            foreach (KeyValuePair<string, string> localizationPair in _fallbackModdedUpgradeLocalization)
            {
                if (!languageDictionary.ContainsKey(localizationPair.Key))
                    languageDictionary.Add(localizationPair.Key, localizationPair.Value);
            }
        }

        internal static void TryAddModdedUpgradeLocalizationStringToDictionary(string ID, string text)
        {
            if (!_fallbackModdedUpgradeLocalization.ContainsKey(ID))
                _fallbackModdedUpgradeLocalization.Add(ID, text);

            TryAddLocalizationStringToDictionary(ID, text);
        }

        internal static void TryAddLocalizationStringToDictionary(string ID, string text)
        {
            if (!LocalizationManager.Instance.IsInitialized())
            {
                if (!_isAddStringsCoroutineRunning)
                    StaticCoroutineRunner.StartStaticCoroutine(waitForLocalizationManagerThenAddAllStrings());

                _localizedStringsToAdd.Add(new Tuple<string, string>(ID, text));
                return;
            }

            Dictionary<string, string> localizationDictionary = LocalizationManager.Instance.GetPrivateField<Dictionary<string, string>>("_translatedStringsDictionary");

            if (!localizationDictionary.ContainsKey(ID))
                localizationDictionary.Add(ID, text);
        }

        /// <summary>
        /// Passes the output of <see cref="LocalizationManager.GetTranslatedString(string, int)"/> into <see cref="debug.Log(string)"/> once the <see cref="LocalizationManager"/> is initialized
        /// </summary>
        /// <param name="localizationID"></param>
        public static void LogLocalizedStringOnceLocalizationManagerInitialized(string localizationID)
        {
            if (LocalizationManager.Instance.IsInitialized())
            {
                debug.Log(LocalizationManager.Instance.GetTranslatedString(localizationID));
                return;
            }

            if (_localizationIDsToLog == null)
                _localizationIDsToLog = new Queue<string>();

            _localizationIDsToLog.Enqueue(localizationID);

            if (!_isLogQueueCoroutineRunning)
                StaticCoroutineRunner.StartStaticCoroutine(waitForLocalizationManagerThenLogQueue());
        }

        static IEnumerator waitForLocalizationManagerThenLogQueue()
        {
            _isLogQueueCoroutineRunning = true;

            yield return new UnityEngine.WaitUntil(LocalizationManager.Instance.IsInitialized);
            while(_localizationIDsToLog.Count > 0)
            {
                string text = LocalizationManager.Instance.GetTranslatedString(_localizationIDsToLog.Dequeue());
                debug.Log(text);
            }

            _isLogQueueCoroutineRunning = false;
        }

        static IEnumerator waitForLocalizationManagerThenAddAllStrings()
        {
            _isAddStringsCoroutineRunning = true;

            yield return new UnityEngine.WaitUntil(LocalizationManager.Instance.IsInitialized);

            Dictionary<string, string> localizationDictionary = LocalizationManager.Instance.GetPrivateField<Dictionary<string, string>>("_translatedStringsDictionary");
            foreach (Tuple<string, string> idAndLocalizedString in _localizedStringsToAdd)
            {
                if (localizationDictionary.ContainsKey(idAndLocalizedString.Item1))
                    continue;

                localizationDictionary.Add(idAndLocalizedString.Item1, idAndLocalizedString.Item2);
            }

            _localizedStringsToAdd.Clear();
            _isAddStringsCoroutineRunning = false;
        }

        internal static string GetLocalizedModBotString(string localizationID)
        {
            if (LocalizationManager.Instance != null && LocalizationManager.Instance.IsInitialized())
            {
                return LocalizationManager.Instance.GetTranslatedString(localizationID);
            }
            else
            {
                Dictionary<string, string> modBotLocalizationDictionary = getLocalizationDictionaryForCurrentLanguage();
                if (modBotLocalizationDictionary.TryGetValue(localizationID, out string localizedString))
                {
                    return localizedString;
                }
                else
                {
                    return "[nl: " + localizationID + "]";
                }
            }
        }

        [HarmonyPatch]
        static class Patches
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(LocalizationManager), "populateDictionaryForCurrentLanguage")]
            static void LocalizationManager_populateDictionaryForCurrentLanguage_Postfix(Dictionary<string, string> ____translatedStringsDictionary)
            {
                OnLocalizationDictionaryUpdated();
                AddAllLocalizationStringsToDictionary(____translatedStringsDictionary);

                if (ModsManager.Instance != null && ModsManager.Instance.PassOnMod != null)
                {
                    ModsManager.Instance.PassOnMod.OnLanguageChanged(currentLanguageID, ____translatedStringsDictionary);
                }
            }
        }
    }
}
