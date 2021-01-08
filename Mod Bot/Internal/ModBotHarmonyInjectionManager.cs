using BestHTTP;
using Bolt;
using HarmonyLib;
using ModLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
#pragma warning disable CS0618 // We dont care if its depricated since its ours

namespace InternalModBot
{
    /// <summary>
    /// Handles all of Mod-Bots runtils patching
    /// </summary>
    public static class ModBotHarmonyInjectionManager
    {
        const BindingFlags TARGET_METHOD_FLAGS = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Injects all patches if it is not already done
        /// </summary>
        public static void TryInject()
        {
            Harmony harmony = new Harmony("com.Mod-Bot.Internal");
            if (harmony.GetPatchedMethods().Any())
                return;

            MethodInfo[] methods = typeof(Injections).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo injectionSource in methods)
            {
                string[] splitName = injectionSource.Name.Split('_');
                string typeName = splitName[0];
                string methodName = splitName[1];
                string mode = splitName[2].ToLower();
                bool isTargetMethodSetProperty = mode.StartsWith("set");
                bool isTargetMethodGetProperty = mode.StartsWith("get");

                if (isTargetMethodSetProperty || isTargetMethodGetProperty)
                    mode = mode.Remove(0, 3); // Remove the first 3 characters (ie. "get" or "set")

                string typeNamePrefix = "";
                Type[] argumentTypes = null;
                bool hasGenericParameters = false;
                Type[] genericParameterTypes = null;
                BindingFlags bindingFlags = BindingFlags.Default;

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

                    bindingFlags = injectionExtraData.BindingFlagsOverride;
                }

                if (bindingFlags == BindingFlags.Default)
                    bindingFlags = TARGET_METHOD_FLAGS;

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
                        targetMethod = type.GetProperty(methodName, bindingFlags).GetMethod;
                    }
                    else if (isTargetMethodSetProperty)
                    {
                        targetMethod = type.GetProperty(methodName, bindingFlags).SetMethod;
                    }
                    else
                    {
                        if (argumentTypes != null)
                        {
                            targetMethod = type.GetMethod(methodName, bindingFlags, null, argumentTypes, null);
                        }
                        else
                        {
                            targetMethod = type.GetMethod(methodName, bindingFlags);
                        }
                    }
                }
                catch (AmbiguousMatchException ambiguousMatch)
                {
                    List<MethodInfo> matchingMethods = type.GetMethods(bindingFlags).Where((MethodInfo m) => m.Name == methodName).ToList();
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
                                targetMethod = matchingMethods[0].MakeGenericMethod(genericParameterTypes);
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

            public BindingFlags BindingFlagsOverride { get; set; }
        }

        static class Injections
        {
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

            public static float UpgradeDescription_GetAngleOffset_Postfix(float __result, UpgradeDescription __instance)
            {
                return UpgradePagesManager.GetAngleOfUpgrade(__instance.UpgradeType, __instance.Level);
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

            /* Harmony REALLY does not like generic methods, I have given up on trying to make this work, it will continue being in Injector.exe
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

                return __result;
            }
            */

            [ExtraInjectionData(Namespace = "UnityEngine")]
            public static UnityEngine.Object ResourceRequest_asset_GetPostfix(UnityEngine.Object __result, ref string ___m_Path)
            {
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

            public static void LocalizationManager_populateDictionaryForCurrentLanguage_Postfix(ref Dictionary<string, string> ____translatedStringsDictionary)
            {
                ModBotLocalizationManager.AddAllLocalizationStringsToDictionary(____translatedStringsDictionary);
                ModsManager.Instance.PassOnMod.OnLanguageChanged(SettingsManager.Instance.GetCurrentLanguageID(), ____translatedStringsDictionary);
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

            public static bool MultiplayerPlayerInfoState_GetOrPrepareSafeDisplayName_Prefix(MultiplayerPlayerInfoState __instance, Action<string> onSafeDisplayNameReceived, ref bool ____isSafeDisplayNameBeingPrepared)
            {
                if (string.IsNullOrEmpty(Accessor.GetPrivateProperty<MultiplayerPlayerInfoState, string>("SafeDisplayName", __instance)))
                {
                    Action<string> singleCallback = null;
                    singleCallback = delegate (string safeDisplayName)
                    {
                        __instance.OnSafeDisplayNamePrepared -= singleCallback;
                        Action<string> onSafeDisplayNameReceived3 = onSafeDisplayNameReceived;
                        if (onSafeDisplayNameReceived3 != null)
                            onSafeDisplayNameReceived3(MultiplayerPlayerNameManager.GetFullPrefixForPlayfabID(__instance.state.PlayFabID) + MultiplayerPlayerNameManager.GetNameForPlayfabID(__instance.state.PlayFabID, safeDisplayName));
                    };

                    __instance.OnSafeDisplayNamePrepared += singleCallback;
                    
                    if (____isSafeDisplayNameBeingPrepared)
                        return false;

                    ____isSafeDisplayNameBeingPrepared = true;
                    ProfanityChecker.FilterForProfanities(__instance.state.DisplayName, delegate (string preparedName)
                    {
                        Accessor.SetPrivateProperty("SafeDisplayName", __instance, preparedName);
                        Accessor.SetPrivateField("_isSafeDisplayNameBeingPrepared", __instance, false);

                        EventInfo safeDisplayNamePreparedEvent = typeof(MultiplayerPlayerInfoState).GetEvent("OnSafeDisplayNamePrepared", BindingFlags.Public | BindingFlags.Instance);
                        if (safeDisplayNamePreparedEvent.RaiseMethod != null)
                            safeDisplayNamePreparedEvent.RaiseMethod.Invoke(__instance, new object[] { Accessor.GetPrivateProperty<MultiplayerPlayerInfoState, string>("SafeDisplayName", __instance) });
                    });
                }
                else
                {
                    Action<string> onSafeDisplayNameReceived2 = onSafeDisplayNameReceived;
                    if (onSafeDisplayNameReceived2 != null)
                        onSafeDisplayNameReceived2(MultiplayerPlayerNameManager.GetFullPrefixForPlayfabID(__instance.state.PlayFabID) + MultiplayerPlayerNameManager.GetNameForPlayfabID(__instance.state.PlayFabID, Accessor.GetPrivateProperty<MultiplayerPlayerInfoState, string>("SafeDisplayName", __instance)));
                }

                return false;
            }

            public static bool CharacterNameTagManager_updateNameTag_Prefix(Character character)
            {
                if (!GameModeManager.ShouldCreateNameTagsForOtherPlayers())
                    return false;
                
                MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(character.state.PlayFabID, delegate (string displayName)
                {
                    if (displayName != null && !character.HasNameTag())
                    {
                        displayName = MultiplayerPlayerNameManager.GetFullPrefixForPlayfabID(character.state.PlayFabID) + MultiplayerPlayerNameManager.GetNameForPlayfabID(character.state.PlayFabID, displayName);
                        CharacterNameTagManager.Instance.StartCoroutine(Accessor.CallPrivateMethod<CharacterNameTagManager, IEnumerator>("addNameTagWithDelay", CharacterNameTagManager.Instance, new object[] { character, displayName }));
                    }
                });

                return false;
            }

            public static void MultiplayerPlayerInfoLabel_RefreshVisuals_Postfix(ref MultiplayerPlayerInfoState ____playerInfoState, Text ___PlayerNameLabel)
			{
				if(____playerInfoState == null || ____playerInfoState.IsDetached())
					return;

                MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(____playerInfoState.state.PlayFabID, delegate (string displayName)
                {
                    ___PlayerNameLabel.supportRichText = true; // allow custom colors
                    ___PlayerNameLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
                    ___PlayerNameLabel.text = displayName;
                });
			}
			
			public static void EnemyNameTag_Initialize_Postfix(EnemyNameTag __instance, Character character)
			{
				__instance.gameObject.AddComponent<NameTagRefreshListener>().Init(character, __instance);
			}
			public static void EnemyNameTag_OnDestroy_Postfix(EnemyNameTag __instance)
			{
				NameTagRefreshListener nameTagRefreshListener = __instance.GetComponent<NameTagRefreshListener>();
				Destroy(nameTagRefreshListener);
			}

			public static void CurrentlySpectatingUI_Show_Postfix(Text ___CurrentPlayerText)
			{
				___CurrentPlayerText.supportRichText = true;
			}

            public static void ErrorManager_SendDataToLoggly_Prefix(WWWForm form)
            {
                form.AddField("IsModdedClient", "true");

                string loadedModNames = string.Join<string>(", ", ModsManager.Instance.GetActiveModInfos().GetPropertyValues<ModInfo, string>("DisplayName"));
                form.AddField("LoadedMods", loadedModNames);
            }

            public static Vector3 Character_GetPositionToDeflectArrowsAt_Postfix(Vector3 __result, Character __instance)
            {
                if (__instance is MortarWalker)
                    return __instance.GetPositionForAIToAimAt(true);

                return __result;
            }
        }
    }
}