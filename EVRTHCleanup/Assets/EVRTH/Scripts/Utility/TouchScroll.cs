using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utility
{
    /// <summary>
    /// lets you scroll a scrollbar on the dropdown with buttons instead of with the handle, since the handle did not work well with VR
    /// </summary>
    public class TouchScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerHandlerVR
    {
        [Range(-1f,1f)]
        public float scrollSpeed;

        public Transform dropDownTransform;
        public Scrollbar scrollbar;
        public Image targetImage;
        public Color defaultColor;
        public Color highlightedColor;

        private bool isScrolling;

        private void Start()
        {
            scrollbar = dropDownTransform.GetChild(dropDownTransform.childCount - 1)
                .GetComponentInChildren<Scrollbar>();
        }

        private void Update()
        {
            if (isScrolling)
            {
                scrollbar.value += scrollSpeed * Time.deltaTime;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isScrolling = true;
            targetImage.color = highlightedColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isScrolling = false;
            targetImage.color = defaultColor;
        }

        public void PointerDown(Vector3 PointerPosition)
        {
            
        }

        public void PointerUp(Vector3 PointerPosition)
        {
        }

        public void PointerEnter(Vector3 PointerPosition)
        {
            OnPointerEnter(new PointerEventData(EventSystem.current) { position = Camera.main.WorldToScreenPoint(PointerPosition) });
        }

        public void PointerExit(Vector3 PointerPosition)
        {
            OnPointerExit(
                new PointerEventData(EventSystem.current) {position = Camera.main.WorldToScreenPoint(PointerPosition)});
        }
    }
}
