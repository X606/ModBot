using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Optimal way of getting camera instances
    /// </summary>
    public static class CameraManager
    {
        private static int s_lastFrameMainCameraWasRefreshedOn, s_lastFrameCurrentCameraWasRefreshedOn;

        private static Camera s_mainCamera, s_currentCamera;

        /// <summary>
        /// Instance of main camera if there's one
        /// </summary>
        public static Camera MainCamera
        {
            get
            {
                int frameCount = Time.frameCount;
                if (frameCount == s_lastFrameMainCameraWasRefreshedOn)
                {
                    return s_mainCamera;
                }

                s_lastFrameMainCameraWasRefreshedOn = frameCount;
                s_mainCamera = Camera.main;
                return s_mainCamera;
            }
        }

        /// <summary>
        /// Instance of current camera if there's one
        /// </summary>
        public static Camera CurrentCamera
        {
            get
            {
                int frameCount = Time.frameCount;
                if (frameCount == s_lastFrameCurrentCameraWasRefreshedOn)
                {
                    return s_currentCamera;
                }

                s_lastFrameCurrentCameraWasRefreshedOn = frameCount;
                s_currentCamera = Camera.current;
                return s_currentCamera;
            }
        }

        /// <summary>
        /// Force refresh main camera
        /// </summary>
        public static void ForceRefreshMainCamera()
        {
            s_lastFrameMainCameraWasRefreshedOn = Time.frameCount;
            s_mainCamera = Camera.main;
        }

        /// <summary>
        /// Force refresh current camera
        /// </summary>
        public static void ForceRefreshCurrentCamera()
        {
            s_lastFrameCurrentCameraWasRefreshedOn = Time.frameCount;
            s_currentCamera = Camera.current;
        }
    }
}