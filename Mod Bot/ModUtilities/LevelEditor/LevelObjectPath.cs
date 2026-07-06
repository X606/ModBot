namespace ModLibrary.LevelEditor
{
    /// <summary>
    /// Represents a path under Mods folder to an object
    /// </summary>
    public readonly struct LevelObjectPath
    {
        public readonly string ModName, FolderName, ObjectName;

        /// <summary>
        /// Path to an object
        /// </summary>
        /// <param name="modName">The name of the mod adding the object</param>
        /// <param name="objectName">The name of the object</param>
        public LevelObjectPath(string modName, string objectName)
        {
            ModName = modName;
            FolderName = null;
            ObjectName = objectName;
        }

        /// <summary>
        /// Path to an object
        /// </summary>
        /// <param name="modName">The name of the mod adding the object</param>
        /// <param name="folderName">The name of subfolder</param>
        /// <param name="objectName">The name of the object</param>
        public LevelObjectPath(string modName, string folderName, string objectName)
        {
            ModName = modName;
            FolderName = folderName;
            ObjectName = objectName;
        }

        /// <summary>
        /// Converts to [ModName]/[FolderName]/[ObjectName]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            bool isFolderNullOrEmpty = string.IsNullOrEmpty(FolderName);
            bool isModNullOrEmpty = string.IsNullOrEmpty(ModName);

            string modPath = isModNullOrEmpty ? string.Empty : ModName + "/";
            string folderPath = isFolderNullOrEmpty ? string.Empty : FolderName + "/";

            return modPath + folderPath + ObjectName;
        }
    }
}