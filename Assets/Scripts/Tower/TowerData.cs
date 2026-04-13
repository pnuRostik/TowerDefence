using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    
public string towerName;
    public int cost;
    public float attackSpeed;
    public float range;
    public float damage;
    public AttackType attackType;
    
    public GameObject prefab; 
    public Sprite icon;       
}

public enum AttackType
{
    SingleTarget, 
    AoE,         
    Slow
}