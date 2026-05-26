using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectorMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI texto;
    private string textoOriginal;

    void Start()
    {
        texto = GetComponentInChildren<TextMeshProUGUI>();
        textoOriginal = texto.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.text = ">> " + textoOriginal;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.text = textoOriginal;
    }
}