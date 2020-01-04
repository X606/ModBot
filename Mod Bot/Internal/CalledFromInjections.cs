using InternalModBot;
using ModLibrary;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using ModLibrary.Properties;

namespace InternalModBot
{
    /// <summary>
    /// Contains methods that get called from the game itself
    /// </summary>
    public static class CalledFromInjections
    {
        /// <summary>
        /// Called at the start of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromRefreshUpgradesStart(FirstPersonMover owner)
        {
            if (owner == null || owner.gameObject == null || !owner.IsAlive() || owner.GetCharacterModel() == null)
                return;

            UpgradeCollection upgrade = owner.GetComponent<UpgradeCollection>();
            ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(owner, upgrade);
        }

        /// <summary>
        /// Called at the end of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromRefreshUpgradesEnd(FirstPersonMover owner)
        {
            if (owner.gameObject == null)
                return;

            ModsManager.Instance.PassOnMod.AfterUpgradesRefreshed(owner, owner.GetComponent<UpgradeCollection>());
        }

        /// <summary>
        /// Called from <see cref="Character.Start"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterStart(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterSpawned(owner);
        }

        /// <summary>
        /// Called from <see cref="Character.Update"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterUpdate(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterUpdate(owner);
        }

        /// <summary>
        /// Called from <see cref="Character.onDeath(Character, DamageSourceType, int)"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="killer"></param>
        /// <param name="damageSource"></param>
        public static void FromOnCharacterDeath(Character owner, Character killer, DamageSourceType damageSource)
        {
            ModsManager.Instance.PassOnMod.OnCharacterKilled(owner, killer, damageSource);
        }

        /// <summary>
        /// Called from <see cref="UpgradeDescription.GetAngleOffset"/> and returns the angle the upgrade should be on the current page
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static float FromGetAngleOffset(UpgradeDescription upgrade)
        {
            return UpgradePagesManager.GetAngleOfUpgrade(upgrade.UpgradeType, upgrade.Level);
        }

        /// <summary>
        /// Called from <see cref="UpgradeDescription.IsUpgradeCurrentlyVisible"/> and returns if the upgrade should be visible on the current page
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static bool FromIsUpgradeCurrentlyVisible(UpgradeDescription upgrade)
        {
            if (!UpgradePagesManager.IsUpgradeVisible(upgrade.UpgradeType, upgrade.Level))
                return false;

            if (UpgradePagesManager.ForceUpgradeVisible(upgrade.UpgradeType, upgrade.Level))
                return true;

            return (GameModeManager.ShowsStoryBlockedUpgrades() || (!UpgradeManager.Instance.IsUpgradeLockedByCurrentMetagameProgress(upgrade) && !upgrade.HideInStoryMode)) && (!GameModeManager.IsMultiplayerDuel() || upgrade.IsAvailableInDuels) && (!GameModeManager.IsBattleRoyale() || upgrade.IsAvailableInBattleRoyale) && (!GameModeManager.IsCoop() || upgrade.IsAvailableInCoop) && (upgrade.IsUpgradeVisible || GameModeManager.IsMultiplayer()) && upgrade.IsCompatibleWithCharacter(CharacterTracker.Instance.GetPlayer());
        }

        /// <summary>
        /// Called from <see cref="Projectile.FixedUpdate"/>
        /// </summary>
        /// <param name="projectile"></param>
        public static void FromFixedUpdate(Projectile projectile)
        {
            if (!projectile.IsFlying())
                return;

            ModsManager.Instance.PassOnMod.OnProjectileUpdate(projectile);
        }

        /// <summary>
        /// Called from <see cref="Projectile.StartFlying(Vector3, Vector3, bool, Character)"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromStartFlying(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileStartedMoving(arrow);
        }

        /// <summary>
        /// Called from <see cref="Projectile.DestroyProjectile"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromDestroyProjectile(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileDestroyed(arrow);
        }

        /// <summary>
        /// Gets from <see cref="Projectile.OnEnvironmentCollided(bool)"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromOnEnvironmentCollided(Projectile arrow)
        {
            if (arrow.PassThroughEnvironment)
                return;

            if (!arrow.IsFlying())
                return;

            if (arrow.MainCollider != null)
                arrow.MainCollider.enabled = false;

            ModsManager.Instance.PassOnMod.OnProjectileDestroyed(arrow);
        }

        /// <summary>
        /// Called from <see cref="GameUIRoot.RefreshCursorEnabled"/> and returns if the cursor has been disabled by a mod
        /// </summary>
        /// <returns></returns>
        public static bool FromRefreshCursorEnabled()
        {
            if (RegisterShouldCursorBeEnabledDelegate.ShouldMouseBeEnabled() || ModsManager.Instance == null || ModsManager.Instance.PassOnMod.ShouldCursorBeEnabled())
            {
                InputManager.Instance.SetCursorEnabled(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called from <see cref="MortarWalker.GetPositionForAIToAimAt"/>
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Vector3 FromGetPositionForAIToAimAt(Character character)
        {
            List<MechBodyPart> powerCrystals = character.GetBodyParts(MechBodyPartType.PowerCrystal);
            if (powerCrystals.Count == 0)
                return character.transform.position;

            return powerCrystals[0].transform.position;
        }

        /// <summary>
        /// Called from <see cref="UnityEngine.Resources.Load(string)"/>, <see cref="UnityEngine.Resources.Load{T}(string)"/> and <see cref="ResourceRequest.asset"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object FromResourcesLoad(string path)
        {
            UnityEngine.Object levelEditorObject = LevelEditorObjectAdder.GetObjectData(path);

            if (levelEditorObject != null)
                return levelEditorObject;

            if(ModsManager.Instance == null)
                return null;

            return ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
        }

        /// <summary>
        /// Called from <see cref="FirstPersonMover.ExecuteCommand(Bolt.Command, bool)"/> and sets the <see cref="FPMoveCommand.Input"/> properties to 0 or <see langword="false"/> is they have been restricted
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static FPMoveCommand FromExecuteCommand(FirstPersonMover owner, Bolt.Command command)
        {
            FPMoveCommand moveCommand = (FPMoveCommand)command;

            if (!CharacterInputRestrictor.HasAnyRestrictions(owner))
                return moveCommand;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.Jump))
                moveCommand.Input.Jump = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalCursorMovement))
            {
                moveCommand.Input.VerticalCursorMovement = 0f;
            }
            else
            {
                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalCursorMovementUp) && moveCommand.Input.VerticalCursorMovement > 0f)
                    moveCommand.Input.VerticalCursorMovement = 0f;

                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalCursorMovementDown) && moveCommand.Input.VerticalCursorMovement < 0f)
                    moveCommand.Input.VerticalCursorMovement = 0f;
            }

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalCursorMovement))
            {
                moveCommand.Input.HorizontalCursorMovement = 0f;
            }
            else
            {
                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalCursorMovementLeft) && moveCommand.Input.HorizontalCursorMovement < 0f)
                    moveCommand.Input.HorizontalCursorMovement = 0f;

                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalCursorMovementRight) && moveCommand.Input.HorizontalCursorMovement > 0f)
                    moveCommand.Input.HorizontalCursorMovement = 0f;
            }

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.AttackKeyDown))
                moveCommand.Input.AttackKeyDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.AttackKeyUp))
                moveCommand.Input.AttackKeyUp = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.AttackKeyHeld))
                moveCommand.Input.AttackKeyHeld = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SecondAttackKeyDown))
                moveCommand.Input.SecondAttackDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SecondAttackKeyUp))
                moveCommand.Input.SecondAttackUp = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SecondAttackKeyHeld))
                moveCommand.Input.SecondAttackHeld = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.JetpackKeyHeld))
                moveCommand.Input.JetpackHeld = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.ScrollWheelDelta))
                moveCommand.Input.ScrollWheelDelta = 0f;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.UseAbilityKeyDown))
                moveCommand.Input.UseAbilityDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.UseAbilityKeyHeld))
                moveCommand.Input.UseAbilityHeld = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.NextAbilityKeyDown))
                moveCommand.Input.NextAbilityDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.UseKeyDown))
                moveCommand.Input.UseKeyDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.TransferConsciousnessKeyDown))
                moveCommand.Input.TransferConsciousnessDown = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SwitchToWeapon1KeyDown))
                moveCommand.Input.Weapon1 = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SwitchToWeapon2KeyDown))
                moveCommand.Input.Weapon2 = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SwitchToWeapon3KeyDown))
                moveCommand.Input.Weapon3 = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SwitchToWeapon4KeyDown))
                moveCommand.Input.Weapon4 = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.SwitchToWeapon5KeyDown))
                moveCommand.Input.Weapon5 = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.NextWeaponKeyDown))
                moveCommand.Input.NextWeapon = false;

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalMovement))
            {
                moveCommand.Input.VerticalMovement = 0f;
            }
            else
            {
                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalMovementForward) && moveCommand.Input.VerticalMovement > 0f)
                    moveCommand.Input.VerticalMovement = 0f;

                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.VerticalMovementBackwards) && moveCommand.Input.VerticalMovement < 0f)
                    moveCommand.Input.VerticalMovement = 0f;
            }

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalMovement))
            {
                moveCommand.Input.HorizontalMovement = 0f;
            }
            else
            {
                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalMovementLeft) && moveCommand.Input.HorizontalMovement < 0f)
                    moveCommand.Input.HorizontalMovement = 0f;

                if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.HorizontalMovementRight) && moveCommand.Input.HorizontalMovement > 0f)
                    moveCommand.Input.HorizontalMovement = 0f;
            }

            if (CharacterInputRestrictor.HasRestrictions(owner, InputRestrictions.EmoteKeyHeld))
                moveCommand.Input.IsEmoteKeyHeld = false;

            return moveCommand;
        }

        /// <summary>
        /// Called from LocalizationManager.populateDictionaryForCurrentLanguage
        /// </summary>
        public static void FromPopulateLanguageDictionary()
        {
            Dictionary<string, string> translatedStrings = Accessor.GetPrivateField<LocalizationManager, Dictionary<string, string>>("_translatedStringsDictionary", LocalizationManager.Instance);

            ModBotLocalizationManager.AddAllLocalizationStringsToDictionary(translatedStrings);
            ModsManager.Instance.PassOnMod.OnLanugageChanged(SettingsManager.Instance.GetCurrentLanguageID(), translatedStrings);
        }

    }

}
