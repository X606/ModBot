using ModLibrary;
using Newtonsoft.Json;

namespace InternalModBot
{
    /// <summary>
    /// Used when deserilizing data from the site
    /// </summary>
    public struct ModsHolder
    {
        /// <summary>
        /// A list of all the mods downloaded.
        /// </summary>
        [JsonProperty(PropertyName = "ModInfos")]
        public ModInfo[] Mods;
    }
}
