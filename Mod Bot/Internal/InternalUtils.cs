using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InternalModBot
{
	/// <summary>
	/// 
	/// </summary>
	public class InternalUtils
	{
		/// <summary> </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetSubdomain(string path)
		{
			string[] subDomainsArray = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

			List<string> subDomainsList = new List<string>(subDomainsArray);
			subDomainsList.RemoveAt(subDomainsList.Count - 1);

			return subDomainsList.Join("/") + "/";
		}

		/// <summary>
		/// Gets the mod root folder from a specified assembly path
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <returns></returns>
		public static string GetModFolderRootFromAssemblyPath(string assemblyPath)
		{
			assemblyPath = GetSubdomain(assemblyPath); // remove the dll file name from the path

			while(true)
			{
				string possiblePath = assemblyPath + ModsManager.MOD_INFO_FILE_NAME;
				if (File.Exists(possiblePath))
				{
					int lastIndex = assemblyPath.LastIndexOf(ModLibrary.AssetLoader.MODS_FOLDER_NAME);

					assemblyPath = assemblyPath.Substring(lastIndex);

					return assemblyPath;
				}

				assemblyPath = GetSubdomain(assemblyPath);
				if(!Directory.Exists(assemblyPath))
					break;
			}

			return "Clone Drone in the Danger Zone_Data/";
		}
	}
}
