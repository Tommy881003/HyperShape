using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    [TextArea(1,3)]
    public string toShow;
    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = toShow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = "";
    }
}
