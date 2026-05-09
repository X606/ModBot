using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace InternalModBot.LevelEditor
{
    internal class CustomObjectsList
    {
        public const string CUSTOM_OBJECTS_LIST_KEY = "LevelCustomObjects";

        private static JsonSerializerSettings s_serializerSettings;

        public List<LevelEditorLevelObject> Objects;

        public string Serialize() => JsonConvert.SerializeObject(this, getSettings());

        public static CustomObjectsList Deserialize(string @string)
        {
            using (StringReader sr = new StringReader(@string))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return JsonSerializer.Create(getSettings()).Deserialize<CustomObjectsList>(reader);
            }
        }

        private static JsonSerializerSettings getSettings()
        {
            if (s_serializerSettings != null) return s_serializerSettings;

            s_serializerSettings = new JsonSerializerSettings(DataRepository.Instance.GetSettings())
            {
                SerializationBinder = new TypeMigrationSerializationBinder()
            };
            return s_serializerSettings;
        }
    }
}