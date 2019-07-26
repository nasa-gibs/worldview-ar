using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolDropdown : MonoBehaviour {
    private ToolManager toolManager;

    [SerializeField]
    private Dropdown toolDropdown;

    private void Start()
    {
        toolManager = GetComponent<ToolManager>();
        InitToolDropdown();
    }

    private void InitToolDropdown()
    {
        toolDropdown.ClearOptions();
        toolDropdown.AddOptions(toolManager.GetToolNames());
        toolDropdown.onValueChanged.AddListener(delegate {
            ActivateTool(toolDropdown);
        });

        ActivateTool(toolDropdown);
    }

    private void ActivateTool(Dropdown dropdown)
    {
        toolManager.ActivateTool(dropdown.value);
    }
}
