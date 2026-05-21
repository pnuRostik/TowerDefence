using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconDisplay;
    public TextMeshProUGUI towerNameDisplay;
    public TextMeshProUGUI priceDisplay;
    public TextMeshProUGUI statsDisplay;
    public Button buyButton;

    private TowerData data;
    private TowerBuilder builder;

 
    public void Initialize(TowerData towerData, TowerBuilder towerBuilder)
    {
        data = towerData;
        builder = towerBuilder;

        if (iconDisplay != null) iconDisplay.sprite = data.icon;
        if (towerNameDisplay != null) towerNameDisplay.text = data.towerName;
        if (priceDisplay != null) priceDisplay.text = data.cost.ToString();
        
        UpdateStatsDisplay();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => {
            Debug.Log($"Buy button clicked for {data.towerName}");
            builder.ConfirmBuild(data);
        });
    }

    private void UpdateStatsDisplay()
    {
        if (statsDisplay == null) return;
        
        statsDisplay.text = $"Dmg: {data.damage}\nSpd: {data.attackSpeed}\nRng: {data.range}\n{data.attackType}";
    }

    private void Update()
    {
        if (data == null || buyButton == null || EconomyManager.Instance == null)
            return;

        bool canAfford = EconomyManager.Instance.CanAfford(data.cost);
        buyButton.interactable = canAfford;
        
        if (priceDisplay != null)
        {
            priceDisplay.color = canAfford ? Color.white : Color.red;
        }

        if (buyButton.targetGraphic is UnityEngine.UI.Image btnImg)
        {
            btnImg.color = canAfford ? new Color(0.2f, 0.6f, 0.2f) : new Color(0.3f, 0.3f, 0.3f);
        }
    }
}