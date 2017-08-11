using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobeNS;
using UnityEngine;
using Utility;
using WMS;

namespace Visualiation
{
    public class ScienceDataIngestor : MonoBehaviour
    {
        private string layerName;
        public string textureFormatString;
        public string stylesString;

        public bool averageWithPreviousDays;

        public float latStep;
        public float lonStep;

        public bool bilinearlyFilterScienceDataValues;

        //public GameObject tileTemplate;

        public int daysToUse;

        public int colorMissingFromDictCount;
        public int zeroColorValuesFound;

        private bool tileStructuresCreated;

        // Temporary workaround until better data is available about color-value mappings
        //private readonly Dictionary<Color, float> colorValueDictionary = new Dictionary<Color, float>();

        private DownloadManager downloadManager;
        private AbstractGlobe globe;

        private Dictionary<string, List<Tile>> layerTiles;

        private void Start()
        {

            downloadManager = transform.parent.GetComponent<DownloadManager>();
            globe = transform.parent.GetComponent<AbstractGlobe>();
            layerTiles = new Dictionary<string, List<Tile>>();
            //////////////////////////////////////////////////////////////////////////////
            //test
            //IngestDataForDate(new DateTime(2016,08,26), "AIRS_CO_Total_Column_Day", GetComponent<DeformedSphereMaker>());
            //////////////////////////////////////////////////////////////////////////////
        }

        public void ClearDataState()
        {
            // Delete all children of this transform
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            children.ForEach(child => Destroy(child.gameObject));
        }

        public bool AllTilesDownloaded()
        {
            if (!tileStructuresCreated)
            {
                return false;
            }

            foreach (Tile t in transform.GetComponentsInChildren<Tile>())
            {
                if (!t.loadComplete)
                {
                    return false;
                }
            }
            return true;
        }

        public void IngestDataForDate(DateTime loadTime, string layerToLoad, DataVisualizer deformedSphereMaker)
        {
            List<DataVisualizer> deformedSphereMakers = new List<DataVisualizer> {deformedSphereMaker};
            IngestDataForDate(loadTime, layerToLoad, deformedSphereMakers);
        }

        public void IngestDataForDate(DateTime loadTime, string layerToLoad, List<DataVisualizer> deformedSphereMakers, bool reuseAlreadyDownloadedTileSets = false)
        {
            //        Debug.Log("Ingesting data for date " + loadTime.ToShortDateString());

            if (!reuseAlreadyDownloadedTileSets)
            {
                // First clear all sets of tiles
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            //List<Transform> currentDataContainers = new List<Transform>();
            //foreach (Transform child in transform)
            //{
            //    currentDataContainers.Add(child);
            //}

            layerName = layerToLoad;

            LatLonBoundingBox initialMapTileBBox = new LatLonBoundingBox(-90.0f, -90.0f + latStep, -180.0f, -180.0f + lonStep);

            List<string> neededDataContainerObjectNames = new List<string>();
            for (int i = 0; i < daysToUse; i++)
            {
                DateTime currentDate = loadTime.AddDays(-1 * i);
                string dataContainerObjectName = currentDate.ToString("M.dd.yyyy") + " Data";

                neededDataContainerObjectNames.Add(dataContainerObjectName);

                if (reuseAlreadyDownloadedTileSets)
                {
                    // Only add containers that we don't already have
                    if (!layerTiles.ContainsKey(dataContainerObjectName))//transform.Find(dataContainerObjectName) == null)
                    {
                        //GameObject tileParentObject = new GameObject();
                        //tileParentObject.transform.parent = gameObject.transform;

                        //tileParentObject.name = dataContainerObjectName;
                        layerTiles.Add(dataContainerObjectName,new List<Tile>());
                        // Create tiles into which textures can be downloaded
                        CreateTileStructures(currentDate, initialMapTileBBox, layerTiles[dataContainerObjectName], deformedSphereMakers);

                        tileStructuresCreated = true;

                        // Download the textures
                        LoadData(currentDate, layerTiles[dataContainerObjectName]);
                    }
                    else
                    {
                        // We already have this data and don't need to download it. Let's immediately use it for deformation.
                        //Transform tileParentTransform = transform.Find(dataContainerObjectName);
                        StartCoroutine(DeformBasedOnChildTiles(layerTiles[dataContainerObjectName], deformedSphereMakers));
                    }
                }
                else
                {
                    // Add a tile container
                    //GameObject tileParentObject = new GameObject();
                    //tileParentObject.transform.parent = gameObject.transform;

                    //tileParentObject.name = dataContainerObjectName;

                    // Create tiles into which textures can be downloaded
                    layerTiles.Add(dataContainerObjectName, new List<Tile>());
                    CreateTileStructures(currentDate, initialMapTileBBox, layerTiles[dataContainerObjectName], deformedSphereMakers);

                    tileStructuresCreated = true;

                    // Download the textures
                    LoadData(currentDate, layerTiles[dataContainerObjectName]);
                }

            
            }
            // Finally, clean up any old data containers that we don't need
            //foreach (Transform t in currentDataContainers)
            //{
            //    if (!neededDataContainerObjectNames.Contains(t.gameObject.name))
            //    {
            //        Destroy(t.gameObject);
            //    }
            //}
        }
        //deprecated
        private IEnumerator DeformBasedOnChildTiles(Transform tileParentTransform, List<DataVisualizer> deformedSphereMakers)
        {
            foreach (Transform childTransform in tileParentTransform)
            {
                Tile t = childTransform.GetComponent<Tile>();

                // Use each tile to deform the deformSphereMakers
                for (int j = 0; j < deformedSphereMakers.Count; j++)
                {
                    StartCoroutine(deformedSphereMakers[j].DeformBasedOnNewTile(t,0,100));
                    yield return 0;
                }
                yield return 0;
            }
        }

        private IEnumerator DeformBasedOnChildTiles(List<Tile> tiles, List<DataVisualizer> deformedSphereMakers)
        {
            foreach (Tile t in tiles)
            {
                // Use each tile to deform the deformSphereMakers
                for (int j = 0; j < deformedSphereMakers.Count; j++)
                {
                    StartCoroutine(deformedSphereMakers[j].DeformBasedOnNewTile(t, 0, 100));
                    yield return 0;
                }
                yield return 0;
            }
        }
        //deprecated
        public void LoadData(DateTime date, GameObject tileParentObject)
        {
            foreach (Tile tile in tileParentObject.transform.GetComponentsInChildren<Tile>())
            {
                tile.LoadData(layerName, date);
            }
        }

        public void LoadData(DateTime date, List<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].LoadData(layerName, date);
            }
        }

        private void CreateTileStructures(DateTime dateTime, LatLonBoundingBox initialMapTileBBox, GameObject parentObject, List<DataVisualizer> deformedSphereMakers)
        {
            for (float minLat = initialMapTileBBox.minLat; minLat + initialMapTileBBox.DeltaLat <= 90.0f; minLat += initialMapTileBBox.DeltaLat)
            {
                for (float minLon = initialMapTileBBox.minLon; minLon + initialMapTileBBox.DeltaLon <= 180.0f; minLon += initialMapTileBBox.DeltaLon)
                {
                    LatLonBoundingBox bbox = new LatLonBoundingBox(minLat, minLat + initialMapTileBBox.DeltaLat, minLon, minLon + initialMapTileBBox.DeltaLon);
                    //CreateTile(bbox, parentObject, dateTime, deformedSphereMakers);
                }
            }
        }

        private void CreateTileStructures(DateTime dateTime, LatLonBoundingBox initialMapTileBBox, List<Tile> dataContainer, List<DataVisualizer> deformedSphereMakers)
        {
            for (float minLat = initialMapTileBBox.minLat; minLat + initialMapTileBBox.DeltaLat <= 90.0f; minLat += initialMapTileBBox.DeltaLat)
            {
                for (float minLon = initialMapTileBBox.minLon; minLon + initialMapTileBBox.DeltaLon <= 180.0f; minLon += initialMapTileBBox.DeltaLon)
                {
                    LatLonBoundingBox bbox = new LatLonBoundingBox(minLat, minLat + initialMapTileBBox.DeltaLat, minLon, minLon + initialMapTileBBox.DeltaLon);
                    CreateTile(bbox, dataContainer, dateTime, deformedSphereMakers);
                }
            }
        }

        private void CreateTile(LatLonBoundingBox bbox, List<Tile> dataContainer /*GameObject parentObject*/, DateTime dateTime, List<DataVisualizer> deformedSphereMakers)//used to return gameobject
        {
            //GameObject tileGo = Instantiate(tileTemplate);

            //// Don't render
            //tileGo.GetComponent<Renderer>().enabled = false;

            //// Turn off averaging between pixels
            ////tileGO.GetComponent<Tile>().compressionOn = false;

            //tileGo.transform.parent = parentObject.transform;
            //tileGo.name = bbox.ToString();

            Tile tile = new Tile
            {
                bBox = bbox,
                textureFormatString = textureFormatString,
                stylesString = stylesString,
                selectedLayer = layerName,
                selectedDate = dateTime,
                globe = /*parentObject.GetComponentInParent<AbstractGlobe>() ??*/ globe,
                downloadManager = /*parentObject.GetComponentInParent<DownloadManager>() ?? */downloadManager,
                deformedSphereMakers = deformedSphereMakers
            }; //tileGo.GetComponent<Tile>();

            tile.DetermineAndSetUrl();
            dataContainer.Add(tile);
            //return tileGo;
        }

        //public float GetValueFromTileTextures(float lat, float lon, out Color texturePixelColor)
        //{
        //    Tile[] tileList = transform.GetComponentsInChildren<Tile>();
        //    if (tileList.Length > 0)
        //    {
        //        float latStart, latEnd, lonStart, lonEnd;
        //        string tileName = GetTileName(lat, lon, out latStart, out latEnd, out lonStart, out lonEnd);

        //        List<Tile> tilesForThisLatLong = new List<Tile>();

        //        // Get all of the tiles corresponding to the given lat/long, across the dates that we've accumulated
        //        foreach (Transform child in transform)
        //        {
        //            Transform thisTileTransform = child.Find(tileName);
        //            Tile thisTile = thisTileTransform.GetComponent<Tile>();
        //            tilesForThisLatLong.Add(thisTile);
        //        }

        //        List<float> scienceValues = new List<float>();
        //        List<Color> pixelColors = new List<Color>();

        //        // Go through each tile and get the exact pixel value for the given lat/long
        //        foreach (Tile thisTile in tilesForThisLatLong)
        //        {
        //            if (thisTile != null)
        //            {
        //                // Check if the tile is still loading in
        //                if (thisTile.scienceDataTexture == null)
        //                {
        //                    continue;
        //                }
        //                //Debug.Log("Found tile " + tileName + ".");

        //                // Get the pixel values of the specified lat/long (or what's closest, anyway)
        //                Vector2 pixelCoords;
        //                Color pixelColor;

        //                if (bilinearlyFilterScienceDataValues)
        //                {
        //                    pixelCoords = new Vector2((lon - lonStart) / lonStep, (lat - latStart) / latStep);
        //                    pixelColor = (thisTile.scienceDataTexture).GetPixelBilinear(pixelCoords.x, pixelCoords.y);
        //                }
        //                else
        //                {
        //                    float pixelX = thisTile.width * (lon - lonStart) / lonStep;
        //                    float pixelY = thisTile.height * (lat - latStart) / latStep;
        //                    pixelCoords = new Vector2(Mathf.Round(pixelX), Mathf.Round(pixelY));
        //                    pixelColor = (thisTile.scienceDataTexture).GetPixel((int)pixelCoords.x, (int)pixelCoords.y);
        //                }

        //                pixelColors.Add(pixelColor);

        //                float value = ConvertTo1DValue(pixelColor);

        //                scienceValues.Add(value);
        //            }
        //            else
        //            {
        //                Debug.Log("Tile matching name " + tileName + " not found!");
        //            }
        //        }

        //        // Take latest non-zero value (or an average of the values)
        //        List<float> nonZeroValues = new List<float>();
        //        List<Color> nonZeroColors = new List<Color>();

        //        for (int i = 0; i < scienceValues.Count; i++)
        //        {
        //            if (scienceValues[i] != 0)
        //            {
        //                nonZeroValues.Add(scienceValues[i]);
        //                nonZeroColors.Add(pixelColors[i]);
        //            }
        //        }

        //        float valueToShow;
        //        if (nonZeroValues.Count > 0)
        //        {
        //            if (averageWithPreviousDays)
        //            {
        //                valueToShow = nonZeroValues.Average();
        //                float averageR = nonZeroColors.Average(s => s.r);
        //                float averageG = nonZeroColors.Average(s => s.g);
        //                float averageB = nonZeroColors.Average(s => s.b);
        //                float averageA = nonZeroColors.Average(s => s.a);
        //                texturePixelColor = new Color(averageR, averageG, averageB, averageA);
        //            }
        //            else
        //            {
        //                valueToShow = nonZeroValues.First();
        //                texturePixelColor = nonZeroColors.First();
        //            }
        //        }
        //        else
        //        {
        //            valueToShow = 0.0f;
        //            texturePixelColor = Color.black;
        //            //Debug.Log(nonZeroValues.Count.ToString() + " of " + scienceValues.Count());
        //            //Debug.Log("Not found for lat/long " + lat.ToString() + ", " + lon.ToString());
        //        }

        //        return valueToShow;
        //    }
        //    Debug.Log("Error: no tiles found!");

        //    texturePixelColor = Color.black;
        //    return 0.0f;
        //}

        //private float ConvertTo1DValue(Color color)
        //{
        //    if (!colorValueDictionary.ContainsKey(color))
        //    {
        //        // Get closest color and use that instead
        //        Vector3 colorVector = new Vector3(color.r, color.g, color.b);
        //        Color closestColor = colorValueDictionary.Keys.OrderBy(s => Vector3.Distance(new Vector3(s.r, s.g, s.b), colorVector)).First();

        //        colorMissingFromDictCount++;

        //        return colorValueDictionary[closestColor];
        //    }
        //    if (color == new Color(0.0f, 0.0f, 0.0f, 0.0f))
        //    {
        //        zeroColorValuesFound++;
        //    }

        //    return colorValueDictionary[color];
        //}

        // Use latstep/lonstep to find out the name of the tile where the given lat/long would be located
        private string GetTileName(float lat, float lon, out float latStart, out float latEnd, out float lonStart, out float lonEnd)
        {
            if ((-90.0f <= lat) && (lat <= 90.0f) && (-180.0f <= lon) && (lon <= 180.0f))
            {
                latStart = Mathf.Floor((lat + 90.0f) / latStep) * latStep - 90.0f;
                latEnd = latStart + latStep;
                lonStart = Mathf.Floor((lon + 180.0f) / lonStep) * lonStep - 180.0f;
                lonEnd = lonStart + lonStep;

                // Offset to make sure we don't step outside of the possible bounds
                if (latStart >= 90.0f)
                {
                    latStart -= latStep;
                    latEnd -= latStep;
                }
                if (lonStart >= 180.0f)
                {
                    lonStart -= lonStep;
                    lonEnd -= lonStep;
                }

                return new LatLonBoundingBox(latStart, latEnd, lonStart, lonEnd).ToString();
            }
            Debug.LogError("Bad lat/long values given!");

            latStart = 0.0f;
            latEnd = 0.0f;
            lonStart = 0.0f;
            lonEnd = 0.0f;
            return "";
        }
    }
}