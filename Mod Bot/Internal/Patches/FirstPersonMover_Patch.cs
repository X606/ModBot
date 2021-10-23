using Bolt;
using HarmonyLib;
using ModLibrary;

namespace InternalModBot
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    static class FirstPersonMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshUpgrades")]
        static void RefreshUpgrades_Prefix(FirstPersonMover __instance)
        {
            if (__instance == null || __instance.gameObject == null || !__instance.IsAlive() || __instance.GetCharacterModel() == null)
                return;

            UpgradeCollection upgrade = __instance.GetComponent<UpgradeCollection>();
            ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(__instance, upgrade);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateCharacterModel")]
        static void CreateCharacterModel_Postfix(FirstPersonMover __instance)
        {
            ModsManager.Instance.PassOnMod.OnCharacterModelCreated(__instance);
        }

        // TODO: Remove this injection when removing the CharacterInputRestrictor system
        [HarmonyPrefix]
        [HarmonyPatch("ExecuteCommand")]
        static void ExecuteCommand_Prefix(FirstPersonMover __instance, ref Command command)
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
    }
}