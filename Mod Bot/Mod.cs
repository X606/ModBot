using System;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Base class for all mods, contains virtual implementations for diffrent events in the game.
    /// </summary>
    public abstract class Mod
    {
        /// <summary>
        /// Called when the game scene is changed
        /// </summary>
        /// <param name="gamemode">The new gamemode</param>
        public virtual void OnSceneChanged(GameMode gamemode)
        {
        }

        /// <summary>
        /// Called in FirstPersonMover.Start()
        /// </summary>
        /// <param name="me">The GameObject with the corresponding FirstPersonMover component</param>
        public virtual void OnFirstPersonMoverSpawned(GameObject me)
        {
        }

        /// <summary>
        /// Called in FirstPersonMover.Update()
        /// </summary>
        /// <param name="me">The GameObject with the corresponding FirstPersonMover component</param>
        public virtual void OnFirstPersonMoverUpdate(GameObject me)
        {
        }

        /// <summary>
        /// Called in Character.Start()
        /// </summary>
        /// <param name="me">The GameObject with the corresponding Character component</param>
        public virtual void OnCharacterSpawned(GameObject me)
        {
        }

        /// <summary>
        /// Called in Character.Update()
        /// </summary>
        /// <param name="me">The GameObject with the corresponding FirstPersonMover component</param>
        public virtual void OnCharacterUpdate(GameObject me)
        {

        }

        /// <summary>
        /// Called in ModsManager.resetMods()
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
        /// Called when any object is placed in the level editor
        /// </summary>
        /// <param name="_object">The GameObject with the corresponding ObjectPlacedInLevel component</param>
        public virtual void OnObjectPlacedInLevelEditor(GameObject _object)
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
        /// Returns the description of the mod, override to change the description of your mod
        /// </summary>
        /// <returns></returns>
        public virtual string GetModDescription()
        {
            return "";
        }

        /// <summary>
        /// Returns the image displayed in the mods menu, override to set a custom image for your mod
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use GetModImageURL instead, this is never used")]
        public virtual Texture2D GetModImage()
        {
            return null;
        }

        public virtual string GetModImageURL()
        {
            return "";
        }

        /// <summary>
        /// Called when FirstPersonMover.RefreshUpgrades() is called.
        /// </summary>
        /// <param name="me">The GameObject with the corresponding FirstPersonMover component</param>
        /// <param name="upgrades">The UpgradeCollection on the FirstPersonMover object</param>
        public virtual void OnUpgradesRefreshed(GameObject me, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called at the end of FirstPersonMover.RefreshUpgrades()
        /// </summary>
        /// <param name="owner">The GameObject with the corresponding FirstPersonMover component</param>
        /// <param name="upgrades">The UpgradeCollection on the FirstPersonMover object</param>
        public virtual void AfterUpgradesRefreshed(GameObject owner, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called when a Projectile is created
        /// </summary>
        /// <param name="projectile">The projectile's GameObject</param>
        public virtual void OnProjectileCreated(GameObject projectile)
        {
        }

        /// <summary>
        /// Called when an ArrowProjectile is created
        /// </summary>
        /// <param name="arrow">The created ArrowProjectile</param>
        public virtual void OnArrowProjectileCreated(ArrowProjectile arrow)
        {
        }

        /// <summary>
        /// Called when a BulletProjectile is created
        /// </summary>
        /// <param name="bullet">The created BulletProjectile</param>
        public virtual void OnBulletProjectileCreated(BulletProjectile bullet)
        {
        }

        /// <summary>
        /// Called when a Projectile starts moving
        /// </summary>
        /// <param name="projectile">The projectile's GameObject</param>
        public virtual void OnProjectileStartedMoving(GameObject projectile)
        {
        }

        /// <summary>
        /// Called when an ArrowProjectile starts moving
        /// </summary>
        /// <param name="arrow"></param>
        public virtual void OnArrowProjectileStartedMoving(ArrowProjectile arrow)
        {
        }

        /// <summary>
        /// Called when a BulletProjectile starts moving
        /// </summary>
        /// <param name="bullet"></param>
        public virtual void OnBulletProjectileStartedMoving(BulletProjectile bullet)
        {
        }

        /// <summary>
        /// Called when a character is killed
        /// </summary>
        /// <param name="killedCharacter">The GameObject for the character that was killed</param>
        /// <param name="killerCharacter">The GameObject for the killer character</param>
        /// <param name="damageSourceType">The cause of death</param>
        public virtual void OnCharacterKilled(GameObject killedCharacter, GameObject killerCharacter, DamageSourceType damageSourceType)
        {

        }
    }
}
