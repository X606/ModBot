using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    public class PassOnToModsManager : Mod
    {
        public override string GetModName()
        {
            return "";
        }

        public override void OnFirstPersonMoverSpawned(GameObject me)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverSpawned(me);
            }
        }

        public override void OnFirstPersonMoverUpdate(GameObject me)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverUpdate(me);
            }
        }

        public override void OnSceneChanged(GameMode gamemode)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnSceneChanged(gamemode);
            }
        }

        public override void OnModRefreshed()
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModRefreshed();
            }
        }

        public override void OnLevelEditorStarted()
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLevelEditorStarted();
            }
        }

        public override void OnObjectPlacedInLevelEditor(GameObject _obj)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnObjectPlacedInLevelEditor(_obj);
            }
        }

        public override void OnCommandRan(string command)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCommandRan(command);
            }
        }

        public override void OnUpgradesRefreshed(GameObject me, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnUpgradesRefreshed(me, upgrades);
            }
        }

        public override void OnCharacterSpawned(GameObject me)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterSpawned(me);
            }
        }

        public override void OnCharacterUpdate(GameObject me)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterUpdate(me);
            }
        }

        public override void AfterUpgradesRefreshed(GameObject owner, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].AfterUpgradesRefreshed(owner, upgrades);
            }
        }

        public override void OnProjectileCreated(GameObject projectile)
        {
            List<Mod> mods = ModsManager.Instance.mods;
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

        public override void OnProjectileStartedMoving(GameObject projectile)
        {
            List<Mod> mods = ModsManager.Instance.mods;
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

        public override void OnProjectileUpdate(GameObject projectile)
        {
            List<Mod> mods = ModsManager.Instance.mods;
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

        public override void OnArrowProjectileCreated(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileCreated(arrow);
            }
        }

        public override void OnArrowProjectileStartedMoving(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileStartedMoving(arrow);
            }
        }

        public override void OnArrowProjectileUpdate(ArrowProjectile arrow)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnArrowProjectileUpdate(arrow);
            }
        }

        public override void OnBulletProjectileCreated(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileCreated(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnBulletProjectileStartedMoving(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileStartedMoving(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnBulletProjectileUpdate(BulletProjectile bullet, bool isMortarShrapnel, bool isFlameBreath, bool isRepairFire)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnBulletProjectileUpdate(bullet, isMortarShrapnel, isFlameBreath, isRepairFire);
            }
        }

        public override void OnCharacterKilled(GameObject killedCharacter, GameObject killerCharacter, DamageSourceType damageSourceType)
        {
            List<Mod> mods = ModsManager.Instance.mods;
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType);
            }
        }
    }
}