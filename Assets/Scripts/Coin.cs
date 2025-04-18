using System;
using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    public static event Action<int> OnCoinCollect;
    public int worth = 1;
     private bool isCollected = false;
    public void Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            OnCoinCollect?.Invoke(worth);
            Destroy(gameObject);
        }
        
    }

    
}
