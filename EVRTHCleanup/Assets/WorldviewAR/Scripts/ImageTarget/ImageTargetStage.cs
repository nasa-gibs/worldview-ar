using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTargetStage : MonoBehaviour {
    private MarkerTrackableEventHandler markerTracker;

    [SerializeField]
    private GameObject trackedTarget;
    [SerializeField]
    private float threshold = 1;

	void Start () {
        markerTracker = trackedTarget.GetComponent<MarkerTrackableEventHandler>();
	}
	
	void Update () {
        if(markerTracker.tracked && CheckAngle(trackedTarget.transform)){
            transform.position = trackedTarget.transform.position;
            transform.rotation = trackedTarget.transform.rotation;
        }
	}

    private bool CheckAngle(Transform angleTransform){
        float angleX = angleTransform.localEulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;
        float angleZ = angleTransform.localEulerAngles.z;
        angleZ = (angleZ > 180) ? angleZ - 360 : angleZ;
        if (angleX < threshold && angleX > -threshold && angleZ < threshold && angleZ > -threshold){
            return true;
        }
        return false;
    }
}
