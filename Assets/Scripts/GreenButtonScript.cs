using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayHover();
    }
    void OnClick()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayClick();
    }
}