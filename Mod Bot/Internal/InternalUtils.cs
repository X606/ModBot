using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

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

			while(assemblyPath.Length > 0)
			{
				string possiblePath = assemblyPath + ModsManager.MOD_INFO_FILE_NAME;
				if (File.Exists(possiblePath))
				{
					return GetRelativePathFromFullPath(assemblyPath);
				}

				assemblyPath = GetSubdomain(assemblyPath);
				if(!Directory.Exists(assemblyPath))
					break;
			}

			return "Clone Drone in the Danger Zone_Data/";
		}
		
		/// <summary>
		/// Gets the path relative to the Clone drone folder of the mod that called the method this method was called from
		/// </summary>
		/// <returns></returns>
		public static string GetCallerModPath(int methodsAbove=1)
		{
			StackFrame frame = new StackFrame(1 + methodsAbove);
			MethodBase method = frame.GetMethod();
			Type type = method.DeclaringType;

			return GetModFolderRootFromAssemblyPath(type.Assembly.Location);
		}

		/// <summary>
		/// Gets the full path from a path relative to the clone drone folder
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public static string GetFullPathFromRelativePath(string relativePath)
		{
			if(!relativePath.EndsWith("/") && !relativePath.EndsWith("\\"))
				relativePath += "/";

			return GetSubdomain(Application.dataPath) + relativePath;
		}
		
		/// <summary>
		/// Gets the relative path from the clone drone folder from a full path
		/// </summary>
		/// <param name="fullPath"></param>
		/// <returns></returns>
		public static string GetRelativePathFromFullPath(string fullPath)
		{
			int lastIndex = fullPath.LastIndexOf(ModLibrary.AssetLoader.MODS_FOLDER_NAME);
			return fullPath.Substring(lastIndex);
		}
	}
}
