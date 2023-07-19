using ModLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
{
    internal class ModDeletionInfo
    {
        public static List<string> ModsAboutToBeRemoved = new List<string>();

        public static void SaveModDeletionInfo(in ModInfo info)
        {
            if(info != null)
            {
                if(!ModsAboutToBeRemoved.Contains(info.UniqueID))
                ModsAboutToBeRemoved.Add(info.UniqueID);
            }
            else
            {
                ModsAboutToBeRemoved.Clear();
            }

            File.WriteAllText(Application.persistentDataPath + "ModsToDelete.json", JsonConvert.SerializeObject(ModsAboutToBeRemoved));
        }

        public static void LoadIDsOfModsToDelete()
        {
            if (!File.Exists(Application.persistentDataPath + "ModsToDelete.json") || ModsAboutToBeRemoved == null)
            {
                ModsAboutToBeRemoved = new List<string>();
                return;
            }
            ModsAboutToBeRemoved = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Application.persistentDataPath + "ModsToDelete.json"));
        }

        public static void RemoveID(string id)
        {
            if (!HasID(id) || ModsAboutToBeRemoved.Count == 0)
            {
                return;
            }
            for(int i = ModsAboutToBeRemoved.Count - 1; i > -1; i--)
            {
                if (ModsAboutToBeRemoved[i] == id)
                {
                    ModsAboutToBeRemoved.RemoveAt(i);
                }
            }
        }

        public static bool HasID(string id)
        {
            foreach(string info in ModsAboutToBeRemoved)
            {
                if (info == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
