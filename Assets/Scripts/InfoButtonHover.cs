using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject howToPlayImage;

    void Start()
    {
        if (howToPlayImage != null)
            howToPlayImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (howToPlayImage != null)
            howToPlayImage.SetActive(true);
        
        PlaySound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (howToPlayImage != null)
            howToPlayImage.SetActive(false);
        
        PlaySound();
    }

    private void PlaySound()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayHover();
        }
    }
}
