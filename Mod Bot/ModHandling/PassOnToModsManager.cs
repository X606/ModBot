using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to call events on all loaded active mods, you probably dont want to use this from mods
    /// </summary>
    public class PassOnToModsManager : Mod
    {
        /// <summary>
        /// Mods require us to override this one, but its never used
        /// </summary>
        /// <returns></returns>
        public override string GetModName()
        {
            return string.Empty;
        }
        /// <summary>
        ///  Mods require us to override this one, but its never used
        /// </summary>
        /// <returns></returns>
        public override string GetUniqueID()
        {
            return string.Empty;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        public override void OnFirstPersonMoverSpawned(FirstPersonMover me)
        {
            if (me.IsMainPlayer() && BoltNetwork.isRunning)
            {
                ModBotUserIdentifier.Instance.RequestIds(me);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverSpawned(me);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        public override void OnFirstPersonMoverUpdate(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverUpdate(me);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnModRefreshed()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModRefreshed();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnLevelEditorStarted()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLevelEditorStarted();
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="command"></param>
        public override void OnCommandRan(string command)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCommandRan(command);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        /// <param name="upgrades"></param>
        public override void OnUpgradesRefreshed(FirstPersonMover me, UpgradeCollection upgrades)
        {
            FirstPersonMover firstPersonMover = me.GetComponent<FirstPersonMover>();
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
            {
                return;
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnUpgradesRefreshed(me, upgrades);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverSpawned if the passed character is a FirstPersonMover
        /// </summary>
        /// <param name="me"></param>
        public override void OnCharacterSpawned(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverSpawned(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterSpawned(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverUpdate if the passed character is a firstpersonmover
        /// </summary>
        /// <param name="me"></param>
        public override void OnCharacterUpdate(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverUpdate(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterUpdate(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades"></param>
        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].AfterUpgradesRefreshed(owner, upgrades);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="projectile"></param>
        public override void OnProjectileCreated(Projectile projectile)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnProjectileCreated(projectile);

                Projectile projectileComponent = projectile.GetComponent<Projectile>();

                if (projectileComponent is ArrowProjectile)
                {
                    mods[i].OnArrowProjectileCreated(projectileComponent as ArrowProjectile);
                }
                if (projectileComponent is BulletProjectile)
                {
                    BulletProjectile bullet = projectileComponent as BulletProjectile;

                    bool isMortarShrapnel = bullet.GetDamageSourceType() == DamageSourceType.SpidertronGrenade;
                    bool isFlameBreath = bullet.GetDamageSourceType() == DamageSourceType.FlameBreath || bullet.GetDamageSourceType() == DamageSourceType.SpawnCampDeflectedFlameBreath;
                    bool isRepairFire = bullet.GetDamageSourceType() == DamageSourceType.RepairFire;

                    mods[i].OnBulletProjectileCreated(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
                }
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="projectile"></param>
        public override void OnProjectileStartedMoving(Projectile projectile)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnProjectileStartedMoving(projectile);

                Projectile projectileComponent = projectile.GetComponent<Projectile>();

                if (projectileComponent is ArrowProjectile)
                {
                    mods[i].OnArrowProjectileStartedMoving(projectileComponent as ArrowProjectile);
                }
                if (projectileComponent is BulletProjectile)
                {
                    BulletProjectile bullet = projectileComponent as BulletProjectile;

                    bool isMortarShrapnel = bullet.GetDamageSourceType() == DamageSourceType.SpidertronGrenade;
                    bool isFlameBreath = bullet.GetDamageSourceType() == DamageSourceType.FlameBreath || bullet.GetDamageSourceType() == DamageSourceType.SpawnCampDeflectedFlameBreath;
                    bool isRepairFire = bullet.GetDamageSourceType() == DamageSourceType.RepairFire;

                    mods[i].OnBulletProjectileStartedMoving(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
                }
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="projectile"></param>
        public override void OnProjectileUpdate(Projectile projectile)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnProjectileUpdate(projectile);

                Projectile projectileComponent = projectile.GetComponent<Projectile>();

                if (projectileComponent is ArrowProjectile)
                {
                    mods[i].OnArrowProjectileUpdate(projectileComponent as ArrowProjectile);
                }
                if (projectileComponent is BulletProjectile)
                {
                    BulletProjectile bullet = projectileComponent as BulletProjectile;

                    bool isMortarShrapnel = bullet.GetDamageSourceType() == DamageSourceType.SpidertronGrenade;
                    bool isFlameBreath = bullet.GetDamageSourceType() == DamageSourceType.FlameBreath || bullet.GetDamageSourceType() == DamageSourceType.SpawnCampDeflectedFlameBreath;
                    bool isRepairFire = bullet.GetDamageSourceType() == DamageSourceType.RepairFire;

                    mods[i].OnBulletProjectileUpdate(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
                }
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="projectile"></param>
        public override void OnProjectileDestroyed(Projectile projectile)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnProjectileDestroyed(projectile);

                Projectile projectileComponent = projectile.GetComponent<Projectile>();

                if (projectileComponent is ArrowProjectile)
                {
                    mods[i].OnArrowProjectileDestroyed(projectileComponent as ArrowProjectile);
                }
                if (projectileComponent is BulletProjectile)
                {
                    BulletProjectile bullet = projectileComponent as BulletProjectile;

                    bool isMortarShrapnel = bullet.GetDamageSourceType() == DamageSourceType.SpidertronGrenade;
                    bool isFlameBreath = bullet.GetDamageSourceType() == DamageSourceType.FlameBreath || bullet.GetDamageSourceType() == DamageSourceType.SpawnCampDeflectedFlameBreath;
                    bool isRepairFire = bullet.GetDamageSourceType() == DamageSourceType.RepairFire;

                    mods[i].OnBulletProjectileDestroyed(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
                }
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="arrow"></param>
        public override void OnArrowProjectileCreated(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileCreated(arrow);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="arrow"></param>
        public override void OnArrowProjectileStartedMoving(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileStartedMoving(arrow);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="arrow"></param>
        public override void OnArrowProjectileUpdate(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileUpdate(arrow);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="arrow"></param>
        public override void OnArrowProjectileDestroyed(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileDestroyed(arrow);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        /// <param name="isRepairFire"></param>
        public override void OnBulletProjectileCreated(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileCreated(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        /// <param name="isRepairFire"></param>
        public override void OnBulletProjectileStartedMoving(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileStartedMoving(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        /// <param name="isRepairFire"></param>
        public override void OnBulletProjectileUpdate(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileUpdate(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="isMortarShrapnel"></param>
        /// <param name="isFlameBreath"></param>
        /// <param name="isRepairFire"></param>
        public override void OnBulletProjectileDestroyed(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileDestroyed(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="killedCharacter"></param>
        /// <param name="killerCharacter"></param>
        /// <param name="damageSourceType"></param>
        public override void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType);
            }
        }
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnModDeactivated()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModDeactivated();
            }
        }

        /// <summary>
        /// Gets the response from this from all loaded mods, and uses the or operator on all of them, then returns
        /// </summary>
        /// <returns></returns>
        public override bool ShouldCursorBeEnabled() // if any mod tells the game that the cursor should be enabled, it will be
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            bool result = false;
            foreach(Mod mod in mods)
            {
                if (mod.ShouldCursorBeEnabled())
                {
                    result = true;
                }
            }

            return result || Generic2ButtonDialogue.IsWindowOpen;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void GlobalUpdate()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].GlobalUpdate();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="moddedEvent"></param>
        public override void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnMultiplayerEventReceived(moddedEvent);
            }
        }

    }
}