using HarmonyLib;
using ModLibrary;
using ModLibrary.LevelEditor;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelEditorDataManager))]
    static class LevelEditorDataManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorDataManager.SerializeWorldToDataObject))]
        static bool SerializeWorldToDataObject_Patch(LevelEditorDataManager __instance)
        {
            List<LevelEditorLevelObject> customLevelObjects = new List<LevelEditorLevelObject>();
            CustomObjectsList customObjectsList = new CustomObjectsList()
            {
                Objects = customLevelObjects,
            };

            // new faster serialization logic for root object
            // i dont think it has to support object parenting since the game is written without parenting considered for the most part
            Transform levelRoot = LevelEditorObjectPlacementManager.Instance.GetLevelRoot();
            LevelEditorLevelObject rootLevelObject = new LevelEditorLevelObject
            {
                Name = levelRoot.name,
                Transform = new LevelEditorTransform()
            };
            rootLevelObject.Transform.SerializeFrom(levelRoot);
            if (levelRoot.childCount != 0)
            {
                for (int i = 0; i < levelRoot.childCount; i++)
                {
                    ObjectPlacedInLevel objectPlacedInLevel = levelRoot.GetChild(i).GetComponent<ObjectPlacedInLevel>();
                    if (objectPlacedInLevel == null) continue;

                    LevelEditorLevelObject childLevelObject = new LevelEditorLevelObject();
                    childLevelObject.SerializeFrom(objectPlacedInLevel);

                    // separate custom objects to avoid crashes and the possibility of losing them
                    if (objectPlacedInLevel.IsCustomLevelObject())
                        customLevelObjects.Add(childLevelObject);
                    else
                        rootLevelObject.Children.Add(childLevelObject);
                }
            }
            __instance._currentLevelData.RootLevelObject = rootLevelObject;

            // write to level meta data
            LevelEditorLevelData levelData = __instance._currentLevelData;
            if (levelData.ModdedMetadata == null) levelData.ModdedMetadata = new Dictionary<string, string>();
            levelData.ModdedMetadata[CustomObjectsList.CUSTOM_OBJECTS_LIST_KEY] = customObjectsList.Serialize();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorDataManager.DeserializeInto))]
        static void DeserializeInto_Patch(Transform levelRoot, LevelEditorLevelData currentLevelData, bool isAsync = false)
        {
            // try to deserialize custom objects list and insert it into main list
            List<LevelEditorLevelObject> levelObjects = currentLevelData?.RootLevelObject?.Children;
            if (levelObjects == null) return;

            CustomObjectsList customObjects;
            if (currentLevelData.ModdedMetadata != null && currentLevelData.ModdedMetadata.ContainsKey(CustomObjectsList.CUSTOM_OBJECTS_LIST_KEY))
            {
                try
                {
                    customObjects = CustomObjectsList.Deserialize(currentLevelData.ModdedMetadata[CustomObjectsList.CUSTOM_OBJECTS_LIST_KEY]);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }

            if (customObjects != null && customObjects.Objects != null && customObjects.Objects.Count != 0)
                levelObjects.AddRange(customObjects.Objects);
        }
    }
}
