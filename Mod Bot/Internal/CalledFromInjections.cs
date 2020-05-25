using InternalModBot;
using ModLibrary;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using ModLibrary.Properties;

namespace InternalModBot
{
    /// <summary>
    /// Contains methods that get called from the game itself
    /// </summary>
    public static class CalledFromInjections
    {
        /// <summary>
        /// Called from <see cref="UnityEngine.Resources.Load(string)"/>, <see cref="UnityEngine.Resources.Load{T}(string)"/> and <see cref="ResourceRequest.asset"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object FromResourcesLoad(string path)
        {
            UnityEngine.Object levelEditorObject = LevelEditorObjectAdder.GetObjectData(path);

            if (levelEditorObject != null)
                return levelEditorObject;

            if (ModsManager.Instance == null)
                return null;

            return ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
        }
    }
}
