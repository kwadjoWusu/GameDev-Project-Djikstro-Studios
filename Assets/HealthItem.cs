using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public int healAmount = 1; // Amount of health to restore

    public static event Action<int> OnHealthCollect;
    public void Collect()
    {
        OnHealthCollect.Invoke(healAmount);
        Destroy(gameObject); // Destroy the item when collected
    }
}
