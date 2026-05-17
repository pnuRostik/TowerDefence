using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private void Awake()
    {
        EnsurePanels();
        HideAll();
    }

    private void OnEnable() => GameManager.OnStateChanged += HandleStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= HandleStateChanged;

    private void Start()
    {
        if (GameManager.Instance != null)
            HandleStateChanged(GameManager.Instance.CurrentState);
    }

    private void HandleStateChanged(GameState state)
    {
        HideAll();

        switch (state)
        {
            case GameState.Victory:
                if (winPanel != null) winPanel.SetActive(true);
                break;
            case GameState.Loss:
                if (losePanel != null) losePanel.SetActive(true);
                break;
        }
    }

    private void HideAll()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void EnsurePanels()
    {
        if (winPanel != null && losePanel != null) return;

        var font = FindAnyObjectByType<TextMeshProUGUI>()?.font;

        if (winPanel == null)
        {
            winPanel = CreateEndPanel(
                "WinPanel",
                new Color(0.08f, 0.22f, 0.12f, 0.92f),
                new Color(0.15f, 0.55f, 0.28f, 1f),
                "VICTORY!",
                "All waves cleared",
                font);
        }

        if (losePanel == null)
        {
            losePanel = CreateEndPanel(
                "LosePanel",
                new Color(0.24f, 0.08f, 0.08f, 0.92f),
                new Color(0.7f, 0.18f, 0.18f, 1f),
                "DEFEAT",
                "Base destroyed",
                font);
        }
    }

    private GameObject CreateEndPanel(
        string panelName,
        Color overlayColor,
        Color cardColor,
        string title,
        string subtitle,
        TMP_FontAsset font)
    {
        var panel = CreateStretchObject(panelName, transform);
        var overlay = panel.AddComponent<Image>();
        overlay.color = overlayColor;
        overlay.raycastTarget = true;

        var card = CreateStretchObject("Card", panel.transform);
        var cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(420f, 280f);
        cardRect.anchoredPosition = Vector2.zero;

        var cardImage = card.AddComponent<Image>();
        cardImage.color = cardColor;

        var titleText = CreateLabel("Title", card.transform, font, 42, title, new Vector2(0f, 70f));
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;

        var subtitleText = CreateLabel("Subtitle", card.transform, font, 22, subtitle, new Vector2(0f, 20f));
        subtitleText.color = new Color(1f, 1f, 1f, 0.85f);
        subtitleText.alignment = TextAlignmentOptions.Center;

        CreateRestartButton(card.transform, font);

        panel.SetActive(false);
        panel.transform.SetAsLastSibling();
        return panel;
    }

    private static GameObject CreateStretchObject(string objectName, Transform parent)
    {
        var go = new GameObject(objectName, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return go;
    }

    private static TextMeshProUGUI CreateLabel(
        string objectName,
        Transform parent,
        TMP_FontAsset font,
        float fontSize,
        string text,
        Vector2 anchoredPosition)
    {
        var go = new GameObject(objectName, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(360f, 80f);
        rect.anchoredPosition = anchoredPosition;

        var label = go.AddComponent<TextMeshProUGUI>();
        if (font != null) label.font = font;
        label.fontSize = fontSize;
        label.text = text;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;

        return label;
    }

    private void CreateRestartButton(Transform parent, TMP_FontAsset font)
    {
        var buttonGo = new GameObject("RestartButton", typeof(RectTransform));
        buttonGo.transform.SetParent(parent, false);

        var rect = buttonGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(260f, 56f);
        rect.anchoredPosition = new Vector2(0f, -80f);

        var image = buttonGo.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        var button = buttonGo.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(OnRestartClicked);

        var label = CreateLabel("Text", buttonGo.transform, font, 24, "Try Again", Vector2.zero);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = Vector2.zero;
        label.rectTransform.offsetMax = Vector2.zero;
        label.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
