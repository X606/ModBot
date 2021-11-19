using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt;
using ModLibrary;
using System.Collections;

namespace InternalModBot
{
    internal class ModdedMultiplayerEventListener : GlobalEventListener
    {
        public override void OnEvent(ActivateAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(AllArmorDestroyedFromEMPEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ArenaLiftStartedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ArmorAppliedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(BattleRoyaleAdminCommandEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(BlockSwordEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(BodyPartDamageClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(BodyPartDamageEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(BombDropperStartEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(CancelArrowDrawEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientCustomize1v1GameEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientCustomizeBattleRoyaleEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientCustomizeCoopGameEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientGarbageInfoEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientLoadedLevelEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientRequestsFinalZoneActivation evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientRequestsStartingLevelNowEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientRequestStreamingLevelDownload evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientRequestsUpgradeBotRelease evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientSelectedUpgradeEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientThinksEnemyDiedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientWantsToPlayAgainEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(CreateAndDrawArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(CreateAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(CreateCoopSpawnVFXEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DebugServerLogEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DeflectedArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DestroyArmorPieceClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DestroyArmorPieceEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DestroyArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }
        
        public override void OnEvent(DismissAutomatedLaserBlastEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(FireArrowEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(FireSpreadEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(FireTrapStartTimeEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ForcefieldRepulsionEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(GenericStringForModdingEvent evnt)
        {
            ModSharingManager.Instance.OnModdedEvent(evnt);
            ModBotUserIdentifier.Instance.OnEvent(evnt);
            ModsManager.Instance.PassOnMod.OnMultiplayerEventReceived(evnt);
        }

        public override void OnEvent(HammerHitShieldEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(HammerImpactClientEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(HammerImpactEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(JumpPadEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(MultiplayerKillEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(PlayCommentatorSpeechEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(SendLevelDataEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ServerReceivedLevelFromClient evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ServerRequestsGarbageInfoEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ServerToClientsAdminCommand evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(SwordVisualizationEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(TriggerActivatedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(UpgradeSelectionCompleteEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

		public override void Connected(BoltConnection connection)
		{
			ModBotUserIdentifier.Instance.OnLocalClientConnected();
			ModsManager.Instance.PassOnMod.OnClientConnectedToServer();
		}

		public override void Disconnected(BoltConnection connection)
		{
			ModsManager.Instance.PassOnMod.OnClientDisconnectedFromServer();
		}

        public override void OnEvent(ArenaLiftArrivedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientCustomizeCharacterEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientEmoteEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(ClientPicksChallengeEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(DestructibleBlockDestroyedEvent evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }

        public override void OnEvent(MatchInstance evnt)
        {
            MultiplayerEventCallback.OnEventReceived(evnt);
        }
    }
}
