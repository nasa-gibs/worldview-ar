using UnityEngine;
using System.Collections;
using EVRTH.Scripts.GIBS;
using EVRTH.Scripts.GlobeNS;
using UnityEngine.UI;

public class ColorProbeTool : MonoBehaviour {
    private ColorMap colorMap;
    private MarkerTrackableEventHandler markerTrackable;
    private const string invalidColorMapTitle = "Please Selection a Layer with a Valid Color Map";
    private const string invalidColorMapProbe = "Invalid Color Map";

    [SerializeField]
    private Slider toolSlider;
    [SerializeField]
    private Text toolTitleText, probeText;
    [SerializeField]
    private GameObject imageTarget, probeBody, probeTop;
    [SerializeField]
    private Globe globe;
    [SerializeField]
    private LineRenderer laserLine;

	private void Start()
	{
        markerTrackable = imageTarget.transform.parent.gameObject.GetComponent<MarkerTrackableEventHandler>();
	}

	private void Update()
    {
        ProbeColorMap();
    }

    private void OnEnable()
    {
        InitToolUI();
    }

    private void ProbeColorMap(){
        laserLine.enabled = false;
        probeText.transform.parent.gameObject.transform.rotation = Quaternion.LookRotation(imageTarget.transform.position - Camera.main.transform.position);
        if (!CheckColorMap())
        {
            probeText.text = invalidColorMapProbe;
            toolTitleText.text = invalidColorMapTitle;
            return;
        }
        colorMap = globe.CurrentLayer.Colormap;
        toolTitleText.text = colorMap.title + " Color Map";
        toolSlider.maxValue = colorMap.max;
        toolSlider.minValue = colorMap.min;

        RaycastHit hit;
        if (!Physics.Raycast(imageTarget.transform.position + imageTarget.transform.up * 0.1f, -imageTarget.transform.up, out hit) || !markerTrackable.tracked)
            return;
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Texture2D tex = rend.material.GetTexture("_NewTex") as Texture2D;

        laserLine.enabled = true;
        laserLine.SetPosition(0, transform.position + transform.up * 0.01f);
        laserLine.SetPosition(1, hit.point);

        if (tex == null) { return; }

        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width * hit.collider.gameObject.GetComponent<Renderer>().material.GetTextureScale("_NewTex").x;
        pixelUV.y *= tex.height * hit.collider.gameObject.GetComponent<Renderer>().material.GetTextureScale("_NewTex").y;
        pixelUV.x += tex.width * hit.collider.gameObject.GetComponent<Renderer>().material.GetTextureOffset("_NewTex").x;
        pixelUV.y += tex.height * hit.collider.gameObject.GetComponent<Renderer>().material.GetTextureOffset("_NewTex").y;


        Color colorSelect = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
        Vector3 color = new Vector3(colorSelect.r, colorSelect.g, colorSelect.b) * 255;
        DataRange colorRange = colorMap.GetRange(color);

        laserLine.material.color = colorSelect;
        SetProbeColor(new Color(colorSelect.r, colorSelect.g, colorSelect.b));
        toolSlider.value = (colorRange.max + colorRange.min) / 2;
        probeText.text = Mathf.Round(colorRange.min).ToString() + " - " + Mathf.Round(colorRange.max).ToString() + " " + colorMap.units;
    }

    private bool CheckColorMap(){
        if (globe.CurrentLayer.colormapUrl == null)
        {
            return false;
        }
        return true;
    }

    private void InitToolUI(){
        toolSlider.gameObject.SetActive(true);
        toolSlider.interactable = false;
    }

    private void SetProbeColor(Color color){
        probeBody.GetComponent<Renderer>().material.color = color;
        probeTop.GetComponent<Renderer>().material.color = color;
    }
}
