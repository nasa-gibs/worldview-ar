using System.Collections.Generic;
using GIBS;
using GlobeNS;
using UnityEngine;

internal struct TileCreationParams
{
    public GlobeTile parent;
    public Wmts coords;

    public TileCreationParams(GlobeTile parent, Wmts coords)
    {
        this.parent = parent;
        this.coords = coords;
    }
}

public class TileFactory : MonoBehaviour
{
    public int tilesPerFrame = 3;
    private Queue<TileCreationParams> tileQueue = new Queue<TileCreationParams>();

    [HideInInspector]
    public int queueSize;

    public GameObject tileTemplate;
    public Globe globe;

    private void Update()
    {
        queueSize = tileQueue.Count;

        for (int i = 0; i < tilesPerFrame; i++)
        {
            if (tileQueue.Count > 0)
            {
                TileCreationParams parameters = tileQueue.Dequeue();
                CreateTile(parameters.parent, parameters.coords);
            }
            else
            {
                break;
            }
        }
    }

    public void CreateTileAsync(GlobeTile parent, Wmts coords)
    {
        tileQueue.Enqueue(new TileCreationParams(parent, coords));
    }

    private void CreateTile(GlobeTile parent, Wmts coords)
    {
        GameObject tileGO = Instantiate(tileTemplate);

        tileGO.transform.SetParentClearRelativeTransform(parent.transform);
        tileGO.name = coords.ToString();

        GlobeTile tile = tileGO.GetComponent<GlobeTile>();
        tile.coords = coords;
        tile.globe = globe;
        tile.bBox = globe.CurrentLayer.Wmts2Bbox(coords);

        tile.SetVisible(false);

        parent.AddChild(tile);
    }

	public GlobeTile CreateTile(Transform parent, Wmts coords)
	{
		GameObject tileGO = Instantiate(tileTemplate);

        tileGO.transform.SetParentClearRelativeTransform(parent);
        tileGO.name = coords.ToString();

        GlobeTile tile = tileGO.GetComponent<GlobeTile>();
        tile.coords = coords;
        tile.globe = globe;
        tile.bBox = Layer.Wmts2DefaultBbox(coords);
        tile.SetVisible(false);
		return tile;
	}
}
