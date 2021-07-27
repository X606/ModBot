using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModsData", menuName = "Hidden/ModsData", order = 0)]
public class ModsData : ScriptableObject
{
    public List<ModData> Mods = new List<ModData>();
}
