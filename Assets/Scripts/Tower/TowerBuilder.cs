using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerBuilder : MonoBehaviour
{
    public Tilemap towerTilemap;
    public GameObject menuPanel;   
    public GameObject slotPrefab;   
    public List<TowerData> towers;  

    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
    private Vector3Int selectedCell;

    private void Start()
    {
        GenerateMenu();
        menuPanel.SetActive(false);
    }

   
    private void GenerateMenu()
    {
        foreach (TowerData t in towers)
        {
            GameObject slot = Instantiate(slotPrefab, menuPanel.transform);
            slot.GetComponent<TowerSlot>().Initialize(t, this);
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            
            if (EventSystem.current.IsPointerOverGameObject()) return;

            HandleTileSelection();
        }
    }

    private void HandleTileSelection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        Vector3Int cellPos = towerTilemap.WorldToCell(worldPos);

        if (towerTilemap.HasTile(cellPos) && !occupiedCells.Contains(cellPos))
        {
            selectedCell = cellPos;
            menuPanel.SetActive(true);
            menuPanel.transform.position = mousePos; 
        }
        else
        {
            menuPanel.SetActive(false);
        }
    }

    public void ConfirmBuild(TowerData data)
    {
        if (EconomyManager.Instance.CanAfford(data.cost))
        {
            EconomyManager.Instance.SpendGold(data.cost);
            
            Vector3 spawnPos = towerTilemap.GetCellCenterWorld(selectedCell);
            spawnPos.y += 0.5f;
            Instantiate(data.prefab, spawnPos, Quaternion.identity);

            
            occupiedCells.Add(selectedCell);
            menuPanel.SetActive(false); 
        }
    }
}