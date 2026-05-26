using HarmonyLib;

namespace InternalModBot
{
    [HarmonyPatch(typeof(EnemyNameTag))]
    static class EnemyNameTag_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyNameTag.Initialize))]
        static void Initialize_Postfix(EnemyNameTag __instance, Character character)
        {
            if (MultiplayerPlayerInfoManager.Instance != null && MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(character.state.PlayFabID) != null)
            {
                __instance.NameText.supportRichText = true;
                __instance.gameObject.AddComponent<NameTagRefreshListener>().Init(character, __instance);
            }
        }
    }
}