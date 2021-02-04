using ModLibrary;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace InternalModBot.Scripting
{
	[MoonSharpUserData]
	public class LevelObjectsRef
	{

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
