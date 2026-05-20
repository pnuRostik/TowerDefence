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
        MusicManager.Instance.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MusicManager.Instance.PlayHover();
    }
    void OnClick()
    {
        MusicManager.Instance.PlayClick();
    }
}