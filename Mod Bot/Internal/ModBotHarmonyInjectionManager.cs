using BestHTTP;
using Bolt;
using HarmonyLib;
using ModLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Handles all of Mod-Bots runtils patching
    /// </summary>
    public static class ModBotHarmonyInjectionManager
    {
        const BindingFlags TARGET_METHOD_FLAGS = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool hasInjected()
        {
            return false;
        }

        /// <summary>
        /// Injects all patches if it is not already done
        /// </summary>
        public static void TryInject()
        {
            if (hasInjected())
                return;

            Harmony harmony = new Harmony("com.Mod-Bot.Internal");
            MethodInfo[] methods = typeof(Injections).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo injectionSource in methods)
            {
                string[] splitName = injectionSource.Name.Split('_');
                string typeName = splitName[0];
                string methodName = splitName[1];
                string mode = splitName[2].ToLower();
                bool isTargetMethodSetProperty = mode.StartsWith("set");
                bool isTargetMethodGetProperty = mode.StartsWith("get");

                string typeNamePrefix = "";
                Type[] argumentTypes = null;
                bool hasGenericParameters = false;
                Type[] genericParameterTypes = null;

                ExtraInjectionDataAttribute injectionExtraData = injectionSource.GetCustomAttribute<ExtraInjectionDataAttribute>();
                if (injectionExtraData != null)
                {
                    if (!string.IsNullOrWhiteSpace(injectionExtraData.Namespace))
                        typeNamePrefix = injectionExtraData.Namespace + ".";

                    if (injectionExtraData.ArgumentTypes != null)
                        argumentTypes = injectionExtraData.ArgumentTypes;

                    if (injectionExtraData.HasGenericParameters)
                    {
                        hasGenericParameters = true;
                        genericParameterTypes = injectionExtraData.GenericParameterTypes;
                    }
                }

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Type type = null;
                foreach (Assembly assembly in assemblies)
                {
                    type = assembly.GetType(typeNamePrefix + typeName);

                    if (type != null)
                        break;
                }

                if (type == null)
                    throw new Exception("Type \"" + typeName + "\" not found!");

                MethodInfo targetMethod;
                try
                {
                    if (isTargetMethodGetProperty)
                    {
                        targetMethod = type.GetProperty(methodName).GetGetMethod(false);
                    }
                    else if (isTargetMethodSetProperty)
                    {
                        targetMethod = type.GetProperty(methodName).SetMethod;
                    }
                    else
                    {
                        if (argumentTypes != null)
                        {
                            targetMethod = type.GetMethod(methodName, TARGET_METHOD_FLAGS, null, argumentTypes, null);
                        }
                        else
                        {
                            targetMethod = type.GetMethod(methodName, TARGET_METHOD_FLAGS);
                        }
                    }
                }
                catch (AmbiguousMatchException ambiguousMatch)
                {
                    List<MethodInfo> matchingMethods = type.GetMethods(TARGET_METHOD_FLAGS).Where((MethodInfo m) => m.Name == methodName).ToList();
                    if (argumentTypes != null)
                    {
                        for (int i = 0; i < matchingMethods.Count;)
                        {
                            ParameterInfo[] parameters = matchingMethods[i].GetParameters();
                            if (parameters.Length != argumentTypes.Length)
                            {
                                matchingMethods.RemoveAt(i);
                                continue;
                            }

                            bool matching = true;
                            for (int j = 0; j < parameters.Length; j++)
                            {
                                if (parameters[j].ParameterType != argumentTypes[j])
                                {
                                    matching = false;
                                    break;
                                }
                            }

                            if (!matching)
                            {
                                matchingMethods.RemoveAt(i);
                                continue;
                            }

                            i++;
                        }
                    }

                    if (matchingMethods.Count > 1)
                    {
                        if (hasGenericParameters)
                        {
                            matchingMethods.RemoveAll((MethodInfo m) => !m.ContainsGenericParameters);

                            if (matchingMethods.Count == 1)
                            {
                                targetMethod = matchingMethods[0];
                            }
                            else if (matchingMethods.Count == 0)
                            {
                                throw ambiguousMatch;
                            }
                            else
                            {
                                matchingMethods.RemoveAll((MethodInfo m) => m.GetGenericArguments().Length != genericParameterTypes.Length);

                                if (matchingMethods.Count == 1)
                                {
                                    targetMethod = matchingMethods[0].MakeGenericMethod(genericParameterTypes);
                                }
                                else if (matchingMethods.Count == 0)
                                {
                                    throw ambiguousMatch;
                                }
                                else
                                {
                                    throw ambiguousMatch;
                                }
                            }
                        }
                        else
                        {
                            matchingMethods.RemoveAll((MethodInfo m) => m.ContainsGenericParameters);

                            if (matchingMethods.Count == 1)
                            {
                                targetMethod = matchingMethods[0];
                            }
                            else if (matchingMethods.Count == 0)
                            {
                                throw ambiguousMatch;
                            }
                            else
                            {
                                throw ambiguousMatch;
                            }
                        }
                    }
                    else if (matchingMethods.Count == 0)
                    {
                        throw ambiguousMatch;
                    }
                    else
                    {
                        targetMethod = matchingMethods[0];
                    }
                }

                if (targetMethod == null)
                    throw new MissingMethodException(typeName, methodName);

                HarmonyMethod prefix = null;
                if (mode == "prefix")
                    prefix = new HarmonyMethod(injectionSource);

                HarmonyMethod postfix = null;
                if (mode == "postfix")
                    postfix = new HarmonyMethod(injectionSource);

                //debug.Log("Injecting " + mode + " into '" + targetMethod.FullDescription() + "' from '" + injectionSource.FullDescription() + "'");

                harmony.Patch(targetMethod, prefix, postfix);

                //debug.Log("Injected " + mode + " into '" + targetMethod.FullDescription() + "' from '" + injectionSource.FullDescription() + "'");
            }
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        sealed class ExtraInjectionDataAttribute : Attribute
        {
            public ExtraInjectionDataAttribute()
            {
            }

            public string Namespace { get; set; }

            public Type[] ArgumentTypes { get; set; }

            public bool HasGenericParameters { get; set; }

            public Type[] GenericParameterTypes { get; set; }
        }

        static class Injections
        {
            [ExtraInjectionData(Namespace = "InternalModBot")]
            public static bool ModBotHarmonyInjectionManager_hasInjected_Postfix(bool __result)
            {
                return true;
            }

            public static void FirstPersonMover_RefreshUpgrades_Prefix(FirstPersonMover __instance)
            {
                if (__instance == null || __instance.gameObject == null || !__instance.IsAlive() || __instance.GetCharacterModel() == null)
                    return;

                UpgradeCollection upgrade = __instance.GetComponent<UpgradeCollection>();
                ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(__instance, upgrade);
            }

            public static void FirstPersonMover_ExecuteCommand_Prefix(FirstPersonMover __instance, ref Command command)
            {
                FPMoveCommand moveCommand = (FPMoveCommand)command;

                if (!CharacterInputRestrictor.HasAnyRestrictions(__instance))
                    return;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.Jump))
                    moveCommand.Input.Jump = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalCursorMovement))
                {
                    moveCommand.Input.VerticalCursorMovement = 0f;
                }
                else
                {
                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalCursorMovementUp) && moveCommand.Input.VerticalCursorMovement > 0f)
                        moveCommand.Input.VerticalCursorMovement = 0f;

                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalCursorMovementDown) && moveCommand.Input.VerticalCursorMovement < 0f)
                        moveCommand.Input.VerticalCursorMovement = 0f;
                }

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalCursorMovement))
                {
                    moveCommand.Input.HorizontalCursorMovement = 0f;
                }
                else
                {
                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalCursorMovementLeft) && moveCommand.Input.HorizontalCursorMovement < 0f)
                        moveCommand.Input.HorizontalCursorMovement = 0f;

                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalCursorMovementRight) && moveCommand.Input.HorizontalCursorMovement > 0f)
                        moveCommand.Input.HorizontalCursorMovement = 0f;
                }

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.AttackKeyDown))
                    moveCommand.Input.AttackKeyDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.AttackKeyUp))
                    moveCommand.Input.AttackKeyUp = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.AttackKeyHeld))
                    moveCommand.Input.AttackKeyHeld = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SecondAttackKeyDown))
                    moveCommand.Input.SecondAttackDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SecondAttackKeyUp))
                    moveCommand.Input.SecondAttackUp = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SecondAttackKeyHeld))
                    moveCommand.Input.SecondAttackHeld = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.JetpackKeyHeld))
                    moveCommand.Input.JetpackHeld = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.ScrollWheelDelta))
                    moveCommand.Input.ScrollWheelDelta = 0f;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.UseAbilityKeyDown))
                    moveCommand.Input.UseAbilityDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.UseAbilityKeyHeld))
                    moveCommand.Input.UseAbilityHeld = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.NextAbilityKeyDown))
                    moveCommand.Input.NextAbilityDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.UseKeyDown))
                    moveCommand.Input.UseKeyDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.TransferConsciousnessKeyDown))
                    moveCommand.Input.TransferConsciousnessDown = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SwitchToWeapon1KeyDown))
                    moveCommand.Input.Weapon1 = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SwitchToWeapon2KeyDown))
                    moveCommand.Input.Weapon2 = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SwitchToWeapon3KeyDown))
                    moveCommand.Input.Weapon3 = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SwitchToWeapon4KeyDown))
                    moveCommand.Input.Weapon4 = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.SwitchToWeapon5KeyDown))
                    moveCommand.Input.Weapon5 = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.NextWeaponKeyDown))
                    moveCommand.Input.NextWeapon = false;

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalMovement))
                {
                    moveCommand.Input.VerticalMovement = 0f;
                }
                else
                {
                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalMovementForward) && moveCommand.Input.VerticalMovement > 0f)
                        moveCommand.Input.VerticalMovement = 0f;

                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.VerticalMovementBackwards) && moveCommand.Input.VerticalMovement < 0f)
                        moveCommand.Input.VerticalMovement = 0f;
                }

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalMovement))
                {
                    moveCommand.Input.HorizontalMovement = 0f;
                }
                else
                {
                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalMovementLeft) && moveCommand.Input.HorizontalMovement < 0f)
                        moveCommand.Input.HorizontalMovement = 0f;

                    if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.HorizontalMovementRight) && moveCommand.Input.HorizontalMovement > 0f)
                        moveCommand.Input.HorizontalMovement = 0f;
                }

                if (CharacterInputRestrictor.HasRestrictions(__instance, InputRestrictions.EmoteKeyHeld))
                    moveCommand.Input.IsEmoteKeyHeld = false;
            }

            public static void Character_Start_Prefix(Character __instance)
            {
                ModsManager.Instance.PassOnMod.OnCharacterSpawned(__instance);
            }

            public static void Character_Update_Prefix(Character __instance)
            {
                ModsManager.Instance.PassOnMod.OnCharacterUpdate(__instance);
            }

            public static void Character_onDeath_Prefix(Character __instance, Character killer, DamageSourceType damageSourceType, int attackID)
            {
                ModsManager.Instance.PassOnMod.OnCharacterKilled(__instance, killer, damageSourceType, attackID);
            }

            public static bool UpgradeUI_CreateChildIcons_Prefix(UpgradeDescription root, Vector2 rootPosition, float parentAngle, float angleIncrement, int recursionLevel,
                                                             ref UpgradeDescription ____emptyUpgradeDescriptionForConsumablesBranch,
                                                             ref bool ____isInStoryCutsceneMode,
                                                             ref bool ____authoringChallengeUpgrades,
                                                             ref List<UpgradeUIIcon> ____icons)
            {
                bool isConsumablesBranch = root == ____emptyUpgradeDescriptionForConsumablesBranch;
                List<UpgradeDescription> list;
                if (!isConsumablesBranch)
                {
                    list = UpgradeManager.Instance.GetVisibleUpgradesWithRequirement(root);
                }
                else
                {
                    list = UpgradeManager.Instance.GetVisibleUpgradesWithRequirement(null).Where((UpgradeDescription upgrade) => upgrade.IsConsumable).ToList();
                }

                List<UpgradeDescription> list2 = list;

                bool hasConsumablesNotInConsumablesBranch = list2.Count((UpgradeDescription description) => description.IsConsumable) > 1 && !isConsumablesBranch;

                if (hasConsumablesNotInConsumablesBranch)
                    list2.RemoveAll((UpgradeDescription description) => description.IsConsumable);

                float num = angleIncrement;
                float num2;
                if (recursionLevel > 0)
                {
                    if (isConsumablesBranch)
                    {
                        if (list2.Count >= 3)
                        {
                            num2 = GameUIRoot.Instance.UpgradeUI.Consumables3PlusChildRadius;
                            num = GameUIRoot.Instance.UpgradeUI.Consumables3PlusChildAngle * 0.0174532924f;
                        }
                        else
                        {
                            num2 = GameUIRoot.Instance.UpgradeUI.UpgradeLevelRadius;
                            num = GameUIRoot.Instance.UpgradeUI.ConsumablesChildAngle * 0.0174532924f;
                        }
                    }
                    else
                    {
                        num = GameUIRoot.Instance.UpgradeUI.ChildAngle * 0.0174532924f;
                        num2 = GameUIRoot.Instance.UpgradeUI.UpgradeLevelRadius;
                    }
                }
                else
                {
                    if (hasConsumablesNotInConsumablesBranch)
                        list2.Add(____emptyUpgradeDescriptionForConsumablesBranch);

                    num2 = GameUIRoot.Instance.UpgradeUI.FirstCircleRadius;

                    if (____isInStoryCutsceneMode)
                        num2 += GameUIRoot.Instance.UpgradeUI.Chapter3RadiusDelta;
                }

                float num3 = (list2.Count - 1) * num;

                for (int i = 0; i < list2.Count; i++)
                {
                    float num4 = parentAngle - (num3 * 0.5f) + (num * i) + (UpgradePagesManager.GetAngleOfUpgrade(list2[i].UpgradeType, list2[i].Level) * 0.0174532924f);
                    bool flag3 = list2[i] == ____emptyUpgradeDescriptionForConsumablesBranch;
                    float num5 = flag3 ? GameUIRoot.Instance.UpgradeUI.ConsumablesBranchRadius : num2;

                    if (list2[i].ApplyRadiusOffsetIfVisible == null || list2[i].ApplyRadiusOffsetIfVisible.IsUpgradeCurrentlyVisible())
                        num5 += list2[i].RadiusOffset;

                    RectTransform rectTransform = UnityEngine.Object.Instantiate(GameUIRoot.Instance.UpgradeUI.IconLinePrefab);
                    rectTransform.SetParent(GameUIRoot.Instance.UpgradeUI.IconContainer, false);
                    rectTransform.SetAsFirstSibling();
                    rectTransform.anchoredPosition = rootPosition;
                    rectTransform.localEulerAngles = new Vector3(0f, 0f, num4 * 57.29578f);
                    rectTransform.sizeDelta = new Vector2(num5, rectTransform.sizeDelta.y);
                    Vector2 vector = rootPosition + (num5 * new Vector2(Mathf.Cos(num4), Mathf.Sin(num4)));
                    if (!flag3)
                    {
                        UpgradeUIIcon upgradeUIIcon = UnityEngine.Object.Instantiate(____authoringChallengeUpgrades ? GameUIRoot.Instance.UpgradeUI.UpgradeConfigIconPrefab : GameUIRoot.Instance.UpgradeUI.IconPrefab);
                        upgradeUIIcon.transform.SetParent(GameUIRoot.Instance.UpgradeUI.IconContainer, false);
                        upgradeUIIcon.GetComponent<RectTransform>().anchoredPosition = vector;
                        upgradeUIIcon.Populate(list2[i], rectTransform.GetComponent<Image>());
                        ____icons.Add(upgradeUIIcon);
                    }

                    Accessor.CallPrivateMethod("CreateChildIcons", GameUIRoot.Instance.UpgradeUI, new object[] { list2[i], vector, num4, angleIncrement, recursionLevel + 1 });
                }

                return false;
            }

            public static bool UpgradeDescription_IsUpgradeCurrentlyVisible_Postfix(bool __result, UpgradeDescription __instance)
            {
                if (!UpgradePagesManager.IsUpgradeVisible(__instance.UpgradeType, __instance.Level))
                    return false;

                if (UpgradePagesManager.ForceUpgradeVisible(__instance.UpgradeType, __instance.Level))
                    return true;

                return __result;
            }

            public static bool ErrorManager_HandleLog_Prefix()
            {
                return !IgnoreCrashesManager.GetIsIgnoringCrashes();
            }

            #region Remove when removing projectile events
            public static void Projectile_FixedUpdate_Prefix(Projectile __instance)
            {
                if (!__instance.IsFlying())
                    return;

                ModsManager.Instance.PassOnMod.OnProjectileUpdate(__instance);
            }

            [ExtraInjectionData(ArgumentTypes = new Type[] { typeof(Vector3), typeof(Vector3), typeof(bool), typeof(Character), typeof(int), typeof(float) })]
            public static void Projectile_StartFlying_Postfix(Projectile __instance)
            {
                ModsManager.Instance.PassOnMod.OnProjectileStartedMoving(__instance);
            }

            public static void Projectile_DestroyProjectile_Prefix(Projectile __instance)
            {
                ModsManager.Instance.PassOnMod.OnProjectileDestroyed(__instance);
            }

            public static void Projectile_OnEnvironmentCollided_Prefix(Projectile __instance)
            {
                if (__instance.PassThroughEnvironment || !__instance.IsFlying())
                    return;

                if (__instance.MainCollider != null)
                    __instance.MainCollider.enabled = false;

                ModsManager.Instance.PassOnMod.OnProjectileDestroyed(__instance);
            }
            #endregion

            public static bool GameUIRoot_RefreshCursorEnabled_Prefix()
            {
                if (RegisterShouldCursorBeEnabledDelegate.ShouldMouseBeEnabled() || ModsManager.Instance == null || ModsManager.Instance.PassOnMod.ShouldCursorBeEnabled())
                {
                    InputManager.Instance.SetCursorEnabled(true);
                    return false;
                }

                return true;
            }
            
            [ExtraInjectionData(Namespace = "UnityEngine", HasGenericParameters = false, ArgumentTypes = new Type[] { typeof(string) })]
            public static UnityEngine.Object Resources_Load_Postfix(UnityEngine.Object __result, string path)
            {
                UnityEngine.Object moddedResource = LevelEditorObjectAdder.GetObjectData(path);
                if (moddedResource != null)
                    return moddedResource;

                if (ModsManager.Instance != null)
                {
                    moddedResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
                    if (moddedResource != null)
                        return moddedResource;
                }

                return __result;
            }

            // TODO: Harmony does not like patching generic methods, fix this eventually, for now it will be injected in Injector.exe
            /*
            [ExtraInjectionData(Namespace = "UnityEngine", HasGenericParameters = true, GenericParameterTypes = new Type[] { typeof(UnityEngine.Object) }, ArgumentTypes = new Type[] { typeof(string) })]
            public static UnityEngine.Object Resources_Load_Postfix_T(UnityEngine.Object __result, string path)
            {
                UnityEngine.Object moddedResource = LevelEditorObjectAdder.GetObjectData(path);
                if (moddedResource != null)
                    return moddedResource;

                if (ModsManager.Instance != null)
                {
                    moddedResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
                    if (moddedResource != null)
                        return moddedResource;
                }

                debug.Log(path + " loaded (" + __result.GetType().Name + ")");

                return __result;
            }
            */

            // TODO: For some reason this doesn't want to inject, fix this eventually, for now it will be injected in Injector.exe
            /*
            [ExtraInjectionData(Namespace = "UnityEngine")]
            public static UnityEngine.Object ResourceRequest_asset_GetPrefix(UnityEngine.Object __result, ref string ___m_Path)
            {
                debug.Log(___m_Path + " loaded async");

                UnityEngine.Object moddedResource = LevelEditorObjectAdder.GetObjectData(___m_Path);
                if (moddedResource != null)
                    return moddedResource;

                if (ModsManager.Instance != null)
                {
                    moddedResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(___m_Path);
                    if (moddedResource != null)
                        return moddedResource;
                }

                return __result;
            }
            */

            public static void LocalizationManager_populateDictionaryForCurrentLanguage_Postfix(ref Dictionary<string, string> ____translatedStringsDictionary)
            {
                ModBotLocalizationManager.AddAllLocalizationStringsToDictionary(____translatedStringsDictionary);
                ModsManager.Instance.PassOnMod.OnLanugageChanged(SettingsManager.Instance.GetCurrentLanguageID(), ____translatedStringsDictionary);
            }

            public static bool TwitchWhoIsLiveManager_onGameLiveStreamRequestFinished_Prefix(HTTPResponse response, ref List<TwitchStreamInfo> ____streamInfos, ref bool ____hasEverRetirevedLiveUsers)
            {
                if (response == null || !response.IsSuccess)
                    return false;

                try
                {
                    ____streamInfos = new List<TwitchStreamInfo>();

                    foreach (JToken jtoken in JsonConvert.DeserializeObject<JContainer>(response.DataAsText).First.First.Children())
                    {
                        TwitchStreamInfo twitchStreamInfo = jtoken.ToObject<TwitchStreamInfo>();
                        ____streamInfos.Add(twitchStreamInfo);
                    }

                    ____hasEverRetirevedLiveUsers = true;

                    GlobalEventManager.Instance.Dispatch(GlobalEvents.TwitchLiveStreamsRefreshed);
                }
                catch (Exception exception)
                {
                    ErrorManager.Instance.LogExceptionWithoutPausingGame(exception, "onGameLiveStreamRequestFinished Error");
                }

                return false;
            }
        }
    }
}