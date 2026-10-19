using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnanebleObjects", menuName = "SpawnanebleObjects")]
public class SpawnanebleObjects : ScriptableObject
{
   public List<GameObject> enemys;
   public List<GameObject> chests;
   public List<GameObject> npcs;
}
