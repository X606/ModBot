using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace InternalModBot
{
    public class TypeMigrationSerializationBinder : DefaultSerializationBinder
    {
        private static readonly Dictionary<string, string> _oldToNewTypeFullNames = new Dictionary<string, string>()
        {
            { "LevelEditorPatch.CustomObjectsList", "ModLibrary.LevelEditor.CustomObjectsList" }, // support levels made with mods that use LevelEditorPatch.dll
            { "InternalModBot.LevelEditor.CustomObjectsList", "ModLibrary.LevelEditor.CustomObjectsList" }
        };

        public TypeMigrationSerializationBinder()
        {
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (_oldToNewTypeFullNames.TryGetValue(typeName, out string newType))
            {
                return Type.GetType(newType);
            }
            return base.BindToType(assemblyName, typeName);
        }
    }
}