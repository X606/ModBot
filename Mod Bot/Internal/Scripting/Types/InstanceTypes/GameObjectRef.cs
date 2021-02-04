using MoonSharp.Interpreter;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot.Scripting
{
	[MoonSharpUserData]
	public class GameObjectRef
	{
		GameObject _gameObject;

		static GameObjectRef create(GameObject gameObject)
		{
			if (gameObject == null)
				return null;

			return new GameObjectRef()
			{
				_gameObject = gameObject
			};
		}
		
		public Vector3Ref position
		{
			get => _gameObject.transform.localPosition;
			set => _gameObject.transform.localPosition = value;
		}
		public Vector3Ref eulerAngles
		{
			get => _gameObject.transform.localEulerAngles;
			set => _gameObject.transform.localEulerAngles = value;
		}
		public Vector3Ref scale
		{
			get => _gameObject.transform.localScale;
			set => _gameObject.transform.localScale = value;
		}

		public GameObjectRef parent
		{
			get => _gameObject.transform.parent.gameObject;
		}

		public Vector3Ref getForward() => _gameObject.transform.forward;

		public void destroy()
		{
			GameObject.Destroy(_gameObject);
		}

		public static implicit operator GameObject(GameObjectRef gameObject) => gameObject._gameObject;
		public static implicit operator GameObjectRef(GameObject gameObject) => GameObjectRef.create(gameObject);
	}

}
