using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVRTH.Scripts.Geometry;

public class GlobeMarker : MonoBehaviour {
    private LatLongValue latLongValue;

    //HACK: Scale factors acquired from trial and error. Need to dynamically calculate these.
    private float scaleFactorX = 0.315f;
    private float scaleFactorZ = 0.156f;
    private float scaleFactorGlobe = 0.515f;
                                      
    [SerializeField]
    private GameObject mapTarget, markerTarget, globe;
    [SerializeField]
    private MarkerTrackableEventHandler markerTracker;

	private void Start () {
        latLongValue = new LatLongValue(0, 0, 0);
	}
	
	private void Update () {
        UpdateGlobeMarker();
	}

    private void UpdateGlobeMarker(){
        if (markerTracker.tracked == true)
        {
            latLongValue.latitude = Mathf.Clamp(90f * (markerTarget.transform.InverseTransformPoint(mapTarget.transform.position).z) / scaleFactorZ, -90, 90);
            latLongValue.longitude = Mathf.Clamp(180f * (markerTarget.transform.InverseTransformPoint(mapTarget.transform.position).x) / scaleFactorX, -180, 180);
            Vector3 relativePosition = LatLongValue.Get3DPoint(latLongValue, 1, 0, 0, true) * scaleFactorGlobe;
            //Adjusts for the rotation of the "Globe" gameobject
            transform.localPosition = new Vector3(relativePosition.z, -relativePosition.x, -relativePosition.y);
        }
        transform.rotation = Quaternion.LookRotation(transform.position - globe.transform.position);
    }
}
