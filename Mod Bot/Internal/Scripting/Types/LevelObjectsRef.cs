using ModLibrary;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// Contains functions for handling level objects
	/// </summary>
	[MoonSharpUserData]
	internal class LevelObjectsRef
	{
		/// <summary>
		/// Gets all the Gameobjects in the curret level
		/// </summary>
		/// <returns></returns>
		public List<GameObjectRef> getLevelEditorObjects()
		{
			List<ObjectPlacedInLevel> levelObjects = LevelEditorObjectPlacementManager.Instance.GetAllObjectsInLevelSameDifficultyGroup();
			List<GameObjectRef> gameObjectRefs = new List<GameObjectRef>();
			for (int i = 0; i < levelObjects.Count; i++)
			{
				gameObjectRefs.Add(levelObjects[i].gameObject);
			}

			return gameObjectRefs;
		}

		/// <summary>
		/// Creates a new level editor object
		/// </summary>
		/// <param name="prefabPath"></param>
		/// <returns></returns>
		public GameObjectRef createObject(string prefabPath)
		{
			ObjectPlacedInLevel spawned;
			try
			{
				spawned = LevelEditorObjectPlacementManager.Instance.PlaceObjectInLevelRoot(new LevelObjectEntry()
				{
					PathUnderResources = prefabPath
				}, LevelEditorObjectPlacementManager.Instance.GetLevelRoot());

			} catch
			{
				throw new System.Exception("Nothing found at the prefab path \"" + prefabPath + "\"");
			}

			return spawned.gameObject;
		}

	}
}
