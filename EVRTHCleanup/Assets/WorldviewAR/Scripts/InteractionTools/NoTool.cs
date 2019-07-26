using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoTool : MonoBehaviour {
    private const string toolTitleDisplay = "No Tool Activated";

    [SerializeField]
    private Text toolTitleText;
    [SerializeField]
    private Slider toolSlider;

	private void OnEnable()
	{
        UpdateToolUI();
	}

    private void UpdateToolUI(){
        toolSlider.gameObject.SetActive(false);
        toolTitleText.text = toolTitleDisplay;
    }
}
