using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utility
{
    interface IPointerHandlerVR
    {
        void PointerDown(Vector3 PointerPosition);
        void PointerUp(Vector3 PointerPosition);
        void PointerEnter(Vector3 PointerPosition);
        void PointerExit(Vector3 PointerPosition);
    }
}
