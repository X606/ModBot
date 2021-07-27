using ModLibrary;
using MoonSharp.Interpreter;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// Contains debug methods for scriptable objects
	/// </summary>
	[MoonSharpUserData]
	internal class DebugRef
	{
		/// <summary>
		/// Writes to the console
		/// </summary>
		/// <param name="message"></param>
		public void log(string message)
		{
			debug.Log(message);
		}

	}
}
