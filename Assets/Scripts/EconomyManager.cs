using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance; 
    public int gold = 1050; 

    private void Awake() => Instance = this;

    public bool CanAfford(int amount) => gold >= amount;

    public void SpendGold(int amount)
    {
        gold -= amount;
    }
}