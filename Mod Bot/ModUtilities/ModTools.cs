using ModLibrary.LevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ModLibrary
{
    /// <summary>
    /// General tools to help you when working with enums!
    /// </summary>
    public static class EnumTools
    {
        /// <summary>
        /// Gets the name of the given value in an <see langword="enum"/>
        /// <para>Exceptions:</para>
        /// <para/><see cref="ArgumentNullException"/>: If value is <see langword="null"/> or <see langword="typeof"/>(<typeparamref name="T"/>) is <see langword="null"/>
        /// <para/><see cref="ArgumentException"/>: <typeparamref name="T"/> is not an <see langword="enum"/> type
        /// </summary>
        /// <typeparam name="T">The type of <see langword="enum"/> to get the name from</typeparam>
        /// <param name="value">The value assigned to an entry in the specified <see langword="enum"/></param>
        /// <returns>The name of the entry with the value <paramref name="value"/></returns>
        public static string GetName<T>(T value)
        {
            return Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// Gets all names in the given <see langword="enum"/>
        /// <para>Exceptions:</para>
        /// <para/><see cref="ArgumentNullException"/>: If <see langword="typeof"/>(<typeparamref name="T"/>) is <see langword="null"/>
        /// <para/><see cref="ArgumentException"/>: <typeparamref name="T"/> is not an <see langword="enum"/> type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetNames<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        /// <summary>
        /// Gets all values of an <see langword="enum"/>
        /// <para>Exceptions:</para>
        /// <para/><see cref="ArgumentNullException"/>: If <see langword="typeof"/>(<typeparamref name="T"/>) is <see langword="null"/>
        /// <para/><see cref="ArgumentException"/>: <typeparamref name="T"/> is not an <see langword="enum"/> type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetValues<T>()
        {
            return ((T[])Enum.GetValues(typeof(T))).ToList();
        }
    }

    /// <summary>
    /// Extention methods implemented by mod tools, don't call these directly.
    /// </summary>
    public static class ModToolExtensionMethods
    {
        /// <summary>
        /// Gets all enemy <see cref="Character"/>s in the specified range
        /// </summary>
        /// <param name="characterTracker"></param>
        /// <param name="origin">The point to calculate the distance from</param>
        /// <param name="radius">The radius to get all enemy <see cref="Character"/>s within</param>
        /// <returns></returns>
        public static List<Character> GetAllEnemyCharactersInRange(this CharacterTracker characterTracker, Vector3 origin, float radius)
        {
            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            List<Character> charactersInRange = new List<Character>();

            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i].IsPlayerTeam && !characters[i].IsMainPlayer() && Vector3.Distance(origin, characters[i].transform.position) <= radius)
                    charactersInRange.Add(characters[i]);
            }

            return charactersInRange;
        }

        /// <summary>
        /// Gets all <see cref="Character"/>s in the specified range
        /// </summary>
        /// <param name="characterTracker"></param>
        /// <param name="origin">The point to calculate the distance from</param>
        /// <param name="radius">The radius to get all <see cref="Character"/>s within</param>
        /// <returns></returns>
        public static List<Character> GetAllCharactersInRange(this CharacterTracker characterTracker, Vector3 origin, float radius)
        {
            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            List<Character> charactersInRange = new List<Character>();

            for (int i = 0; i < characters.Count; i++)
            {
                if (Vector3.Distance(origin, characters[i].transform.position) <= radius)
                {
                    charactersInRange.Add(characters[i]);
                }
            }

            return charactersInRange;
        }

        /// <summary>
        /// Checks whether or not the given <see cref="UpgradeType"/> and level is already in use by an <see cref="UpgradeDescription"/>
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="ID">The ID of the upgrade</param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public static bool IsUpgradeTypeAndLevelUsed(this UpgradeManager upgradeManager, UpgradeType ID, int Level = 1)
        {
            return UpgradeManager.Instance.GetUpgrade(ID, Level) != null;
        }

        /// <summary>
        /// Gets the first found <see cref="MechBodyPart"/> of the given <see cref="MechBodyPartType"/> (Returns <see langword="null"/> if the given <see cref="Character"/> does not have the specified <see cref="MechBodyPartType"/>)
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MechBodyPart GetBodyPart(this Character character, MechBodyPartType type)
        {
            return character.GetBaseBodyPart(type) as MechBodyPart;
        }

        /// <summary>
        /// Gets all <see cref="MechBodyPart"/>s of the given <see cref="MechBodyPartType"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MechBodyPart> GetBodyParts(this Character character, MechBodyPartType type)
        {
            List<BaseBodyPart> baseBodyParts = character.GetBaseBodyParts(type);
            List<MechBodyPart> mechBodyParts = new List<MechBodyPart>();

            foreach (BaseBodyPart baseBodyPart in baseBodyParts)
            {
                if (baseBodyPart is MechBodyPart mechBodyPart)
                    mechBodyParts.Add(mechBodyPart);
            }

            return mechBodyParts;
        }

        /// <summary>
        /// Gets the first found <see cref="BaseBodyPart"/> of the given <see cref="MechBodyPartType"/> (Returns <see langword="null"/> if the given <see cref="Character"/> does not have the specified <see cref="MechBodyPartType"/>)
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseBodyPart GetBaseBodyPart(this Character character, MechBodyPartType type)
        {
            List<BaseBodyPart> bodyParts = character.GetAllBaseBodyParts();

            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (bodyParts[i].PartType == type)
                    return bodyParts[i];
            }

            return null;
        }

        /// <summary>
        /// Gets all <see cref="BaseBodyPart"/>s of the given <see cref="MechBodyPartType"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<BaseBodyPart> GetBaseBodyParts(this Character character, MechBodyPartType type)
        {
            List<BaseBodyPart> bodyParts = new List<BaseBodyPart>();

            for (int i = 0; i < character.GetAllBaseBodyParts().Count; i++)
            {
                if (character.GetAllBaseBodyParts()[i].PartType == type)
                    bodyParts.Add(character.GetAllBaseBodyParts()[i]);
            }

            return bodyParts;
        }

        /// <summary>
        /// Gets the first found <see cref="MindSpaceBodyPart"/> of the given <see cref="MechBodyPartType"/> (Returns <see langword="null"/> if the given <see cref="Character"/> does not have the specified <see cref="MechBodyPartType"/>)
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MindSpaceBodyPart GetMindSpaceBodyPart(this Character character, MechBodyPartType type)
        {
            return character.GetBaseBodyPart(type) as MindSpaceBodyPart;
        }

        /// <summary>
        /// Gets all <see cref="MindSpaceBodyPart"/>s of the given <see cref="MechBodyPartType"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MindSpaceBodyPart> GetMindSpaceBodyParts(this Character character, MechBodyPartType type)
        {
            List<BaseBodyPart> baseBodyParts = character.GetBaseBodyParts(type);
            List<MindSpaceBodyPart> mindSpaceBodyParts = new List<MindSpaceBodyPart>();

            foreach (BaseBodyPart baseBodyPart in baseBodyParts)
            {
                if (baseBodyPart is MindSpaceBodyPart mindSpaceBodyPart)
                    mindSpaceBodyParts.Add(mindSpaceBodyPart);
            }

            return mindSpaceBodyParts;
        }

        /// <summary>
        /// Check if <see cref="ObjectPlacedInLevel"/> was added by one of the mods
        /// </summary>
        /// <param name="objectPlacedInLevel"></param>
        /// <returns></returns>
        public static bool IsCustomLevelObject(this ObjectPlacedInLevel objectPlacedInLevel)
        {
            return objectPlacedInLevel && objectPlacedInLevel.LevelObjectEntry != null && objectPlacedInLevel.LevelObjectEntry.IsCustomLevelObject();
        }

        /// <summary>
        /// Check if <see cref="LevelObjectEntry"/> is related to a mod
        /// </summary>
        /// <param name="levelObjectEntry"></param>
        /// <returns></returns>
        public static bool IsCustomLevelObject(this LevelObjectEntry levelObjectEntry)
        {
            if (levelObjectEntry == null || string.IsNullOrEmpty(levelObjectEntry.PathUnderResources))
                return false;

            return CustomLevelEditorManager.IsPathToCustomObject(levelObjectEntry.PathUnderResources);
        }

        /// <summary>
        /// Quickly creates a sprite from texture
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Sprite ToSprite(this Texture2D texture2D) => texture2D ? Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect) : null;
    }

    /// <summary>
    /// Adds a few extension methods to the <see cref="ScrollRect"/> class
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scrolls the <see cref="ScrollRect"/> to the top
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        /// <summary>
        /// Scrolls the <see cref="ScrollRect"/> to the bottom
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}