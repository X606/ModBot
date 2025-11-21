using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;

namespace InternalModBot
{
    internal class ModLoadError
    {
        public ModLoadError(ModInfo modInfo, string errorMessage) : this(modInfo.FolderPath, modInfo.DisplayName, errorMessage)
        {
            Info = modInfo;
        }

        public ModLoadError(string folderPath, string errorMessage) : this(folderPath, folderPath.Split('/').Last(), errorMessage)
        {
        }

        public ModLoadError(string folderPath, string modName, string errorMessage)
        {
            FolderPath = folderPath;
            ModName = modName;
            ErrorMessage = errorMessage;
        }
		public ModLoadError(string errorMesage) : this("", "", errorMesage)
		{
		}

        public ModInfo Info;
        public string FolderPath;
        public string ModName;
        public string ErrorMessage;
    }
}