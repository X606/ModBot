using System;
using InternalModBot;

namespace ModLibrary
{
    public static class ExtensionMethods
    {
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (index < 0 || index >= moddedObject.objects.Count)
                return null;

            if (!(moddedObject.objects[index] is T))
            {
                throw new InvalidCastException("Object at index " + index + " was not of type " + typeof(T).ToString());
            }

            return moddedObject.objects[index] as T;
        }

        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.AddUpgradeIcon(upgradeDescription, url);
        }
    }
}
