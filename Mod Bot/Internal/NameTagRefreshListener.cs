using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Add this component to a name tag and call Init to have it listen for <see cref="MultiplayerPlayerNameManager.REFRESH_NAME_TAGS_EVENT"/> global event and refresh itself if that event gets called
    /// </summary>
    internal class NameTagRefreshListener : MonoBehaviour
    {
        Character _owner;
        EnemyNameTag _nameTag;

        /// <summary>
        /// Inits the component with the appropriate values
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="nameTag"></param>
        public void Init(Character owner, EnemyNameTag nameTag)
        {
            _owner = owner;
            _nameTag = nameTag;

            GlobalEventManager.Instance.AddEventListener(MultiplayerPlayerNameManager.REFRESH_NAME_TAGS_EVENT, refreshNameTag);
        }

        void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(MultiplayerPlayerNameManager.REFRESH_NAME_TAGS_EVENT, refreshNameTag);
        }

        void refreshNameTag()
        {
            string playfabID = _owner.GetPlayFabID();
            if (string.IsNullOrEmpty(playfabID)) return;

            MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(playfabID, delegate (string displayName)
            {
                _nameTag.NameText.text = displayName;
            });
        }
    }
}
