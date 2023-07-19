using ModLibrary;
using System;
using UnityEngine;

namespace InternalModBot
{
    public class ModBotHUDRootNew : MonoBehaviour
    {
        private ModdedObject m_ModdedObject;

        private static ModBotHUDRootNew m_Instance;
        public static ModBotHUDRootNew HUDRoot => m_Instance;

        private static ModDownloadWindowNew m_DownloadWindow;
        public static ModDownloadWindowNew DownloadWindow => m_DownloadWindow;

        private static GenericLoadingBar m_GenericLoadingBar;
        public static GenericLoadingBar LoadingBar => m_GenericLoadingBar;

        internal void Init()
        {
            m_Instance = this;
            m_ModdedObject = base.GetComponent<ModdedObject>();
            if (!m_ModdedObject)
            {
                throw new NullReferenceException("No modded object found for Mod-Bot HUD Root");
            }

            Transform t2 = m_ModdedObject.GetObject_Alt<Transform>(13);
            m_GenericLoadingBar = t2.gameObject.AddComponent<GenericLoadingBar>().Init();

            Transform t1 = m_ModdedObject.GetObject_Alt<Transform>(12);
            m_DownloadWindow = t1.gameObject.AddComponent<ModDownloadWindowNew>().Init();
        }
    }
}
