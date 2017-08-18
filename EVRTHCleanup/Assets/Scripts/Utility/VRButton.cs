using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Lets you interact with uGUI systems without using the mouse by passing events to the underlying uGUI interfaces
/// Can also have its own OnClick events, letting a vr user have additional events trigger when using the UI element vs a non-vr user
/// </summary>
public class VRButton : MonoBehaviour,IPointerHandlerVR
{
    /// <summary>
    /// Event that will only be triggered by this script and not normal ui interaction
    /// </summary>
    public UnityEvent OnClick;

    #region uGUI event passthrough

    private IPointerEnterHandler pointerEnter;
    private IPointerExitHandler pointerExit;
    private IPointerClickHandler pointerClick;
    private IPointerDownHandler pointerDown;
    private IPointerUpHandler pointerUp;
    #endregion

    private void Awake()
    {
        pointerEnter = GetComponent<IPointerEnterHandler>();
        pointerExit = GetComponent<IPointerExitHandler>();
        pointerClick = GetComponent<IPointerClickHandler>();
        pointerDown = GetComponent<IPointerDownHandler>();
        pointerUp = GetComponent<IPointerUpHandler>();

    }

    public void Click(Vector3 PointerPosition)
    {
        if (OnClick != null)
        {
            OnClick.Invoke();
        }

        if (pointerClick != null)
        {
            pointerClick.OnPointerClick(new PointerEventData(EventSystem.current){position = Camera.main.WorldToScreenPoint(PointerPosition)});
        }

    }

    public void PointerDown(Vector3 PointerPosition)
    {
        //if (pointerDown != null)
        //{
        //    pointerDown.OnPointerDown(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        //}
        Click(PointerPosition);
    }

    public void PointerUp(Vector3 PointerPosition)
    {
        //if (pointerUp != null)
        //{
        //    pointerUp.OnPointerUp(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        //}
    }

    public void PointerEnter(Vector3 PointerPosition)
    {
        if (pointerEnter != null)
        {
            pointerEnter.OnPointerEnter(new PointerEventData(EventSystem.current){position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }
    }

    public void PointerExit(Vector3 PointerPosition)
    {
        if (pointerExit != null)
        {
            pointerExit.OnPointerExit(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }
    }
}
