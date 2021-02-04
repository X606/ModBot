using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Scripting
{
	[MoonSharpUserData]
	public class InputRef
	{
		public bool getKeyDown(int key) => Input.GetKeyDown((KeyCode)key);
		public bool getKey(int key) => Input.GetKey((KeyCode)key);
		public bool getKeyUp(int key) => Input.GetKeyUp((KeyCode)key);
	}

}
