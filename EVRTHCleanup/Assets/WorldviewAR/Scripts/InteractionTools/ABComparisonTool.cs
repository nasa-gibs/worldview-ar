using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EVRTH.Scripts.Utility;

public class ABComparisonTool : MonoBehaviour {
    private float maxRadius = 0.1f;
    private string savedLayerText;
    private MarkerTrackableEventHandler markerTrackable;

    [SerializeField]
    private Slider toolSlider;
    [SerializeField]
    private Text toolTitleText, saveButtonText;
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private GameObject imageTarget, probe;
    [SerializeField]
    private MapTileManager mapTileManager;
    [SerializeField]
    private LayerPresetLoader layerPresetLoader;
    [SerializeField]
    private LineRenderer laserLine;

	private void Start()
	{
        if(saveButton){
            saveButton.onClick.AddListener(mapTileManager.SaveTextureAllTiles);
            saveButton.onClick.AddListener(SetTitleText); 
        }
        markerTrackable = imageTarget.transform.parent.gameObject.GetComponent<MarkerTrackableEventHandler>();
	}

	private void Update () {
        UpdateABComparison();
        UpdateSaveButton();
	}

    private void OnEnable()
    {
        InitToolUI();
        mapTileManager.SetABToggleAllTiles(true);
    }

    private void OnDisable()
    {
        HideToolUI();
        mapTileManager.SetABToggleAllTiles(false);
    }

    private void SetTitleText(){
        savedLayerText = string.Format("Saved Layer: {0} at {1}/{2}/{3}", layerPresetLoader.presets[layerPresetLoader.currentPreset].presetName, layerPresetLoader.date.month, layerPresetLoader.date.day, layerPresetLoader.date.year);
        toolTitleText.text = savedLayerText;
    }

    private void UpdateABComparison(){
        mapTileManager.SetABRadiusAllTiles(toolSlider.normalizedValue * maxRadius);
        laserLine.enabled = false;
        RaycastHit hit;
        if (!Physics.Raycast(imageTarget.transform.position + imageTarget.transform.up * 0.1f, -imageTarget.transform.up, out hit) || !markerTrackable.tracked)
            return;
        mapTileManager.SetABPositionAllTiles(hit.point);
        if (hit.distance < 0.125f){
            probe.SetActive(false);
            return;
        }
        probe.SetActive(true);
        laserLine.enabled = true;
        laserLine.SetPosition(0, transform.position + transform.up * 0.01f);
        laserLine.SetPosition(1, hit.point);
        laserLine.startWidth = 0.006f;
        laserLine.endWidth = toolSlider.normalizedValue * maxRadius * 2f;
        laserLine.alignment = LineAlignment.Local;
    }

    private void UpdateSaveButton(){
        if (!saveButton) { return; }
        saveButton.interactable = mapTileManager.CheckTilesLoaded();
        if (!saveButton.interactable)
        {
            saveButtonText.text = "Loading: " + mapTileManager.PercentTilesLoaded().ToString() + "%";
        }
        else
        {
            saveButtonText.text = "Save Layer";
        }
    }

    private void InitToolUI(){
        toolSlider.gameObject.SetActive(true);
        toolSlider.maxValue = 1;
        toolSlider.minValue = 0;
        toolSlider.interactable = true;
        toolSlider.value = 0.5f;
        if (saveButton){
            saveButton.gameObject.SetActive(true);
        }
        toolTitleText.text = savedLayerText ?? "No Saved Layer";
    }

    private void HideToolUI(){
        if (!saveButton) { return; }
        saveButton.gameObject.SetActive(false);
    }
}
