using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EVRTH.Scripts.Utility;

public class LayerPresetDropdown : MonoBehaviour {
    private List<string> presetNames;

    [SerializeField]
    private LayerPresetLoader presetLoader;
    [SerializeField]
    private Dropdown presetDropdown;

	public void Start () {
        presetNames = new List<string>();
        foreach (Preset preset in presetLoader.presets){
            presetNames.Add(preset.presetName);
        }

        presetDropdown.ClearOptions();
        presetDropdown.AddOptions(presetNames);
        presetDropdown.onValueChanged.AddListener(delegate {
            ApplyPreset(presetDropdown);
        });
	}
	
    private void ApplyPreset(Dropdown dropdown){
        presetLoader.ApplyPreset(dropdown.value);
    }
}
