using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour {
    private List<string> toolNames;

    [SerializeField]
    private GameObject[] tools;

	private void Awake () {
        SetToolNames();
	}

	private void Start()
	{
        ActivateTool(0);
	}

	private void SetToolNames(){
        toolNames = new List<string>();

        foreach (GameObject tool in tools)
        {
            toolNames.Add(tool.name);
        }
    }

    public List<string> GetToolNames(){
        return toolNames;
    }

    public void ActivateTool(int toolIndex)
    {
        foreach (GameObject tool in tools)
        {
            tool.SetActive(false);
        }

        tools[toolIndex].SetActive(true);
    }
}
