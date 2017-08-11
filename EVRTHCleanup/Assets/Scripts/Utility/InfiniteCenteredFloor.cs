using UnityEngine;

namespace Simulation
{
    public class InfiniteCenteredFloor : MonoBehaviour 
    {
        [Tooltip( "The prefab to use as a floor" )]
        [SerializeField]
        protected GameObject floorPrefab;

        [Tooltip( "How off center the floor can get before resetting" )]
        [SerializeField]
        protected Vector2 floorGridSize = new Vector2(1, 1);

        protected GameObject activeFloorPrefab;
        protected Renderer activeFloorRenderer;
        protected MaterialPropertyBlock floorDataBlock;
        protected int fadeDataId;
        protected Vector4 fadeData;
        /*protected int pulseDataID;
	protected Vector4 pulseData;	
	protected int pulseDisplayDataID;
	protected Vector2 pulseDisplayData;
*/
        private Transform hmdCamera;

        // We don't allow negative floor grid sizes
        private void OnValidate()
        {
            floorGridSize = Vector2.Max(floorGridSize, new Vector2(0.0f, 0.0f));
        }

        private void Start () 
        {
            // Spawn the floor prefab
            activeFloorPrefab = Instantiate(floorPrefab);
            activeFloorPrefab.transform.parent = transform;
            activeFloorRenderer = activeFloorPrefab.GetComponent<Renderer>();
            activeFloorRenderer.enabled = true;
            floorDataBlock = new MaterialPropertyBlock();
            fadeDataId = Shader.PropertyToID("_fadeData");
            fadeData = activeFloorRenderer.sharedMaterial.GetVector(fadeDataId);
            /*
		pulseDataID = Shader.PropertyToID("_pulseData");
		pulseData = activeFloorRenderer.sharedMaterial.GetVector(pulseDataID);
		pulseDisplayDataID = Shader.PropertyToID("_pulseDisplayData");
		pulseDisplayData = activeFloorRenderer.sharedMaterial.GetVector(pulseDisplayDataID);
*/
            hmdCamera = Camera.main.transform;

            // Initialize them
            RecenterFloor();
        }

        private void LateUpdate () 
        {
            UpdateHexFloorPosition ();
            RecenterFloor();
        }

        private void UpdateHexFloorPosition()
        {
            Vector3 pos = hmdCamera.position;
            pos.y = 0f;
            transform.position = pos;
        }

        private void RecenterFloor()
        {
            // Get the location of the floor rounded to grid-space space
            var quantizedLocation = transform.position;
            if (floorGridSize.x > 0)
            {
                quantizedLocation.x = Mathf.Round(quantizedLocation.x / floorGridSize.x) * floorGridSize.x;
            }

            if (floorGridSize.y > 0)
            {
                quantizedLocation.z = Mathf.Round(quantizedLocation.z / floorGridSize.y) * floorGridSize.y;
            }

            // Set the floor prefab in this quantized location
            activeFloorPrefab.transform.position = quantizedLocation;
            fadeData.Set(transform.position.x, transform.position.y, transform.position.z, fadeData.w);
            floorDataBlock.SetVector(fadeDataId, fadeData);

            activeFloorRenderer.SetPropertyBlock(floorDataBlock);
        }
    }
}
