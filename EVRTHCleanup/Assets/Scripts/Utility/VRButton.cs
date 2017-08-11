using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnClick;

    public void Click(Vector3 PointerPosition)
    {
        if (OnClick != null)
        {
            OnClick.Invoke();
        }

        if (GetComponent<IPointerClickHandler>() != null)
        {
            GetComponent<IPointerClickHandler>().OnPointerClick(new PointerEventData(EventSystem.current){position = Camera.main.WorldToScreenPoint(PointerPosition)});
        }

    }

    public void PointerEnter(Vector3 PointerPosition)
    {
        IPointerEnterHandler pe = GetComponent<IPointerEnterHandler>();
        if (pe != null)
        {
            pe.OnPointerEnter(new PointerEventData(EventSystem.current){position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }
    }

    public void PointerExit(Vector3 PointerPosition)
    {
        IPointerExitHandler pe = GetComponent<IPointerExitHandler>();
        if (pe != null)
        {
            pe.OnPointerExit(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }
    }
}
