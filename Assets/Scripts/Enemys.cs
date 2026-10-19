using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawnnendeobjects", menuName = "Enemys")]
public class spawnableobjects : ScriptableObject
{
   public List<GameObject> enemys;
   public List<GameObject> chests;
   public List<GameObject> npcs;
}
