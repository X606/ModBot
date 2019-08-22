using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    public class PassOnToModsManager : Mod
    {
        public override string GetModName()
        {
            return string.Empty;
        }
        public override string GetUniqueID()
        {
            return string.Empty;
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverSpawned(me);
            }
        }

        public override void OnFirstPersonMoverUpdate(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverUpdate(me);
            }
        }

        public override void OnSceneChanged(GameMode gamemode)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnSceneChanged(gamemode);
            }
        }

        public override void OnModRefreshed()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModRefreshed();
            }
        }

        public override void OnLevelEditorStarted()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLevelEditorStarted();
            }
        }

        public override void OnObjectPlacedInLevelEditor(ObjectPlacedInLevel _obj)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnObjectPlacedInLevelEditor(_obj);
            }
        }

        public override void OnCommandRan(string command)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCommandRan(command);
            }
        }

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

        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].AfterUpgradesRefreshed(owner, upgrades);
            }
        }

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

        public override void OnArrowProjectileCreated(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileCreated(arrow);
            }
        }

        public override void OnArrowProjectileStartedMoving(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileStartedMoving(arrow);
            }
        }

        public override void OnArrowProjectileUpdate(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileUpdate(arrow);
            }
        }

        public override void OnArrowProjectileDestroyed(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileDestroyed(arrow);
            }
        }

        public override void OnBulletProjectileCreated(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileCreated(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnBulletProjectileStartedMoving(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileStartedMoving(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnBulletProjectileUpdate(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileUpdate(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnBulletProjectileDestroyed(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileDestroyed(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType);
            }
        }

        public override void OnModDeactivated()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModDeactivated();
            }
        }

    }
}