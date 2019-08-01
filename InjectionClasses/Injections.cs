using ModLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class MethodsToInject
{
    public static Sprite LibraryListItemDisplay_PNGPathToSprite(string previewPathUnderResources)
    {
        Texture2D texture2D = new Texture2D(10, 10);
        if (previewPathUnderResources.StartsWith("modded/"))
        {
            string str = previewPathUnderResources.Replace("modded/", "");
            byte[] data = File.ReadAllBytes(AssetLoader.GetSubdomain(Application.dataPath) + "mods/" + str);
            texture2D.LoadImage(data);
        }
        else
        {
            texture2D = Resources.Load<Texture2D>(previewPathUnderResources.Replace(".png", string.Empty));
        }
        return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
    }

    public static List<Dropdown.OptionData> LevelEnemySpawner_GetDropdownOptions(string fieldName)
    {
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        List<LevelObjectEntry> levelObjectsInLibrary = Singleton<LevelObjectsLibraryManager>.Instance.GetLevelObjectsInLibrary();
        for (int i = 0; i < levelObjectsInLibrary.Count; i++)
        {
            if (!levelObjectsInLibrary[i].PathUnderResources.StartsWith("modded/"))
            {
                LevelEnemySpawner component = Resources.Load<Transform>(levelObjectsInLibrary[i].PathUnderResources).GetComponent<LevelEnemySpawner>();
                if (component != null)
                {
                    FirstPersonMover component2 = component.EnemyPrefab.GetComponent<FirstPersonMover>();
                    if (component2 != null && component2.CanRideOthers())
                    {
                        list.Add(new DropdownIntOptionData
                        {
                            text = levelObjectsInLibrary[i].DisplayName,
                            IntValue = (int)component2.CharacterType
                        });
                    }
                }
            }
        }
        list.Sort((Dropdown.OptionData x, Dropdown.OptionData y) => x.text.CompareTo(y.text));
        list.Insert(0, new DropdownIntOptionData
        {
            text = "None",
            IntValue = 0
        });
        return list;
    }

    public static ObjectPlacedInLevel ObjectPlacedInLevel_PlaceObjectInLevelRoot(LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot)
    {
        Transform transform = null;
        if (objectPlacedLevelObjectEntry.PathUnderResources.StartsWith("modded/"))
        {
            string[] array = objectPlacedLevelObjectEntry.PathUnderResources.Split('/');
            if (array.Length != 3)
            {
                Debug.LogError("'" + objectPlacedLevelObjectEntry.PathUnderResources + "' was not set up right! It needs to only have 2 '/'es");
            }
            try
            {
                transform = AssetLoader.GetObjectFromFile(array[1], array[2]).transform;
            }
            catch (Exception exception)
            {
                Debug.LogError("You dont have the mod '" + array[1] + "' installed, please install this mod to use this level.\nCaught exception: " + exception.Message);
            }
        }
        else
        {
            transform = Resources.Load<Transform>(objectPlacedLevelObjectEntry.PathUnderResources);
        }
        if (transform == null)
        {
            if (objectPlacedLevelObjectEntry.PathUnderResources.StartsWith("modded/"))
            {
                Debug.LogError("Looks like this level requires a mod called " + objectPlacedLevelObjectEntry.PathUnderResources.Split('/')[1] + " ask around in the discord on how you fix this :)");
            }
            else
            {
                Debug.LogError("PlaceObjectInLevelRoot, Can't find asset: " + objectPlacedLevelObjectEntry.PathUnderResources);
            }
            return null;
        }
        Transform transform2 = UnityEngine.Object.Instantiate(transform);
        transform2.SetParent(levelRoot, false);
        if (!objectPlacedLevelObjectEntry.IsSection())
        {
            transform2.gameObject.AddComponent<SectionMember>();
        }
        ObjectPlacedInLevel objectPlacedInLevel = transform2.gameObject.AddComponent<ObjectPlacedInLevel>();
        objectPlacedInLevel.LevelObjectEntry = objectPlacedLevelObjectEntry;
        objectPlacedInLevel.Initialize();
        Accessor.CallPrivateMethod("registerObjectInAllObjectList", LevelEditorObjectPlacementManager.Instance, new object[] { objectPlacedInLevel });
        return objectPlacedInLevel;
    }

    public static ArrowProjectile ProjectileManager_CreateInActiveArrow(bool isOnFire)
    {
        Transform transform = ProjectileManager.Instance.ArrowPool.InstantiateNewObject(false);
        ArrowProjectile arrowProjectile = Singleton<CacheManager>.Instance.GetArrowProjectile(transform);
        arrowProjectile.SetFlamingVisuals(isOnFire);
        arrowProjectile.SetInactive();
        arrowProjectile.SetDamageTypeOverride(DamageSourceType.None);

        Accessor projectileManagerAccessor = new Accessor(typeof(ProjectileManager), ProjectileManager.Instance);

        if (BoltNetwork.isServer)
        {
            projectileManagerAccessor.SetPrivateField("_nextArrowID", projectileManagerAccessor.GetPrivateField<int>("_nextArrowID") + 1);
        }
        arrowProjectile.SetProjectileID(projectileManagerAccessor.GetPrivateField<int>("_nextArrowID"));
        ModsManager.Instance.passOnMod.OnProjectileCreated(arrowProjectile.gameObject);
        return arrowProjectile;
    }

    public static BulletProjectile ProjectileManager_CreateMortarShrapnel(Vector3 startPosition, Vector3 flyDirection, Character owner)
    {
        Transform transform = ProjectileManager.Instance.MortarExplosionShrapnelPool.InstantiateNewObject(false);
        BulletProjectile bulletProjectile = CacheManager.Instance.GetBulletProjectile(transform);
        ModsManager.Instance.passOnMod.OnProjectileCreated(bulletProjectile.gameObject);
        bulletProjectile.StartFlying(startPosition, flyDirection, false, owner);
        return bulletProjectile;
    }

    public static BulletProjectile ProjectileManager_CreateFlameBreathProjectile(Vector3 startPosition, Vector3 flyDirection, Character owner)
    {
        PooledPrefab pooledPrefab;
        if (owner.IsMainPlayer())
        {
            pooledPrefab = ProjectileManager.Instance.FlameBreathProjectilePool;
        }
        else
        {
            pooledPrefab = ProjectileManager.Instance.FlameBreathEnemyProjectilePool;
        }
        Transform transform = pooledPrefab.InstantiateNewObject(false);
        BulletProjectile bulletProjectile = CacheManager.Instance.GetBulletProjectile(transform);
        ModsManager.Instance.passOnMod.OnProjectileCreated(bulletProjectile.gameObject);
        bulletProjectile.StartFlying(startPosition, flyDirection, false, owner);
        return bulletProjectile;
    }

    public static BulletProjectile ProjectileManager_CreateRepairFlameProjectile(Vector3 startPosition, Vector3 flyDirection)
    {
        Transform transform = ProjectileManager.Instance.RepairFlameProjectilePool.InstantiateNewObject(false);
        BulletProjectile bulletProjectile = CacheManager.Instance.GetBulletProjectile(transform);
        ModsManager.Instance.passOnMod.OnProjectileCreated(bulletProjectile.gameObject);
        bulletProjectile.StartFlying(startPosition, flyDirection, false, null);
        return bulletProjectile;
    }

    public static void Projectile_OnEnvironmentCollided(Projectile projectile, bool playImpactVFX)
    {
        if (projectile.PassThroughEnvironment)
        {
            return;
        }

        Accessor projectileAccessor = new Accessor(typeof(Projectile), projectile);

        if (!projectileAccessor.GetPrivateField<bool>("_isFlying"))
        {
            return;
        }
        if (projectile.MainCollider != null)
        {
            projectile.MainCollider.enabled = false;
        }
        projectileAccessor.CallPrivateMethod("PlayGroundImpactAudioClip");
        projectileAccessor.CallPrivateMethod("PlayExtraGroundImpactAudioClips");
        if (playImpactVFX)
        {
            projectileAccessor.CallPrivateMethod("PlayGroundImpactVFX");
        }
        ModsManager.Instance.passOnMod.OnProjectileDestroyed(projectile.gameObject);
        projectile.GetComponent<PooledPrefabReference>().OwnerPool.DestroyObject(projectile.gameObject, true);
    }

    public static void LevelObjectsLibraryManager_Initialize()
    {
        Accessor levelObjectsLibraryManagerAccessor = new Accessor(typeof(LevelObjectsLibraryManager), LevelObjectsLibraryManager.Instance);

        string text = (Resources.Load("Data/LevelObjects/LevelObjectManifest") as TextAsset).text;
        string[] files = Directory.GetFiles(AssetLoader.GetModsFolderDirectory());
        List<LevelObjectEntry> moddedObjects = new List<LevelObjectEntry>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".json"))
            {
                string value = File.ReadAllText(files[i]);
                moddedObjects.AddRange(JsonConvert.DeserializeObject<List<LevelObjectEntry>>(value, LevelObjectsLibraryManager.getSerializerSettings()));
            }
        }
        levelObjectsLibraryManagerAccessor.SetPrivateField("_levelObjects", JsonConvert.DeserializeObject<List<LevelObjectEntry>>(text, LevelObjectsLibraryManager.getSerializerSettings()));
        levelObjectsLibraryManagerAccessor.GetPrivateField<List<LevelObjectEntry>>("_levelObjects").AddRange(moddedObjects);

        levelObjectsLibraryManagerAccessor.SetPrivateField("_visibleLevelObjects", new List<LevelObjectEntry>(levelObjectsLibraryManagerAccessor.GetPrivateField<List<LevelObjectEntry>>("_levelObjects")));
        levelObjectsLibraryManagerAccessor.GetPrivateField<List<LevelObjectEntry>>("_visibleLevelObjects").RemoveAll((LevelObjectEntry entry) => entry.IsUnityEditorOnly());
    }
}

