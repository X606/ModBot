using System;

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