using MoonSharp.Interpreter;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// A wrapper for the <see cref="GameObject"/> for scriptable objects
	/// </summary>
	[MoonSharpUserData]
	internal class GameObjectRef
	{
		GameObject _gameObject;

		/// <summary>
		/// Initializes a new wrapper for a target gameobject
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		static GameObjectRef create(GameObject gameObject)
		{
			if (gameObject == null)
				return null;

			return new GameObjectRef()
			{
				_gameObject = gameObject
			};
		}
		
		/// <summary>
		/// The position of the gameobject
		/// </summary>
		public Vector3Ref position
		{
			get => _gameObject.transform.localPosition;
			set => _gameObject.transform.localPosition = value;
		}
		/// <summary>
		/// The rotation, in eulereangles, of the object
		/// </summary>
		public Vector3Ref eulerAngles
		{
			get => _gameObject.transform.localEulerAngles;
			set => _gameObject.transform.localEulerAngles = value;
		}
		/// <summary>
		/// The scale of the object
		/// </summary>
		public Vector3Ref scale
		{
			get => _gameObject.transform.localScale;
			set => _gameObject.transform.localScale = value;
		}

		/// <summary>
		/// The parent of the object
		/// </summary>
		public GameObjectRef parent
		{
			get => _gameObject.transform.parent.gameObject;
		}

		/// <summary>
		/// The forward vector of the object
		/// </summary>
		/// <returns></returns>
		public Vector3Ref getForward() => _gameObject.transform.forward;

		/// <summary>
		/// Destroys the object
		/// </summary>
		public void destroy()
		{
			GameObject.Destroy(_gameObject);
		}

		/// <summary>
		/// Makes this wrapper be able to be implicitily converted to a gameobject
		/// </summary>
		/// <param name="gameObject"></param>
		public static implicit operator GameObject(GameObjectRef gameObject) => gameObject._gameObject;
		/// <summary>
		/// Makes gameobjects be implicitly converted to this wrapper
		/// </summary>
		/// <param name="gameObject"></param>
		public static implicit operator GameObjectRef(GameObject gameObject) => GameObjectRef.create(gameObject);
	}

}
