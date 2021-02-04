using UnityEngine;

namespace InternalModBot.Scripting
{
	public static class GlobalScriptFunctions
	{
		public static Vector3Ref vector3(double x, double y, double z) => new Vector3((float)x, (float)y, (float)z);
		public static GameObjectRef getPlayer()
		{
			return CharacterTracker.Instance.GetPlayer().gameObject;
		}

		public static DebugRef Debug = new DebugRef();
		public static InputRef Input = new InputRef();
		public static LevelObjectsRef LevelObjects = new LevelObjectsRef();
	}

}
