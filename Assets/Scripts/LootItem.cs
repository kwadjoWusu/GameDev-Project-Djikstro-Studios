using UnityEngine;

[System.Serializable]
public class LootItem 
{
    public GameObject itemPrefab; // Prefab of the item to spawn
    [Range(0, 100)] public float dropChance;
}
