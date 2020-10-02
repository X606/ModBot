/*
 * This class is only used in the new mod loading system,
 * and since that system isn't done yet I am reverting back to the old system
 * 
 */

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    public class ModLoadError
    {
        public ModLoadError(ModInfo modInfo, string errorMessage) : this(modInfo.FolderPath, modInfo.DisplayName, errorMessage)
        {
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

        public string FolderPath;
        public string ModName;
        public string ErrorMessage;
    }
}
*/