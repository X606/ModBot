using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
	/// <summary>
	/// Put this on the mod class in your mod. the target class will act as the "Main" call of your mod
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class MainModClassAttribute : Attribute
	{

	}
}