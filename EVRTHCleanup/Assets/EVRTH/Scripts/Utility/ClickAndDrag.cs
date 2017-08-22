using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    /// <summary>
    /// Lets you grab and move the ui window to interact with it in VR
    /// </summary>
    public class ClickAndDrag : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler,IPointerHandlerVR
    {
        public Transform FollowTarget;

        public void OnPointerDown(PointerEventData eventData)
        {
            print("Pointer Down");
            if (FollowTarget)
            {
                transform.parent  = FollowTarget;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            print("Pointer Up");
            transform.parent = null;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            print("Pointer Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            print("pointer exit");
        }

        public void OnTriggerEnter(Collider otherCollider)
        {
            FollowTarget = otherCollider.transform;
        }

        public void OnTriggerExit(Collider otherCollider)
        {
            FollowTarget = null;
        }

        public void PointerDown(Vector3 PointerPosition)
        {
            OnPointerDown(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }

        public void PointerUp(Vector3 PointerPosition)
        {
            OnPointerUp(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }

        public void PointerEnter(Vector3 PointerPosition)
        {
            OnPointerEnter(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }

        public void PointerExit(Vector3 PointerPosition)
        {
            OnPointerExit(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            print("Clicked");
        }
    }
}
