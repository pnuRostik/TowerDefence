using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconDisplay;
    public TextMeshProUGUI priceDisplay;
    public Button buyButton;

    private TowerData data;
    private TowerBuilder builder;

 
    public void Initialize(TowerData towerData, TowerBuilder towerBuilder)
    {
        data = towerData;
        builder = towerBuilder;

        iconDisplay.sprite = data.icon;
        priceDisplay.text = data.cost.ToString();

       
        buyButton.onClick.AddListener(() => {
            builder.ConfirmBuild(data);
        });
    }

    private void Update()
    {
        if (data == null) 
        return;

        Debug.Log(buyButton);
        buyButton.interactable = EconomyManager.Instance.CanAfford(data.cost);
    }
}