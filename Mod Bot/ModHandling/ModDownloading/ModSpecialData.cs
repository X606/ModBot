using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace InternalModBot
{
    internal class ModSpecialData
    {
        public Dictionary<string, JToken> Data;

        public bool Verified
        {
            get => Data["Verified"].ToObject<bool>();
        }

        public int Downloads
        {
            get => Data["Downloads"].ToObject<int>();
        }

        public int Likes
        {
            get => Data["Likes"].ToObject<int>();
        }
    }
}