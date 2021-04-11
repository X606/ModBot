namespace ModLibrary
{
    /// <summary>
    /// Represents the type of a mod
    /// </summary>
    public enum ModType
    {
        /// <summary>
        /// A mod written in C#, must have a main .dll file
        /// </summary>
        CSharp,
        /// <summary>
        /// A mod written in LUA, must have a main.lua file
        /// </summary>
        LUA
    }
}