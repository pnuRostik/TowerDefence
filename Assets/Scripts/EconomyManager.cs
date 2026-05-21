using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance; 
    public int gold = 600;

    public event Action<int> OnGoldChanged;

    private void Awake() => Instance = this;

    public bool CanAfford(int amount) => gold >= amount;

    public void SpendGold(int amount)
    {
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }
}