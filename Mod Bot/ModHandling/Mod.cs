using System;
using UnityEngine;
using ModLibrary;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Base class for all mods, contains virtual implementations for different events in the game.
    /// </summary>
    public abstract class Mod
    {
        /// <summary>
        /// Called in <see cref="Character.Start"/> if the <see cref="Character"/> is of type <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> that was spawned</param>
        public virtual void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Update"/> if the <see cref="Character"/> is of type <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> that was updated</param>
        public virtual void OnFirstPersonMoverUpdate(FirstPersonMover firstPersonMover)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Start"/>
        /// </summary>
        /// <param name="character">The <see cref="Character"/> that was spawned</param>
        public virtual void OnCharacterSpawned(Character character)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Update"/>
        /// </summary>
        /// <param name="character">The <see cref="Character"/> that was updated</param>
        public virtual void OnCharacterUpdate(Character character)
        {
        }

        /// <summary>
        /// Called in <see cref="ModsManager.ReloadMods"/>
        /// </summary>
        public virtual void OnModRefreshed()
        {
        }

        /// <summary>
        /// Called when the level editor is started.
        /// </summary>
        public virtual void OnLevelEditorStarted()
        {
        }

        /// <summary>
        /// Called when you run a command in the console (mostly for debuging).
        /// </summary>
        /// <param name="command">The text entered into the command field of the console</param>
        public virtual void OnCommandRan(string command)
        {
        }

        /// <summary>
        /// Returns the name of the mod, override to set the name of you mod
        /// </summary>
        /// <returns></returns>
        public abstract string GetModName();

        /// <summary>
        /// Returns a unique ID for every mod
        /// </summary>
        /// <returns></returns>
        public abstract string GetUniqueID();

        /// <summary>
        /// Returns the description of the mod, override to change the description of your mod
        /// </summary>
        /// <returns></returns>
        public virtual string GetModDescription()
        {
            return "";
        }

        /// <summary>
        /// Returns the url to the image to be displayed in the mods menu, override to set a custom image for your mod
        /// </summary>
        /// <returns></returns>
        public virtual string GetModImageURL()
        {
            return "";
        }

        /// <summary>
        /// Called at the start <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades">The <see cref="UpgradeCollection"/> on the <see cref="FirstPersonMover"/> object</param>
        public virtual void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called at the end of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades">The <see cref="UpgradeCollection"/> on the <see cref="FirstPersonMover"/> object</param>
        public virtual void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.StartFlying(Vector3, Vector3, bool, Character, int, float)"/>
        /// </summary>
        /// <param name="projectile"></param>
        public virtual void OnProjectileStartedMoving(Projectile projectile)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.FixedUpdate"/>
        /// </summary>
        /// <param name="projectile"></param>
        public virtual void OnProjectileUpdate(Projectile projectile)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.DestroyProjectile"/> and <see cref="Projectile.OnEnvironmentCollided(bool)"/>
        /// </summary>
        /// <param name="projectile"></param>
        public virtual void OnProjectileDestroyed(Projectile projectile)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.StartFlying(Vector3, Vector3, bool, Character, int, float)"/> if the <see cref="Projectile"/> is of type <see cref="ArrowProjectile"/>
        /// </summary>
        /// <param name="arrow"></param>
        public virtual void OnArrowProjectileStartedMoving(ArrowProjectile arrow)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.FixedUpdate"/> if the <see cref="Projectile"/> is of type <see cref="ArrowProjectile"/>
        /// </summary>
        /// <param name="arrow"></param>
        public virtual void OnArrowProjectileUpdate(ArrowProjectile arrow)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.DestroyProjectile"/> and <see cref="Projectile.OnEnvironmentCollided(bool)"/> if the <see cref="Projectile"/> is of type <see cref="ArrowProjectile"/>
        /// </summary>
        /// <param name="arrow"></param>
        public virtual void OnArrowProjectileDestroyed(ArrowProjectile arrow)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.StartFlying(Vector3, Vector3, bool, Character, int, float)"/> if the <see cref="Projectile"/> is of type <see cref="BulletProjectile"/>
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isRepairFire"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        public virtual void OnBulletProjectileStartedMoving(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.FixedUpdate"/> if the <see cref="Projectile"/> is of type <see cref="BulletProjectile"/>
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isFlameBreath"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isRepairFire"></param>
        public virtual void OnBulletProjectileUpdate(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
        }

        /// <summary>
        /// Called in <see cref="Projectile.DestroyProjectile"/> and <see cref="Projectile.OnEnvironmentCollided(bool)"/> if the <see cref="Projectile"/> is of type <see cref="BulletProjectile"/>
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isRepairFire"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        public virtual void OnBulletProjectileDestroyed(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.onDeath(Character, DamageSourceType)"/>
        /// </summary>
        /// <param name="killedCharacter">The <see cref="Character"/> that was killed</param>
        /// <param name="killerCharacter">The killer <see cref="Character"/></param>
        /// <param name="damageSourceType">The cause of death</param>
        public virtual void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType)
        {
        }

        /// <summary>
        /// Called when the mod is deactivated from the mods menu, override this method to perform additional cleanup, like remove/disable all added components, reset values in classes, etc. Modded upgrades are automatically hidden when the owner mod is disabled
        /// </summary>
        public virtual void OnModDeactivated()
        {
        }

        /// <summary>
        /// If this returns <see langword="true"/> it will active the mod settings button in the mods window for this mod.
        /// </summary>
        /// <returns></returns>
        public virtual bool ImplementsSettingsWindow()
        {
            return false;
        }

        /// <summary>
        /// Gets called when the user clicks on the mod settings button in the mods window. Allows you to create a neat little UI that saves the values for you. Get the values set by this with SettingsManager.Instance.GetModdedSettingsBoolValue, GetModdedSettingsStringValue, GetModdedSettingsIntValue and GetModdedSettingsFloatValue
        /// </summary>
        /// <param name="builder">The object used to build the UI.</param>
        public virtual void CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
        }

        /// <summary>
        /// If this returns <see langword="true"/> the cursor will get enabled
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldCursorBeEnabled()
        {
            return false;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public virtual void GlobalUpdate()
        {
        }

        /// <summary>
        /// Called whenever we received a <see cref="GenericStringForModdingEvent"/> from a client (including your own if you used <see cref="MultiplayerMessageSender.SendToAllClients(string)"/>)
        /// </summary>
        /// <param name="moddedEvent">The received <see cref="GenericStringForModdingEvent"/></param>
        public virtual void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
        }

        /// <summary>
        /// Called when the mod gets loaded or enabled after previously being disabled
        /// </summary>
        public virtual void OnModEnabled()
        {
        }

        /// <summary>
        /// Will be called just before anything tries to load something from the Resources folder. If this returns <see langword="null"/> it will continue like normal, but if this returns anything else than <see langword="null"/> that will be returned by <see cref="Resources.Load(string)"/> instead.
        /// </summary>
        /// <param name="path">The path specified</param>
        /// <returns></returns>
        public virtual UnityEngine.Object OnResourcesLoad(string path)
        {
            return null;
        }
    }
}
