using HarmonyLib;
using InternalModBot.LevelEditor;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelEditorObjectPlacementManager))]
    static class LevelEditorObjectPlacementManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorObjectPlacementManager.PlaceObjectInLevelRoot))]
        static bool PlaceObjectInLevelRoot_Prefix(LevelEditorObjectPlacementManager __instance, ref ObjectPlacedInLevel __result, LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot)
        {
            bool isMissingObject = false;
            string modName = null;
            string path = objectPlacedLevelObjectEntry.PathUnderResources;

            // try load the prefab
            Transform transform = Resources.Load<Transform>(path);
            if (transform == null)
            {
                string newPath = LevelObjectsLibraryManager.Instance.GetRenamedPath(path);
                if (path != newPath)
                {
                    transform = Resources.Load<Transform>(newPath);
                }
            }

            if (transform == null)
            {
                if (CustomLevelEditorManager.IsPathToCustomObject(path))
                {
                    modName = path.Split('/')[3];
                }
                else
                {
                    Debug.LogError("PlaceObjectInLevelRoot, Can't find asset: " + objectPlacedLevelObjectEntry.PathUnderResources);
                    __result = null;
                    return false;
                }

                transform = CustomLevelEditorManager.GetMissingObjectTransform();
                isMissingObject = true;
            }

            // instantiate and setup
            Transform instantiatedObject = Object.Instantiate(transform);
            instantiatedObject.SetParent(levelRoot, false);
            if (!objectPlacedLevelObjectEntry.IsSection()) instantiatedObject.gameObject.AddComponent<SectionMember>();

            ObjectPlacedInLevel objectPlacedInLevel = instantiatedObject.GetComponent<ObjectPlacedInLevel>();
            if (objectPlacedInLevel == null) objectPlacedInLevel = instantiatedObject.gameObject.AddComponent<ObjectPlacedInLevel>();

            objectPlacedInLevel.LevelObjectEntry = objectPlacedLevelObjectEntry;
            if (isMissingObject)
            {
                string description = $"This object requires \"{modName}\" installed and enabled.\n\n" +
                    $"Try installing the mod or ask around in the discord on how you fix this.\n\n" +
                    $"Object path: \"{path}\"";

                LevelEditorComponentDescription levelEditorComponentDescription = objectPlacedInLevel.gameObject.AddComponent<LevelEditorComponentDescription>();
                levelEditorComponentDescription.Description = description;
            }
            objectPlacedInLevel.Initialize(levelRoot);

            __instance.registerObjectInAllObjectList(objectPlacedInLevel);
            __result = objectPlacedInLevel;

            return false;
        }
    }
}