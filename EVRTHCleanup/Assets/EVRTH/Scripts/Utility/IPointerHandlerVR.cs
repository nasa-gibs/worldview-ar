using UnityEngine;

namespace Utility
{
    interface IPointerHandlerVR
    {
        void PointerDown(Vector3 PointerPosition);
        void PointerUp(Vector3 PointerPosition);
        void PointerEnter(Vector3 PointerPosition);
        void PointerExit(Vector3 PointerPosition);
    }
}
