using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// defines functions used in scriptable objects to detect key inputs
	/// </summary>
	//[MoonSharpUserData]
	internal class InputRef
	{
		/// <summary>
		/// Gets if a key was pressed this frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool getKeyDown(int key) => Input.GetKeyDown((KeyCode)key);
		/// <summary>
		/// Gets if a key is pressed
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool getKey(int key) => Input.GetKey((KeyCode)key);
		/// <summary>
		/// Gets if a key was released this frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool getKeyUp(int key) => Input.GetKeyUp((KeyCode)key);
	}

}
