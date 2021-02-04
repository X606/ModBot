using ModLibrary;
using MoonSharp.Interpreter;

namespace InternalModBot.Scripting
{
	[MoonSharpUserData]
	public class DebugRef
	{
		public void log(string message)
		{
			debug.Log(message);
		}

	}
}
