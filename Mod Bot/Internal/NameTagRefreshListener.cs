using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModLibrary;

namespace InternalModBot
{
	/// <summary>
	/// Add this component to a name tag and call Init to have it listen for <see cref="MultiplayerPlayerNameManager.RefreshNameTags"/> and refresh itself if that event gets called
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

			MultiplayerPlayerNameManager.Instance.RefreshNameTags += multiplayerNamePrefixManager_RefreshNameTags;
		}
		void OnDestroy()
		{
			MultiplayerPlayerNameManager.Instance.RefreshNameTags -= multiplayerNamePrefixManager_RefreshNameTags;
		}

		void multiplayerNamePrefixManager_RefreshNameTags()
		{
			string playfabID = _owner.GetPlayFabID();
			if (playfabID != null)
			{
				MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(playfabID, delegate (string displayName)
				{
					_nameTag.NameText.text = displayName;
				});
			}
		}
	}
}
