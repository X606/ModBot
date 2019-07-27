using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLibrary
{
    namespace ModTools
    {
        public static class Upgrade
        {
            /// <summary>
            /// Checks whether or not the given upgrade id is already in use
            /// </summary>
            /// <param name="ID">The ID of the upgrade</param>
            /// <returns></returns>
            [Obsolete("Use UpgradeManager.IsUpgradeTypeAndLevelUsed instead")]
            public static bool IsIDAlreadyUsed(int ID)
            {
                List<UpgradeDescription> upgrades = UpgradeManager.Instance.UpgradeDescriptions;

                for (int i = 0; i < upgrades.Count; i++)
                {
                    if ((int)upgrades[i].UpgradeType == ID)
                        return true;
                }

                return false;
            }
            /// <summary>
            /// Checks whether or not the given upgrade id is already in use
            /// </summary>
            /// <param name="ID">The ID of the upgrade</param>
            /// <returns></returns>
            [Obsolete("Use UpgradeManager.IsUpgradeTypeAndLevelUsed instead")]
            public static bool IsIDAlreadyUsed(UpgradeType ID)
            {
                return IsIDAlreadyUsed((int)ID);
            }
            /// <summary>
            /// Gets the icon of the specified upgrade
            /// </summary>
            /// <param name="type">The upgrade's corresponding UpgradeType</param>
            /// <param name="level">The level of the upgrade</param>
            /// <returns></returns>
            [Obsolete("Use UpgradeManager.GetUpgradeIcon instead")]
            public static Sprite GetUpgradeIcon(UpgradeType type, int level = 1)
            {
                return UpgradeManager.Instance.GetUpgrade(type, level).Icon;
            }
            /// <summary>
            /// Gets the icon of the specified upgrade
            /// </summary>
            /// <param name="upgrade"></param>
            /// <returns></returns>
            [Obsolete("Use UpgradeManager.GetUpgradeIcon instead")]
            public static Sprite GetUpgradeIcon(UpgradeTypeAndLevel upgrade)
            {
                return GetUpgradeIcon(upgrade.UpgradeType, upgrade.Level);
            }
            /// <summary>
            /// Gives the specified upgrade to a FirstPersonMover
            /// </summary>
            /// <param name="Target"></param>
            /// <param name="Upgrade"></param>
            [Obsolete("Use FirstPersonMover.GiveUpgrade instead")]
            public static void Give(FirstPersonMover Target, UpgradeDescription Upgrade)
            {
                if (Target == null)
                    return;

                if (Target.IsMainPlayer())
                {
                    GameDataManager.Instance.SetUpgradeLevel(Upgrade.UpgradeType, Upgrade.Level);
                    UpgradeDescription upgrade = UpgradeManager.Instance.GetUpgrade(Upgrade.UpgradeType, Upgrade.Level);
                    GlobalEventManager.Instance.Dispatch("UpgradeCompleted", upgrade);
                }

                else
                {
                    UpgradeCollection upgradeCollection = Target.gameObject.GetComponent<UpgradeCollection>();

                    if (upgradeCollection == null)
                    {
                        debug.Log("Failed to give upgrade '" + Upgrade.UpgradeName + "' (Level: " + Upgrade.Level + ") to " + Target.CharacterName + " (UpgradeCollection is null)", Color.red);
                        return;
                    }

                    UpgradeTypeAndLevel upgradeToGive = new UpgradeTypeAndLevel { UpgradeType = Upgrade.UpgradeType, Level = Upgrade.Level };

                    List<UpgradeTypeAndLevel> upgrades = ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades.ToList();

                    upgrades.Add(upgradeToGive);

                    ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades = upgrades.ToArray();
                    ((PreconfiguredUpgradeCollection)upgradeCollection).InitializeUpgrades();

                    Target.RefreshUpgrades();
                }

                Target.SetUpgradesNeedsRefreshing();
            }
            /// <summary>
            /// Gives the specified upgrades to a FirstPersonMover
            /// </summary>
            /// <param name="Target"></param>
            /// <param name="Upgrades"></param>
            [Obsolete("Use FirstPersonMover.GiveUpgrade instead")]
            public static void Give(FirstPersonMover Target, UpgradeDescription[] Upgrades)
            {
                for (int i = 0; i < Upgrades.Length; i++)
                {
                    Give(Target, Upgrades[i]);
                }
            }
            /// <summary>
            /// Gives the specified upgrade to a FirstPersonMover
            /// </summary>
            /// <param name="Target"></param>
            /// <param name="Upgrade"></param>
            /// <param name="Level"></param>
            [Obsolete("Use FirstPersonMover.GiveUpgrade instead")]
            public static void Give(FirstPersonMover Target, UpgradeType Upgrade, int Level)
            {
                Give(Target, GetUpgradeDescriptionFromTypeAndLevel(Upgrade, Level));
            }
            /// <summary>
            /// Gives the specified upgrades to a FirstPersonMover
            /// </summary>
            /// <param name="Target"></param>
            /// <param name="Upgrades"></param>
            /// <param name="Levels"></param>
            [Obsolete("Use FirstPersonMover.GiveUpgrade instead")]
            public static void Give(FirstPersonMover Target, Dictionary<UpgradeType, int> upgrades)
            {
                List<UpgradeType> upgradeTypes = EnumTools.GetValues<UpgradeType>();

                for (int i = 0; i < upgradeTypes.Count; i++)
                {
                    if (upgrades.TryGetValue(upgradeTypes[i], out int level))
                    {
                        Give(Target, GetUpgradeDescriptionFromTypeAndLevel(upgradeTypes[i], level));
                    }
                }
            }
            /// <summary>
            /// Adds a new upgrade to the upgrade tree
            /// </summary>
            /// <param name="UpgradeID">The ID of the upgrade (If ID is already taken, the upgrade will not be added)</param>
            /// <param name="Name"></param>
            /// <param name="Description"></param>
            /// <param name="Icon">The display image of the upgrade</param>
            /// <param name="AngleOffset">The offset angle in the tree</param>
            /// <param name="IsLimited">If the upgrade has a limited number of uses (Like Clone)</param>
            /// <param name="IsRepeatable">Is the upgrade is repeatable (Like Armor)</param>
            /// <param name="MaxUses">Set the max uses of an upgrade (Will do nothing if IsLimited is false)</param>
            /// <param name="SortOrder"></param>
            /// <param name="Requirement">First Requirement</param>
            /// <param name="SecondRequirement">Second Requirement</param>
            /// <returns>The created upgrade, will be null if upgrade with ID already exists</returns>
            [Obsolete("Use UpgradeManager.AddUpgrade instead")]
            public static UpgradeDescription Add(int UpgradeID, string Name, string Description, Sprite Icon, float AngleOffset, bool IsLimited, bool IsRepeatable, int MaxUses, int SortOrder, UpgradeDescription Requirement, UpgradeDescription SecondRequirement)
            {
                if (UpgradeManager.Instance.IsUpgradeTypeAndLevelUsed((UpgradeType)UpgradeID))
                {
                    return null;
                }

                UpgradeDescription upgrade = UpgradeManager.Instance.UpgradeDescriptions[0].gameObject.AddComponent<UpgradeDescription>();

                upgrade.AngleOffset = AngleOffset;
                upgrade.CanBeTransferredInMultiplayer = false;
                upgrade.Description = Description;
                upgrade.HideInStoryMode = false;
                upgrade.Icon = Icon;
                upgrade.IsAvailableInMultiplayer = false;
                upgrade.IsConsumable = IsLimited;
                upgrade.IsDisabledInBattleRoyale = true;
                upgrade.IsRepeatable = IsRepeatable;
                upgrade.IsUpgradeVisible = true;
                upgrade.Level = 1;
                upgrade.MaxRepetitions = MaxUses;
                upgrade.RequiredMetagameProgress = MetagameProgress.P0_None;
                upgrade.Requirement = Requirement;
                upgrade.Requirement2 = SecondRequirement;
                upgrade.SkillPointCostBattleRoyale = 5;
                upgrade.SkillPointCostMultiplayer = 100;
                upgrade.SortOrder = SortOrder;
                upgrade.UpgradeName = Name;
                upgrade.UpgradeType = (UpgradeType)UpgradeID;

                UpgradeManager.Instance.UpgradeDescriptions.Add(upgrade);

                return upgrade;
            }

            /// <summary>
            /// Gets an <see cref="UpgradeTypeAndLevel"/> from an <see cref="UpgradeDescription"/>
            /// </summary>
            /// <param name="upgrade"></param>
            /// <returns></returns>
            public static UpgradeTypeAndLevel GetUpgradeTypeAndLevelFromUpgradeDescription(UpgradeDescription upgrade)
            {
                return new UpgradeTypeAndLevel { Level = upgrade.Level, UpgradeType = upgrade.UpgradeType };
            }
            /// <summary>
            /// Gets an UpgradeDescription from an UpgradeType and a level
            /// </summary>
            /// <param name="type"></param>
            /// <param name="level"></param>
            /// <returns></returns>
            [Obsolete("Use UpgradeManager.GetUpgrade(UpgradeType, int) instead")]
            public static UpgradeDescription GetUpgradeDescriptionFromTypeAndLevel(UpgradeType type, int level = 1)
            {
                return UpgradeManager.Instance.GetUpgrade(type, level);
            }
            /// <summary>
            /// Gets an UpgradeDescription from an UpgradeTypeAndLevel
            /// </summary>
            /// <param name="upgrade"></param>
            /// <returns></returns>
            public static UpgradeDescription GetUpgradeDescriptionFromTypeAndLevel(UpgradeTypeAndLevel upgrade)
            {
                return UpgradeManager.Instance.GetUpgrade(upgrade.UpgradeType, upgrade.Level);
            }
        }

        [Obsolete("All methods have been replaced with overrides")]
        public static class Clone
        {
            /// <summary>
            /// Spawns a clone in the clone area without the camera panning to it
            /// </summary>
            [Obsolete("Use CloneManager.SpawnClone instead")]
            public static void Spawn()
            {
                CloneManager.Instance.CloneArea.CreateNewClone(false);
                GameDataManager.Instance.IncreaseNumClones(1);
                CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
            }
            /// <summary>
            /// Spawns clones in the clone area without the camera panning to it
            /// </summary>
            /// <param name="count">The amount to spawn</param>
            [Obsolete("Use CloneManager.SpawnClone instead")]
            public static void Spawn(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    Spawn();
                }

                CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
            }
            /// <summary>
            /// Removes a clone from the clone area
            /// </summary>
            [Obsolete("Use CloneManager.RemoveClone instead")]
            public static void Remove()
            {
                if (CloneManager.Instance.GetNumClones() == 0)
                    return;
                
                List<MechBodyPart> bodyParts = ((List<FirstPersonMover>)Accessor.GetPrivateField(typeof(CloneArea), "_clones", CloneManager.Instance.CloneArea))[CloneManager.Instance.GetNumClones() - 1].GetAllBodyParts();

                FireSpreadDefinition fireSpread = new FireSpreadDefinition
                {
                    DamageSourceType = DamageSourceType.None,
                    FireType = FireType.BanFire,
                    MinSpreadProbability = 1000f,
                    SpreadProbabilityDropOff = 0f,
                    StartSpreadProbability = 100f,
                    WaitBetweenSpreads = 0.001f
                };

                for (int i = 0; i < bodyParts.Count; i++)
                {
                    bodyParts[i].TryCutVolume(
                        bodyParts[i].transform.position + new Vector3(2f, 0.5f, -2f),
                        bodyParts[i].transform.position + new Vector3(-2f, 0.5f, -2f),
                        bodyParts[i].transform.position + new Vector3(-2f, 0.5f, 2f),
                        bodyParts[i].transform.position + new Vector3(2f, 0.5f, 2f),
                        AttackManager.Instance.GetNextAttackID(), false, null, DamageSourceType.SpeedHackBanFire, fireSpread, false);
                }

                CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
            }
            /// <summary>
            /// Removes clones from the clone area
            /// </summary>
            /// <param name="count">The amount to remove</param>
            [Obsolete("Use CloneManager.RemoveClone instead")]
            public static void Remove(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    Remove();
                }

                CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
            }
        }

        public static class Arrow
        {
            /// <summary>
            /// Creates an <see cref="ArrowProjectile"/> and makes it fly in the specified <paramref name="MoveDirection"/>
            /// </summary>
            /// <param name="Owner">The <see cref="FirstPersonMover"/> that should be considered the owner of the created <see cref="ArrowProjectile"/></param>
            /// <param name="StartPosition">The starting position of the created <see cref="ArrowProjectile"/></param>
            /// <param name="MoveDirection">The travel direction the created <see cref="ArrowProjectile"/></param>
            /// <param name="BladeWidth">The width of the created <see cref="ArrowProjectile"/></param>
            /// <param name="MakeFlyBySound">If the created <see cref="ArrowProjectile"/> should make a sound when flying close to the player, should be <see langword="false"/> if <paramref name="Owner"/> is the player</param>
            /// <param name="RotationZ">The rotation of the created <see cref="ArrowProjectile"/></param>
            /// <returns>The fired <see cref="ArrowProjectile"/></returns>
            public static ArrowProjectile Create(FirstPersonMover Owner, Vector3 StartPosition, Vector3 MoveDirection, float BladeWidth = 1f, bool MakeFlyBySound = false, float RotationZ = 0f)
            {
                if (Owner == null)
                    return null;

                ArrowProjectile arrow = ProjectileManager.Instance.CreateInactiveArrow(false);
                arrow.transform.SetParent(LevelSpecificWorldRoot.Instance.transform, true);
                arrow.SetBladeWidth(BladeWidth);
                arrow.StartFlying(StartPosition, MoveDirection, MakeFlyBySound, Owner, false, BoltNetwork.serverFrame, RotationZ);

                return arrow;
            }
            /// <summary>
            /// Creates a flaming <see cref="ArrowProjectile"/> with the specified <see cref="FireSpreadDefinition"/> and makes it fly in the specified <paramref name="MoveDirection"/>
            /// </summary>
            /// <param name="Owner">The <see cref="FirstPersonMover"/> that should be considered the owner of the created <see cref="ArrowProjectile"/></param>
            /// <param name="StartPosition">The starting position of the created <see cref="ArrowProjectile"/></param>
            /// <param name="MoveDirection">The travel direction the created <see cref="ArrowProjectile"/></param>
            /// <param name="fireSpreadDefinition">The <see cref="FireSpreadDefinition"/> used to calculate the spread and damage of the fire applied to the <see cref="Character"/> that was hit by the created <see cref="ArrowProjectile"/></param>
            /// <param name="BladeWidth">The width of the created <see cref="ArrowProjectile"/></param>
            /// <param name="MakeFlyBySound">If the created <see cref="ArrowProjectile"/> should make a sound when flying close to the player, should be <see langword="false"/> if <paramref name="Owner"/> is the player</param>
            /// <param name="RotationZ">The rotation of the created <see cref="ArrowProjectile"/></param>
            /// <returns>The fired arrow</returns>
            public static ArrowProjectile CreateFlaming(FirstPersonMover Owner, Vector3 StartPosition, Vector3 MoveDirection, FireSpreadDefinition fireSpreadDefinition, float BladeWidth = 1f, bool MakeFlyBySound = false, float RotationZ = 0f)
            {
                if (Owner == null)
                    return null;

                ArrowProjectile arrow = Create(Owner, StartPosition, MoveDirection, BladeWidth, MakeFlyBySound, RotationZ);
                arrow.SetOnFire(fireSpreadDefinition);

                return arrow;
            }
        }

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

        [Obsolete("All methods have been replaced with overrides")]
        public static class CharacterTools
        {
            /// <summary>
            /// Adds a name tag the the specified Character
            /// </summary>
            /// <param name="character"></param>
            /// <param name="DisplayName"></param>
            /// <param name="ColorIsRed">false: Red, true: Green</param>
            /// <param name="OverrideOld"></param>
            [Obsolete("Use Character.AddNameTag instead")]
            public static void AddNameTag(Character character, string DisplayName = "Ally", bool ColorIsRed = false, bool OverrideOld = false)
            {
                if (character.HasNameTag() && !OverrideOld)
                    return;

                Transform nameTag = TwitchEnemySpawnManager.Instance.TwitchEnemyNameTagPool.InstantiateNewObject(false);
                nameTag.SetParent(GameUIRoot.Instance.transform, false);
                nameTag.SetAsFirstSibling();

                EnemyNameTag enemyNameTag = nameTag.GetComponent<EnemyNameTag>();
                EnemyNameTagConfig enemyNameTagConfig = new EnemyNameTagConfig
                {
                    ForceEnemyColor = ColorIsRed,
                    HideAtDistance = CharacterNameTagManager.Instance.HideNameTagsAtDistance,
                    HideIfNotVisible = true
                };

                enemyNameTag.Initialize(character, DisplayName, enemyNameTagConfig);
                character.SetHasNameTag();
            }
            /// <summary>
            /// Gets the MechBodyPart of the given MechBodyPartType (Returns null if the given Character does not have that body type)
            /// </summary>
            /// <param name="character"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            [Obsolete("Use Character.GetBodyPart instead")]
            public static MechBodyPart GetBodyPartOfType(Character character, MechBodyPartType type)
            {
                List<MechBodyPart> bodyParts = character.GetAllBodyParts();

                for (int i = 0; i < bodyParts.Count; i++)
                {
                    if (bodyParts[i].PartType == type)
                        return bodyParts[i];
                }

                return null;
            }
            /// <summary>
            /// Gets all MechBodyParts of the given MechBodyPartTypes (Elements in the List will be null if the given Character does not have a MechBodyPartType specified in the List)
            /// </summary>
            /// <param name="character"></param>
            /// <param name="types"></param>
            /// <returns></returns>
            [Obsolete("Use Character.GetBodyParts instead")]
            public static List<MechBodyPart> GetBodyPartsOfTypes(Character character, List<MechBodyPartType> types)
            {
                if (types == null || types.Count == 0)
                    return new List<MechBodyPart>();

                List<MechBodyPart> bodyParts = character.GetAllBodyParts();

                for (int i = 0; i < bodyParts.Count;)
                {
                    if (types.Contains(bodyParts[i].PartType))
                    {
                        types.Remove(bodyParts[i].PartType);
                        i++;
                    }
                    else
                    {
                        bodyParts.RemoveAt(i);
                    }
                }

                return bodyParts;
            }
            /// <summary>
            /// Gets a WeaponModel from the given FirstPersonMover from the given WeaponType (Returns null if the FirstPersonMover does not have the weapon)
            /// </summary>
            /// <param name="firstPersonMover"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            [Obsolete("Use FirstPersonMover.GetWeaponModel instead")]
            public static WeaponModel GetWeaponModelFromType(FirstPersonMover firstPersonMover, WeaponType type)
            {
                if (type == WeaponType.None || firstPersonMover == null)
                    return null;

                WeaponModel[] weaponModels = firstPersonMover.GetCharacterModel().WeaponModels;

                for (int i = 0; i < weaponModels.Length; i++)
                {
                    if (weaponModels[i].WeaponType == type)
                        return weaponModels[i];
                }

                return null;
            }
            /// <summary>
            /// Gets a WeaponModel from the given Character from the given WeaponType (Returns null if the Character does not have the weapon)
            /// </summary>
            /// <param name="character"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            [Obsolete("Use FirstPersonMover.GetWeaponModel instead")]
            public static WeaponModel GetWeaponModelFromType(Character character, WeaponType type)
            {
                return GetWeaponModelFromType((FirstPersonMover)character, type);
            }
            
            [Obsolete("Use CharacterTracker.GetAllEnemyCharactersInRange instead")]
            public static List<Character> GetAllEnemyCharactersInRange(Vector3 origin, float radius)
            {
                List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
                List<Character> charactersInRange = new List<Character>();

                for(int i = 0; i < characters.Count; i++)
                {
                    if (!characters[i].IsPlayerTeam && !characters[i].IsMainPlayer() && Vector3.Distance(origin, characters[i].GetPositionForAIToAimAt()) <= radius)
                        charactersInRange.Add(characters[i]);
                }

                return charactersInRange;
            }
        }

        [Obsolete("All methods have been replaced with overrides")]
        public static class ListTools
        {
            /// <summary>
            /// Randomly shuffles the given list
            /// </summary>
            /// <typeparam name="T">The type of the list</typeparam>
            /// <param name="ListToRandomize"></param>
            /// <param name="random"></param>
            /// <returns>The shuffled list</returns>
            [Obsolete("Use List.Randomize instead")]
            public static List<T> Shuffle<T>(List<T> ListToRandomize, System.Random random)
            {
                List<T> RandomizedList = new List<T>();

                for (int i = 0; ListToRandomize.Count > 0;)
                {
                    i = random.Next(0, ListToRandomize.Count - 1);

                    RandomizedList.Add(ListToRandomize[i]);
                    ListToRandomize.RemoveAt(i);
                }

                return RandomizedList;
            }
            /// <summary>
            /// Randomly shuffles the given list
            /// </summary>
            /// <typeparam name="T">The type of the list</typeparam>
            /// <param name="ListToRandomize"></param>
            /// <returns>The shuffled list</returns>
            [Obsolete("Use List.Randomize instead")]
            public static List<T> Shuffle<T>(List<T> ListToRandomize)
            {
                return Shuffle(ListToRandomize, new System.Random());
            }
        }

        public static class Vector3Tools
        {
            /// <summary>
            /// Gets a direction from one <see cref="Vector3"/> to another
            /// </summary>
            /// <param name="StartPoint">The position to go from</param>
            /// <param name="Destination">The position to go to</param>
            /// <returns>The direction between the two points</returns>
            public static Vector3 GetDirection(Vector3 StartPoint, Vector3 Destination)
            {
                return Destination - StartPoint;
            }
        }
    }

    [Obsolete]
    public class ModdedUpgrade
    {
        public ModdedUpgrade(
            UpgradeType _id, int _level, string _upgradeName, string _upgradeDescription, Sprite _icon,
            float _angleOffsetInUpgradeTree, int _maxRepetitions, bool _isUpgradeRepeatable, int _sortOrder,
            UpgradeDescription _firstRequirement, UpgradeDescription _secondRequirement)
        {
            ID = _id;
            UpgradeLevel = _level;
            UpgradeName = _upgradeName;
            UpgradeDescription = _upgradeDescription;
            UpgradeIcon = _icon;
            AngleOffsetInUpgradeTree = _angleOffsetInUpgradeTree;
            HasLimitedUses = _maxRepetitions > 0;
            MaxRepititions = _maxRepetitions;
            IsUpgradeRepeatable = _isUpgradeRepeatable;
            SortOrder = _sortOrder;
            FirstRequirement = _firstRequirement;
            SecondRequirement = _secondRequirement;
        }

        public readonly UpgradeType ID;
        public readonly int UpgradeLevel;
        public readonly string UpgradeName;
        public readonly string UpgradeDescription;
        public readonly Sprite UpgradeIcon;
        public readonly float AngleOffsetInUpgradeTree;
        public readonly bool HasLimitedUses;
        public readonly int MaxRepititions;
        public readonly bool IsUpgradeRepeatable;
        public readonly int SortOrder;
        public readonly UpgradeDescription FirstRequirement;
        public readonly UpgradeDescription SecondRequirement;
    }

    public static class ModToolExtensionMethods
    {
        /// <summary>
        /// Adds a collection to a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="collection"></param>
        [Obsolete("Use List<T>.AddRange(ICollection<T>) instead")]
        public static void Add<T>(this List<T> list, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                list.Add(item);
            }
        }

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
                if (!characters[i].IsPlayerTeam && !characters[i].IsMainPlayer() && Vector3.Distance(origin, characters[i].GetPositionForAIToAimAt()) <= radius)
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
        /// <param name="ID">The ID of the upgrade</param>
        /// <returns></returns>
        public static bool IsUpgradeTypeAndLevelUsed(this UpgradeManager upgradeManager, UpgradeType ID, int Level = 1)
        {
            return UpgradeManager.Instance.GetUpgrade(ID, Level) != null;
        }
        /// <summary>
        /// Gets the icon of the specified upgrade
        /// </summary>
        /// <param name="type">The upgrade's corresponding <see cref="UpgradeType"/></param>
        /// <param name="level">The level of the upgrade</param>
        /// <returns></returns>
        public static Sprite GetUpgradeIcon(this UpgradeManager upgradeManager, UpgradeType type, int level = 1)
        {
            return UpgradeManager.Instance.GetUpgrade(type, level).Icon;
        }
        /// <summary>
        /// Gets the icon of the specified upgrade
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static Sprite GetUpgradeIcon(this UpgradeManager upgradeManager, UpgradeTypeAndLevel upgrade)
        {
            return UpgradeManager.Instance.GetUpgradeIcon(upgrade.UpgradeType, upgrade.Level);
        }
        /// <summary>
        /// Gives the specified <see cref="UpgradeDescription"/> to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="Upgrade"></param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeDescription Upgrade)
        {
            if (firstPersonMover == null)
                return;

            if (firstPersonMover.IsMainPlayer())
            {
                GameDataManager.Instance.SetUpgradeLevel(Upgrade.UpgradeType, Upgrade.Level);
                UpgradeDescription upgrade = UpgradeManager.Instance.GetUpgrade(Upgrade.UpgradeType, Upgrade.Level);
                GlobalEventManager.Instance.Dispatch("UpgradeCompleted", upgrade);
            }
            else
            {
                UpgradeCollection upgradeCollection = firstPersonMover.gameObject.GetComponent<UpgradeCollection>();

                if (upgradeCollection == null)
                {
                    debug.Log("Failed to give upgrade '" + Upgrade.UpgradeName + "' (Level: " + Upgrade.Level + ") to " + firstPersonMover.CharacterName + " (UpgradeCollection is null)", Color.red);
                    return;
                }

                UpgradeTypeAndLevel upgradeToGive = new UpgradeTypeAndLevel { UpgradeType = Upgrade.UpgradeType, Level = Upgrade.Level };

                List<UpgradeTypeAndLevel> upgrades = ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades.ToList();

                upgrades.Add(upgradeToGive);

                ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades = upgrades.ToArray();
                ((PreconfiguredUpgradeCollection)upgradeCollection).InitializeUpgrades();

                firstPersonMover.RefreshUpgrades();
            }

            firstPersonMover.SetUpgradesNeedsRefreshing();
        }
        /// <summary>
        /// Gives the specified <see cref="UpgradeDescription"/> to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="Upgrades"></param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeDescription[] Upgrades)
        {
            for (int i = 0; i < Upgrades.Length; i++)
            {
                firstPersonMover.GiveUpgrade(Upgrades[i]);
            }
        }
        /// <summary>
        /// Gives the specified upgrade to a FirstPersonMover
        /// </summary>
        /// <param name="Upgrade"></param>
        /// <param name="Level"></param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeType upgrade, int Level)
        {
            firstPersonMover.GiveUpgrade(UpgradeManager.Instance.GetUpgrade(upgrade, Level));
        }
        /// <summary>
        /// Gives the specified upgrades to a FirstPersonMover
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Upgrades"></param>
        /// <param name="Levels"></param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, Dictionary<UpgradeType, int> Upgrades)
        {
            foreach (UpgradeType upgrade in Upgrades.Keys)
            {
                firstPersonMover.GiveUpgrade(upgrade, Upgrades[upgrade]);
            }
        }

        /// <summary>
        /// Spawns a clone in the <see cref="CloneArea"/>
        /// </summary>
        /// <returns>The created clone</returns>
        public static FirstPersonMover SpawnClone(this CloneManager cloneManager)
        {
            Accessor cloneAreaAccessor = new Accessor(typeof(CloneArea), CloneManager.Instance.CloneArea);

            FirstPersonMover clone = (FirstPersonMover)cloneAreaAccessor.CallPrivateMethod("spawnClone");

            CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();

            return clone;
        }
        /// <summary>
        /// Spawns clones in the <see cref="CloneArea"/>
        /// </summary>
        /// <param name="cloneManager"></param>
        /// <param name="count">Amount to spawn</param>
        /// <returns>The created clones</returns>
        public static List<FirstPersonMover> SpawnClone(this CloneManager cloneManager, int count)
        {
            List<FirstPersonMover> spawnedClones = new List<FirstPersonMover>();

            for (int i = 0; i < count; i++)
            {
                spawnedClones.Add(CloneManager.Instance.SpawnClone());
            }

            CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();

            return spawnedClones;
        }
        /// <summary>
        /// Removes a clone from the <see cref="CloneArea"/>
        /// </summary>
        /// <param name="cloneManager"></param>
        public static void RemoveClone(this CloneManager cloneManager)
        {
            if (CloneManager.Instance.GetNumClones() == 0)
                return;

            List<FirstPersonMover> clones = (List<FirstPersonMover>)Accessor.GetPrivateField(typeof(CloneArea), "_clones", CloneManager.Instance.CloneArea);

            List<MechBodyPart> bodyParts = clones[CloneManager.Instance.GetNumClones() - 1].GetAllBodyParts();

            FireSpreadDefinition fireSpread = new FireSpreadDefinition
            {
                DamageSourceType = DamageSourceType.None,
                FireType = FireType.BanFire,
                MinSpreadProbability = 1000f,
                SpreadProbabilityDropOff = 0f,
                StartSpreadProbability = 100f,
                WaitBetweenSpreads = 0.001f
            };

            for (int i = 0; i < bodyParts.Count; i++)
            {
                bodyParts[i].TryCutVolume(
                    bodyParts[i].transform.position + new Vector3(2f, 0.5f, -2f),
                    bodyParts[i].transform.position + new Vector3(-2f, 0.5f, -2f),
                    bodyParts[i].transform.position + new Vector3(-2f, 0.5f, 2f),
                    bodyParts[i].transform.position + new Vector3(2f, 0.5f, 2f),
                    AttackManager.Instance.GetNextAttackID(), false, null, DamageSourceType.SpeedHackBanFire, fireSpread, false);
            }

            CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
        }
        /// <summary>
        /// Removes clones from the <see cref="CloneArea"/>
        /// </summary>
        /// <param name="cloneManager"></param>
        /// <param name="count">The amount to remove</param>
        public static void RemoveClone(this CloneManager cloneManager, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CloneManager.Instance.RemoveClone();
            }

            CloneManager.Instance.CloneArea.PutAllClonesBackToStartingPositions();
        }

        /// <summary>
        /// Adds a name tag to a <see cref="Character"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="DisplayName"></param>
        /// <param name="TextColor"></param>
        /// <param name="OverrideOld"></param>
        public static void AddNameTag(this Character character, string DisplayName, Color TextColor, bool OverrideOld)
        {
            if (character.HasNameTag() && !OverrideOld)
                return;

            Transform nameTag = TwitchEnemySpawnManager.Instance.TwitchEnemyNameTagPool.InstantiateNewObject(false);
            nameTag.SetParent(GameUIRoot.Instance.transform, false);
            nameTag.SetAsFirstSibling();

            EnemyNameTag enemyNameTag = nameTag.GetComponent<EnemyNameTag>();
            EnemyNameTagConfig enemyNameTagConfig = new EnemyNameTagConfig
            {
                HideAtDistance = CharacterNameTagManager.Instance.HideNameTagsAtDistance,
                HideIfNotVisible = true
            };

            enemyNameTag.NameText.color = TextColor;

            enemyNameTag.Initialize(character, DisplayName, enemyNameTagConfig);
            character.SetHasNameTag();
        }

        /// <summary>
        /// Gets the first found <see cref="MechBodyPart"/> of the given <see cref="MechBodyPartType"/> (Returns <see langword="null"/> if the given <see cref="Character"/> does not have the specified <see cref="MechBodyPartType"/>)
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MechBodyPart GetBodyPart(this Character character, MechBodyPartType type)
        {
            List<MechBodyPart> bodyParts = character.GetAllBodyParts();

            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (bodyParts[i].PartType == type)
                    return bodyParts[i];
            }

            return null;
        }
        /// <summary>
        /// Gets all <see cref="MechBodyPart"/>s of the given <see cref="MechBodyPartType"/>s (Elements in the returned <see cref="List{T}"/> will be <see langword="null"/> if the given <see cref="Character"/> does not have a <see cref="MechBodyPartType"/> specified in the <see cref="List{T}"/>)
        /// </summary>
        /// <param name="character"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List<MechBodyPart> GetBodyParts(this Character character, List<MechBodyPartType> types)
        {
            if (types == null || types.Count == 0)
                return new List<MechBodyPart>();

            List<MechBodyPart> bodyParts = character.GetAllBodyParts();
            List<MechBodyPart> passedParts = new List<MechBodyPart>();

            for (int i = 0; i < bodyParts.Count;)
            {
                if (types.Contains(bodyParts[i].PartType))
                {
                    types.Remove(bodyParts[i].PartType);
                    passedParts.Add(bodyParts[i]);
                }
            }

            return passedParts;
        }
        /// <summary>
        /// Gets all <see cref="MechBodyPart"/>s of the given <see cref="MechBodyPartType"/>
        /// </summary>
        /// <param name="character"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MechBodyPart> GetBodyParts(this Character character, MechBodyPartType type)
        {
            List<MechBodyPart> bodyParts = new List<MechBodyPart>();

            for (int i = 0; i < character.GetAllBodyParts().Count; i++)
            {
                if (character.GetAllBodyParts()[i].PartType == type)
                {
                    bodyParts.Add(character.GetAllBodyParts()[i]);
                }
            }

            return bodyParts;
        }

        /// <summary>
        /// Gets a WeaponModel from the given FirstPersonMover from the given WeaponType (Returns null if the FirstPersonMover does not have the weapon)
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete("Use CharacterModel.GetWeaponModel(WeaponType) instead")]
        public static WeaponModel GetWeaponModel(this FirstPersonMover firstPersonMover, WeaponType type)
        {
            if (firstPersonMover == null || firstPersonMover.GetCharacterModel() == null)
            {
                return null;
            }

            return firstPersonMover.GetCharacterModel().GetWeaponModel(type);
        }

        /// <summary>
        /// Randomizes the order of the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Randomize<T>(this List<T> list)
        {
            List<T> randomizedList = new List<T>();

            while (list.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, list.Count);

                randomizedList.Add(list[index]);
                list.RemoveAt(index);
            }
        }
    }
}
