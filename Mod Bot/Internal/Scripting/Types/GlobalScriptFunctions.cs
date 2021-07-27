using UnityEngine;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// Contains all the defualt objects that can be called from custom script objects
	/// </summary>
	internal static class GlobalScriptFunctions
	{
		/// <summary>
		/// Creates a Vector3Ref
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3Ref vector3(double x, double y, double z) => new Vector3((float)x, (float)y, (float)z);
		/// <summary>
		/// Gets the player gameobject
		/// </summary>
		/// <returns></returns>
		public static GameObjectRef getPlayer()
		{
			return CharacterTracker.Instance.GetPlayer().gameObject;
		}

		/// <summary>
		/// Used for Debuging things from scriptable objects
		/// </summary>
		public static DebugRef Debug = new DebugRef();
		/// <summary>
		/// Used for detecting input
		/// </summary>
		public static InputRef Input = new InputRef();
		/// <summary>
		/// Used for spawing / managing level objects
		/// </summary>
		public static LevelObjectsRef LevelObjects = new LevelObjectsRef();
	}

}
