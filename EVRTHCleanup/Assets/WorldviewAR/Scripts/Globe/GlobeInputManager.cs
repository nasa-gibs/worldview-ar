using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeInputManager : MonoBehaviour {
    private bool isRotating = false;

    [SerializeField]
    private GameObject globe;

    void Update()
    {
        RotateOnTouch();
        ScaleOnTouch();
    }

    private void RotateOnTouch(){
        if (Input.touchCount == 1)
        {
            Touch touch0 = Input.GetTouch(0);

            if (touch0.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch0.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform.parent.gameObject == globe)
                {
                    isRotating = true;
                }
            }
            else if (touch0.phase == TouchPhase.Moved && isRotating == true)
            {
                globe.transform.Rotate(Camera.main.transform.up * -touch0.deltaPosition.x, Space.World);
                globe.transform.Rotate(Camera.main.transform.right * touch0.deltaPosition.y, Space.World);
            }
            else if (touch0.phase == TouchPhase.Ended){
                isRotating = false;
            }
        }
    }

    private void ScaleOnTouch(){
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float scale = globe.transform.localScale.x;
            scale -= deltaMagnitudeDiff * 0.01f;
            scale = Mathf.Clamp(scale, 0.1f, 2f);
            globe.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
