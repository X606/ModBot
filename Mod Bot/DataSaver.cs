// New mod loading system
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternalModBot;
using Newtonsoft.Json;

namespace ModLibrary
{
	/// <summary>
	/// Used to store data in the SaveData folder, make sure SaveData is set to a persistant folder in your mod data json file if you want the data to persist across mod updates
	/// </summary>
	public static class DataSaver
	{
		internal const string PRIMITIVES_SAVE_DATA_FILE_NAME = "Primitives.json";
		internal const string SAVE_DATA_FOLDER_NAME = "SaveData/";
		internal const string CUSTOM_TYPES_FOLDER_NAME = "CustomTypes/";

		static Dictionary<string, Dictionary<string, object>> _savedValues = new Dictionary<string, Dictionary<string, object>>();

		static void saveData(string key, object value)
		{
			string localPath = InternalUtils.GetCallerModPath(2);
			string modPathFullPath = InternalUtils.GetFullPathFromRelativePath(localPath) + SAVE_DATA_FOLDER_NAME;

			createFoldersIfNeeded(localPath);

			if (!_savedValues.ContainsKey(localPath))
				_savedValues.Add(localPath, new Dictionary<string, object>());

			_savedValues[localPath][key] = value;

			string json = JsonConvert.SerializeObject(_savedValues[localPath]);
			File.WriteAllText(modPathFullPath + PRIMITIVES_SAVE_DATA_FILE_NAME, json);
		}
		static object loadData(string key, Type type)
		{
			string localPath = InternalUtils.GetCallerModPath(2);

			createFoldersIfNeeded(localPath);

			if(!_savedValues.ContainsKey(localPath))
				throw new Exception("Could not find value with key \"" + key + "\"");

			if(!_savedValues[localPath].ContainsKey(key))
				throw new Exception("Could not find value with key \"" + key + "\"");

			object obj = _savedValues[localPath][key];

			#region type convertions becuse jsonConvert is big dumdum
			if(type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(byte) || type == typeof(ushort) || type == typeof(uint))
			{
				Type objectType = obj.GetType();
				long value;
				if(objectType == typeof(sbyte))
				{
					value = (sbyte)obj;
				}
				else if(objectType == typeof(short))
				{
					value = (short)obj;
				}
				else if(objectType == typeof(int))
				{
					value = (int)obj;
				}
				else if(objectType == typeof(long))
				{
					value = (long)obj;
				}
				else if(objectType == typeof(byte))
				{
					value = (byte)obj;
				}
				else if(objectType == typeof(ushort))
				{
					value = (ushort)obj;
				}
				else if(objectType == typeof(uint))
				{
					value = (uint)obj;
				}
				else
				{
					throw new Exception("The requested type was not of type \"" + type.Name + "\", but of type \"" + objectType.Name + "\"");
				}

				if(type == typeof(sbyte))
					return (object)(sbyte)value;

				if(type == typeof(short))
					return (object)(short)value;

				if(type == typeof(int))
					return (object)(int)value;

				if(type == typeof(long))
					return (object)value;

				if(type == typeof(byte))
					return (object)(byte)value;

				if(type == typeof(ushort))
					return (object)(ushort)value;

				if(type == typeof(uint))
					return (object)(uint)value;

			}
			if(type == typeof(ulong))
			{
				Type objectType = obj.GetType();
				
				if (objectType == typeof(long))
				{
					if(((long)obj) < 0)
						throw new InvalidCastException("The value was smaller than the min value of ulong");

					return (object)(ulong)(long)obj;
				}
				else if(objectType == typeof(ulong))
				{
					return (object)(ulong)obj;
				}

				throw new Exception("The requested type was not of type \"" + type.Name + "\", but of type \"" + objectType.Name + "\"");
			}

			if(type == typeof(float) || type == typeof(double) || type == typeof(decimal))
			{
				Type objectType = obj.GetType();
				double value;
				if(objectType == typeof(float))
				{
					value = (float)obj;
				}
				else if(objectType == typeof(double))
				{
					value = (double)obj;
				}
				else if(objectType == typeof(decimal))
				{
					value = (double)(decimal)obj;
				}
				else
				{
					throw new Exception("The requested type was not of type \"" + type.Name + "\", but of type \"" + objectType.Name + "\"");
				}

				if(type == typeof(float))
					return (object)(float)value;

				if(type == typeof(double))
					return (object)(double)value;

				if(type == typeof(decimal))
					return (object)(decimal)value;

			}

			if (type == typeof(char))
			{
				Type objectType = obj.GetType();
				if (objectType == typeof(string))
				{
					string str = (string)obj;
					if(str.Length != 1)
						throw new Exception("The requested type was not of type char");

					return (object)str[0];
				}
				else if (objectType == typeof(char))
				{
					return (object)(char)obj;
				}
				throw new Exception("The requested type was not of type \"" + type.Name + "\", but of type \"" + objectType.Name + "\"");
			}

			#endregion

			try
			{
				return obj;
			} catch(InvalidCastException e) {
				debug.Log(obj.GetType().Name + ", T: " + type.Name);
				throw e;
			}
		}

		/// <summary>
		/// Loads a byte from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A byte stored in the Primitives.json file</returns>
		public static byte LoadByte(string key)       => (byte)loadData(key, typeof(byte));
		/// <summary>
		/// Loads a ushort from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A short stored in the Primitives.json file</returns>
		public static ushort LoadUInt16(string key)   => (ushort)loadData(key, typeof(ushort));
		/// <summary>
		/// Loads a uint from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A uint stored in the Primitives.json file</returns>
		public static uint LoadUInt32(string key)     => (uint)loadData(key, typeof(uint));
		/// <summary>
		/// Loads a ulong from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A ulong stored in the Primitives.json file</returns>
		public static ulong LoadUInt64(string key)    => (ulong)loadData(key, typeof(ulong));
		/// <summary>
		/// Loads a sbyte from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A sbyte stored in the Primitives.json file</returns>
		public static sbyte LoadSByte(string key)     => (sbyte)loadData(key, typeof(sbyte));
		/// <summary>
		/// Loads a short from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A short stored in the Primitives.json file</returns>
		public static short LoadInt16(string key)     => (short)loadData(key, typeof(short));
		/// <summary>
		/// Loads a int from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A int stored in the Primitives.json file</returns>
		public static int LoadInt32(string key)       => (int)loadData(key, typeof(int));
		/// <summary>
		/// Loads a long from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A long stored in the Primitives.json file</returns>
		public static long LoadInt64(string key)      => (long)loadData(key, typeof(long));
		/// <summary>
		/// Loads a float from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A float stored in the Primitives.json file</returns>
		public static float LoadSingle(string key)    => (float)loadData(key, typeof(float));
		/// <summary>
		/// Loads a double from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A double stored in the Primitives.json file</returns>
		public static double LoadDouble(string key)   => (double)loadData(key, typeof(double));
		/// <summary>
		/// Loads a decimal from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A decimal stored in the Primitives.json file</returns>
		public static decimal LoadDecimal(string key) => (decimal)loadData(key, typeof(decimal));
		/// <summary>
		/// Loads a char from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A char stored in the Primitives.json file</returns>
		public static char LoadChar(string key)       => (char)loadData(key, typeof(char));
		/// <summary>
		/// Loads a string from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A string stored in the Primitives.json file</returns>
		public static string LoadString(string key)   => (string)loadData(key, typeof(string));
		/// <summary>
		/// Loads a bool from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A bool stored in the Primitives.json file</returns>
		public static bool LoadBool(string key)       => (bool)loadData(key, typeof(bool));
		/// <summary>
		/// Loads a enum from the Primitives.json file with the provided key
		/// </summary>
		/// <param name="key">The key associated with the value</param>
		/// <returns>A enum stored in the Primitives.json file</returns>
		public static TEnum LoadEnum<TEnum>(string key)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException("The passed type was not an enum");

			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));

			return (TEnum)loadData(key, underlyingType);
		}

		/// <summary>
		/// Saves a byte value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The byte you want to save</param>
		public static void SaveData(string key, byte value)    => saveData(key, value);
		/// <summary>
		/// Saves a ushort value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The ushort you want to save</param>
		public static void SaveData(string key, ushort value)  => saveData(key, value);
		/// <summary>
		/// Saves a uint value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The uint you want to save</param>
		public static void SaveData(string key, uint value)    => saveData(key, value);
		/// <summary>
		/// Saves a ulong value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The ulong you want to save</param>
		public static void SaveData(string key, ulong value)   => saveData(key, value);
		/// <summary>
		/// Saves a sbyte value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The sbyte you want to save</param>
		public static void SaveData(string key, sbyte value)   => saveData(key, value);
		/// <summary>
		/// Saves a short value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The short you want to save</param>
		public static void SaveData(string key, short value)   => saveData(key, value);
		/// <summary>
		/// Saves a int value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The int you want to save</param>
		public static void SaveData(string key, int value)     => saveData(key, value);
		/// <summary>
		/// Saves a long value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The long you want to save</param>
		public static void SaveData(string key, long value)    => saveData(key, value);
		/// <summary>
		/// Saves a float value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The float you want to save</param>
		public static void SaveData(string key, float value)   => saveData(key, value);
		/// <summary>
		/// Saves a double value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The double you want to save</param>
		public static void SaveData(string key, double value)  => saveData(key, value);
		/// <summary>
		/// Saves a decimal value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The decimal you want to save</param>
		public static void SaveData(string key, decimal value) => saveData(key, value);
		/// <summary>
		/// Saves a char value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The char you want to save</param>
		public static void SaveData(string key, char value)    => saveData(key, value);
		/// <summary>
		/// Saves a string value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The string you want to save</param>
		public static void SaveData(string key, string value)  => saveData(key, value);
		/// <summary>
		/// Saves a bool value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The bool you want to save</param>
		public static void SaveData(string key, bool value)    => saveData(key, value);
		/// <summary>
		/// Saves a enum value in the Primitives.json file, please remember that this calls File.WriteAllText so please dont call it in update
		/// </summary>
		/// <param name="key">The key you want associated with the value</param>
		/// <param name="value">The enum you want to save</param>
		public static void SaveEnum<TEnum>(string key, TEnum value)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException("The passed type was not an enum");

			Type type = Enum.GetUnderlyingType(typeof(TEnum));
			
			saveData(key, Convert.ChangeType(value, type));
		}

		/// <summary>
		/// Saves a custom type in the CustomTypes folder, please dont call this in update as it calls File.WriteAllText
		/// </summary>
		/// <typeparam name="Type">The type of the object you want to save</typeparam>
		/// <param name="key">The key you want associated with the value you want to save</param>
		/// <param name="value">The value you want to save</param>
		public static void SaveCustomType<Type>(string key, Type value)
		{
			string localPath = InternalUtils.GetCallerModPath();
			string saveDataFolderPath = InternalUtils.GetFullPathFromRelativePath(localPath) + SAVE_DATA_FOLDER_NAME + CUSTOM_TYPES_FOLDER_NAME;

			createFoldersIfNeeded(localPath);

			string fullFilePath = saveDataFolderPath + key + ".json";

			string json = JsonConvert.SerializeObject(value);
			File.WriteAllText(fullFilePath, json);
		}

		/// <summary>
		/// Loads a custom type from the CustomTypes folder, please dont call this in update as it calls File.ReadAllText
		/// </summary>
		/// <typeparam name="Type">The type of the object you want to save</typeparam>
		/// <param name="key">The key associated with the value you want to load</param>
		/// <returns>The loading value</returns>
		public static Type LoadCustomType<Type>(string key)
		{
			string localPath = InternalUtils.GetCallerModPath();
			string saveDataFolderPath = InternalUtils.GetFullPathFromRelativePath(localPath) + SAVE_DATA_FOLDER_NAME + CUSTOM_TYPES_FOLDER_NAME;

			string fullFilePath = saveDataFolderPath + key + ".json";
			if (!File.Exists(fullFilePath))
			{
				throw new Exception("Could not find any custom types with the key \"" + key + "\"");
			}
			string json = File.ReadAllText(fullFilePath);
			return JsonConvert.DeserializeObject<Type>(json);
		}

		static void createFoldersIfNeeded(string localPath)
		{
			string saveDataFolderPath = InternalUtils.GetFullPathFromRelativePath(localPath) + SAVE_DATA_FOLDER_NAME + CUSTOM_TYPES_FOLDER_NAME;
			if (!Directory.Exists(saveDataFolderPath))
			{
				Directory.CreateDirectory(saveDataFolderPath);
			}
		}

		internal static void TryLoadDataFromFile(string localPath)
		{
			string modPathFullPath = InternalUtils.GetFullPathFromRelativePath(localPath);

			string path = modPathFullPath + SAVE_DATA_FOLDER_NAME + PRIMITIVES_SAVE_DATA_FILE_NAME;

			if(!File.Exists(path))
				return;

			string json = File.ReadAllText(path);
			_savedValues[localPath] = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
		}
	}
}