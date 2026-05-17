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

    [SerializeField] private List<Vector3Int> initialOccupiedCells = new List<Vector3Int>();
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
    private Vector3Int selectedCell;
    private EnemySpawner spawner;

    private void Start()
    {
        spawner = Object.FindAnyObjectByType<EnemySpawner>();
        GenerateMenu();
        menuPanel.SetActive(false);

        foreach (var cell in initialOccupiedCells)
        {
            occupiedCells.Add(cell);
        }
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

        // Close menu if a wave starts
        if (spawner != null && !spawner.CanStartNextWave && menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // Prevent opening menu during wave
            if (spawner != null && !spawner.CanStartNextWave) return;

            HandleTileSelection();
        }
    }

    private void HandleTileSelection()
{
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        Vector3Int cellPos = towerTilemap.WorldToCell(worldPos);
        Debug.Log($"Clicked cell: {cellPos} at world position: {worldPos}");

        if (towerTilemap.HasTile(cellPos) && !occupiedCells.Contains(cellPos))
        {
            selectedCell = cellPos;
            menuPanel.SetActive(true);
            
            RectTransform rect = menuPanel.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0f); 

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            
            PositionMenu(mousePos);
        }
        else
        {
            menuPanel.SetActive(false);
        }
    }

    private void PositionMenu(Vector2 mousePos)
    {
        RectTransform rect = menuPanel.GetComponent<RectTransform>();
        
        rect.position = mousePos;

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Vector3 pos = rect.position;

        if (corners[0].x < 0) pos.x -= corners[0].x;
        if (corners[2].x > Screen.width) pos.x -= (corners[2].x - Screen.width);

        if (corners[0].y < 0) pos.y -= corners[0].y;
        if (corners[1].y > Screen.height) pos.y -= (corners[1].y - Screen.height);

        rect.position = pos;
    }

    public void ConfirmBuild(TowerData data)
    {
        if (spawner != null && !spawner.CanStartNextWave) return;

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