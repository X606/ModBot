namespace InternalModBot
{
    /// <summary>
    /// Contains methods that get called from the game itself
    /// </summary>
    internal static class CalledFromInjections
    {
        // no longer needed, left for now for compatibility with old launcher

        /// <summary>
        /// Called from <see cref="UnityEngine.Resources.Load(string)"/>, <see cref="UnityEngine.Resources.Load{T}(string)"/> and <see cref="UnityEngine.ResourceRequest.asset"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object FromResourcesLoad(string path) => null;
    }
}