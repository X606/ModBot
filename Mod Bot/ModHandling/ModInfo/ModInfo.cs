using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    [Serializable]
    public class ModInfo
    {
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

        public void FixFieldValues()
        {
            if (!MainDLLFileName.EndsWith(".dll"))
                MainDLLFileName += ".dll";
        }

        [JsonRequired]
        public string DisplayName;

        [JsonRequired]
        public string UniqueID;

        [JsonRequired]
        public string MainDLLFileName;

        [JsonRequired]
        public string Author;

        [JsonRequired]
        public uint Version;

        public string ImageFileName;

        public string Description;

        public string[] ModDependencies;

        public string[] Tags;

        [JsonIgnore]
        public string FolderPath;

        public string DLLPath => FolderPath + MainDLLFileName;

        public bool HasImage => !string.IsNullOrWhiteSpace(ImageFileName);
    }
}
