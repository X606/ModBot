using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Handles the fps counter in the corner
    /// </summary>
    internal class FPSCounterUI : MonoBehaviour
    {
        public const bool USE_GAME_MEASUREMENTS = true;
        public const bool SHOW_FPS_ON_START = true;

        private static bool _visible = SHOW_FPS_ON_START;

        private Text _fpsCounter;
        private float _timeToRefresh;

        private int _currentFPS;
        private int _maxFPS;

        private AdaptivePerformanceManager _adaptivePerformanceManager;

        private bool _isExperimentalBranch;

        private void Start()
        {
            _isExperimentalBranch = ExperimentalBranchManager.Instance.IsExperimentalBranch;
            _adaptivePerformanceManager = AdaptivePerformanceManager.Instance;
            _adaptivePerformanceManager._fpsNextMeasureTime = Time.realtimeSinceStartup; // fix fps counter *adapting* too long after playing a while

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UILanguageChanged, ForceRefreshNextFrame);
        }

        private void Update()
        {
            if (USE_GAME_MEASUREMENTS && !_isExperimentalBranch) _adaptivePerformanceManager.updateFPSMeasurement();
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(ModBotPrefs.GetKeyCode(ModBotInputType.ToggleFPSLabel)))
            {
                _visible = !_visible;
                RefreshVisibility();
            }

            int fps;
            if (USE_GAME_MEASUREMENTS)
            {
                fps = _adaptivePerformanceManager._fpsCurrentAverageFps; // this one seems more accurate
                if (fps == _currentFPS) return; // the number stays the same for the most time so refresh the text when the number changes
                _currentFPS = fps;
            }
            else
            {
                _timeToRefresh = Mathf.Max(_timeToRefresh - Time.unscaledDeltaTime, 0f);
                if (_timeToRefresh != 0f) return;
                _timeToRefresh = 0.5f;

                fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            }

            if (fps > _maxFPS) _maxFPS = fps;

            string text = ModBotLocalizationManager.FormatLocalizedStringFromID("fps_label", fps);
            if (ModBotPrefs.ShowMaxFPS)
            {
                _fpsCounter.text = $"{text} (Max: {_maxFPS})";
            }
            else
            {
                _fpsCounter.text = text;
            }
        }

        /// <summary>
        /// Sets up the FPS conter
        /// </summary>
        /// <param name="text"></param>
        public void Init(Text text)
        {
            _fpsCounter = text;
            RefreshVisibility();
        }

        public void RefreshVisibility()
        {
            _fpsCounter.gameObject.SetActive(_visible);
        }

        public void ForceRefreshNextFrame()
        {
            _currentFPS = 0;
            _timeToRefresh = 0f;
        }
    }
}