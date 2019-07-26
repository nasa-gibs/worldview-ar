using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnifyGlassTool : MonoBehaviour {
    private const string toolTitleDisplay = "Zoom in on Points of Interest";

    [SerializeField]
    private Slider toolSlider;
    [SerializeField]
    private Text toolTitleText;
    [SerializeField]
    private GameObject imageTarget;
    [SerializeField]
    private Camera magnifyGlass;
	
	void Update () {
        UpdateZoomLevel();
	}

    private void OnEnable()
    {
        InitToolUI();
    }

    private void UpdateZoomLevel(){
        magnifyGlass.fieldOfView = CalculateZoom();
    }

    private float CalculateZoom(){
        RaycastHit hit;
        if (!Physics.Raycast(imageTarget.transform.position + new Vector3(0, 0.02f, 0), -imageTarget.transform.up, out hit))
            return magnifyGlass.fieldOfView;
        return Mathf.Clamp((1 / (toolSlider.value * hit.distance)), 1, 100);
    }

    private void InitToolUI(){
        toolSlider.gameObject.SetActive(true);
        toolSlider.maxValue = 1;
        toolSlider.minValue = 0.3f;
        toolSlider.interactable = true;
        toolSlider.value = 1f;
        toolTitleText.text = toolTitleDisplay;
    }
}
