// New mod loading system
/*
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
		
		public static byte LoadByte(string key)       => (byte)loadData(key, typeof(byte));
		public static ushort LoadUInt16(string key)   => (ushort)loadData(key, typeof(ushort));
		public static uint LoadUInt32(string key)     => (uint)loadData(key, typeof(uint));
		public static ulong LoadUInt64(string key)    => (ulong)loadData(key, typeof(ulong));
		public static sbyte LoadSByte(string key)     => (sbyte)loadData(key, typeof(sbyte));
		public static short LoadInt16(string key)     => (short)loadData(key, typeof(short));
		public static int LoadInt32(string key)       => (int)loadData(key, typeof(int));
		public static long LoadInt64(string key)      => (long)loadData(key, typeof(long));
		public static float LoadSingle(string key)    => (float)loadData(key, typeof(float));
		public static double LoadDouble(string key)   => (double)loadData(key, typeof(double));
		public static decimal LoadDecimal(string key) => (decimal)loadData(key, typeof(decimal));
		public static char LoadChar(string key)       => (char)loadData(key, typeof(char));
		public static string LoadString(string key)   => (string)loadData(key, typeof(string));
		public static bool LoadBool(string key)       => (bool)loadData(key, typeof(bool));
		public static TEnum LoadEnum<TEnum>(string key)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException("The passed type was not an enum");

			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));

			return (TEnum)loadData(key, underlyingType);
		}

		public static void SaveData(string key, byte value)    => saveData(key, value);
		public static void SaveData(string key, ushort value)  => saveData(key, value);
		public static void SaveData(string key, uint value)    => saveData(key, value);
		public static void SaveData(string key, ulong value)   => saveData(key, value);
		public static void SaveData(string key, sbyte value)   => saveData(key, value);
		public static void SaveData(string key, short value)   => saveData(key, value);
		public static void SaveData(string key, int value)     => saveData(key, value);
		public static void SaveData(string key, long value)    => saveData(key, value);
		public static void SaveData(string key, float value)   => saveData(key, value);
		public static void SaveData(string key, double value)  => saveData(key, value);
		public static void SaveData(string key, decimal value) => saveData(key, value);
		public static void SaveData(string key, char value)    => saveData(key, value);
		public static void SaveData(string key, string value)  => saveData(key, value);
		public static void SaveData(string key, bool value)    => saveData(key, value);
		public static void SaveEnum<TEnum>(string key, TEnum value)
		{
			if(!typeof(TEnum).IsEnum)
				throw new ArgumentException("The passed type was not an enum");

			Type type = Enum.GetUnderlyingType(typeof(TEnum));
			
			saveData(key, Convert.ChangeType(value, type));
		}

		public static void SaveCustomType<Type>(string key, Type value)
		{
			string localPath = InternalUtils.GetCallerModPath();
			string saveDataFolderPath = InternalUtils.GetFullPathFromRelativePath(localPath) + SAVE_DATA_FOLDER_NAME + CUSTOM_TYPES_FOLDER_NAME;

			createFoldersIfNeeded(localPath);

			string fullFilePath = saveDataFolderPath + key + ".json";

			string json = JsonConvert.SerializeObject(value);
			File.WriteAllText(fullFilePath, json);
		}
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
*/