using InternalModBot;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Describes information about a mod, most of this information will come from the ModInfo.json file in your mod
    /// </summary>
    [Serializable]
    public class ModInfo
    {
        /// <summary>
        /// Checks if all required fields are filled out
        /// </summary>
        /// <param name="errorMessage">If some field is not filled out, information about which field wasn't filled out. Otherwise null</param>
        /// <returns></returns>
        public bool AreAllEssentialFieldsAssigned(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                errorMessage = "ModInfo.json: DisplayName not assigned";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UniqueID))
            {
                errorMessage = "ModInfo.json: UniqueID not assigned";
                return false;
            }

            if (string.IsNullOrWhiteSpace(MainDLLFileName))
            {
                errorMessage = "ModInfo.json: MainDLLFileName not assigned";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Author))
            {
                errorMessage = "ModInfo.json: Author not assigned";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Fixes some fields if they are not properly assigned, should only really be used from mod-bot internals
        /// </summary>
        public void FixFieldValues()
        {
            if (!MainDLLFileName.EndsWith(".dll"))
                MainDLLFileName += ".dll";

            if (ModDependencies == null)
                ModDependencies = new string[0];
        }

        /// <summary>
        /// The name of the mod
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        public string DisplayName { get; internal set; }

        /// <summary>
        /// The uuid of the mod
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        public string UniqueID { get; internal set; }

        /// <summary>
        /// The name of the main dll of the mod
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        public string MainDLLFileName { get; internal set; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        public string Author { get; internal set; }

        /// <summary>
        /// The mod version
        /// </summary>
        [JsonRequired]
        [JsonProperty]
        public uint Version { get; internal set; }

        /// <summary>
        /// The name of the tumbnail image that goes with the mod
        /// </summary>
        [JsonProperty]
        public string ImageFileName { get; internal set; }

        /// <summary>
        /// A brief description of the mod
        /// </summary>
        [JsonProperty]
        public string Description { get; internal set; }

        /// <summary>
        /// The uuids of the dependecies of this mod
        /// </summary>
        [JsonProperty]
        public string[] ModDependencies { get; internal set; }

        /// <summary>
        /// The tags used on this mod
        /// </summary>
        [JsonProperty]
        public string[] Tags { get; internal set; }

        /// <summary>
        /// The complete path to this mods folder
        /// </summary>
        public string FolderPath { get; internal set; }

        /// <summary>
        /// The complete path to the main dll file of the mod
        /// </summary>
        public string DLLPath => FolderPath + MainDLLFileName;

        /// <summary>
        /// True if this mod has an image provided, false otherwise
        /// </summary>
        public bool HasImage => !string.IsNullOrWhiteSpace(ImageFileName);

        private Texture2D _cachedImage;

        internal string MarkModAboutToDelete()
        {
            ModDeletionInfo.SaveModDeletionInfo(this);
            return MOD_ABOUT_TO_DELETE_FLAG_STRING;
        }

        internal bool ModIsAboutToDelete()
        {
            return ModDeletionInfo.HasID(UniqueID);
        }

        internal void OnModDeleted()
        {
            if (ModDeletionInfo.HasID(UniqueID))
            {
                ModDeletionInfo.RemoveID(UniqueID);
            }
        }

        /// <summary>
        /// The image to be displayed in the mods menu
        /// </summary>
        public Texture2D CachedImage
        {
            get
            {
                if (!_cachedImage || _cachedImage == null)
                {
                    if (!string.IsNullOrEmpty(ImageFileName))
                    {
                        string imageFilePath = FolderPath + ImageFileName;
                        if (File.Exists(imageFilePath))
                        {
                            byte[] imgData = File.ReadAllBytes(imageFilePath);

                            _cachedImage = new Texture2D(1, 1)
                            {
                                name = $"{UniqueID}-icon"
                            };
                            _cachedImage.LoadImage(imgData);
                        }
                    }
                }

                return _cachedImage;
            }
        }

        /// <summary>
        /// NOTE: setting this value only sets the playerPrefs, use <see cref="LoadedModInfo.IsEnabled"/> to work with loaded mods
        /// </summary>
        internal bool IsModEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(UniqueID, 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(UniqueID, value ? 1 : 0);
            }
        }

        public const string MOD_ABOUT_TO_DELETE_FLAG_STRING = "modIsAboutToBecomeDeleted";
    }
}