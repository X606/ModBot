using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    public class ModLoadError
    {
        public ModLoadError(string folderPath, string errorMessage) : this(folderPath, folderPath.Split('/').Last(), errorMessage)
        {
        }

        public ModLoadError(string folderPath, string modName, string errorMessage)
        {
            FolderPath = folderPath;
            ModName = modName;
            ErrorMessage = errorMessage;
        }

        public string FolderPath;
        public string ModName;
        public string ErrorMessage;
    }
}
