using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Class used to keep both a mod and bool that decides if the mod is active in same list
    /// </summary>
    public class LoadedMod
    {
        private LoadedMod() // this will prevent people from createing now LoadedMod instances in mods
        {
        }

        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="_mod"></param>
        /// <param name="_rawAssemblyData"></param>
        /// <param name="isLoadedFromFile"></param>
        internal LoadedMod(Mod _mod, byte[] _rawAssemblyData, bool isLoadedFromFile)
        {
            Mod = _mod;
            IsDeactivated = false;
            RawAssemblyData = _rawAssemblyData;
            IsOnlyLoadedInMemory = !isLoadedFromFile;
        }

        /// <summary>
        /// The Mod object the class is holding
        /// </summary>
        public Mod Mod;

        /// <summary>
        /// Decides if the mod is deactivated.
        /// </summary>
        public bool IsDeactivated;

        /// <summary>
        /// If this mod doesnt have a file
        /// </summary>
        public bool IsOnlyLoadedInMemory;

        internal readonly byte[] RawAssemblyData;
    }
}