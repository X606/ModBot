using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to catch events
    /// </summary>
    public class ModdedMultiplayerEventListener : GlobalEventListener
    {
        /// <summary>
        /// Called when we recieve a <see cref="ActivateAutomatedLaserBlastEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ActivateAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="AllArmorDestroyedFromEMPEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(AllArmorDestroyedFromEMPEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ArenaLiftStartedEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ArenaLiftStartedEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ArmorAppliedEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ArmorAppliedEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="BattleRoyaleAdminCommandEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(BattleRoyaleAdminCommandEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="BlockSwordEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(BlockSwordEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="BodyPartDamageClientEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(BodyPartDamageClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="BodyPartDamageEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(BodyPartDamageEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="BombDropperStartEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(BombDropperStartEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="CancelArrowDrawEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(CancelArrowDrawEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientCustomize1v1GameEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientCustomize1v1GameEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientCustomizeBattleRoyaleEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientCustomizeBattleRoyaleEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientCustomizeCoopGameEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientCustomizeCoopGameEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientGarbageInfoEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientGarbageInfoEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientLoadedLevelEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientLoadedLevelEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientRequestsFinalZoneActivation"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientRequestsFinalZoneActivation evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientRequestsStartingLevelNowEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientRequestsStartingLevelNowEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientRequestStreamingLevelDownload"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientRequestStreamingLevelDownload evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientRequestsUpgradeBotRelease"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientRequestsUpgradeBotRelease evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientSelectedUpgradeEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientSelectedUpgradeEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientThinksEnemyDiedEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientThinksEnemyDiedEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ClientWantsToPlayAgainEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ClientWantsToPlayAgainEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="CreateAndDrawArrowEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(CreateAndDrawArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="CreateAutomatedLaserBlastEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(CreateAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="CreateCoopSpawnVFXEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(CreateCoopSpawnVFXEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DebugServerLogEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DebugServerLogEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DeflectedArrowEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DeflectedArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DestroyArmorPieceClientEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DestroyArmorPieceClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DestroyArmorPieceEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DestroyArmorPieceEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DestroyArrowEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DestroyArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="DismissAutomatedLaserBlastEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(DismissAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="FireArrowEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(FireArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="FireSpreadEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(FireSpreadEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="FireTrapStartTimeEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(FireTrapStartTimeEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ForcefieldRepulsionEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ForcefieldRepulsionEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="GenericStringForModdingEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(GenericStringForModdingEvent evnt)
        {
            ModsManager.Instance.PassOnMod.OnMultiplayerEventReceived(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="HammerHitShieldEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(HammerHitShieldEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="HammerImpactClientEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(HammerImpactClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="HammerImpactEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(HammerImpactEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="JumpPadEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(JumpPadEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="MultiplayerKillEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(MultiplayerKillEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="PlayCommentatorSpeechEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(PlayCommentatorSpeechEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="SendLevelDataEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(SendLevelDataEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ServerReceivedLevelFromClient"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ServerReceivedLevelFromClient evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ServerRequestsGarbageInfoEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ServerRequestsGarbageInfoEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="ServerToClientsAdminCommand"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(ServerToClientsAdminCommand evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="SwordVisualizationEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(SwordVisualizationEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="TriggerActivatedEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(TriggerActivatedEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }

        /// <summary>
        /// Called when we recieve a <see cref="UpgradeSelectionCompleteEvent"/>
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(UpgradeSelectionCompleteEvent evnt)
        {
            MultiplayerEventCallback.OnEventRecieved(evnt);
        }
    }
}
