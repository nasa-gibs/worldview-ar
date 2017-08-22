using System.Collections.Generic;
using UnityEngine;

namespace GIBS
{
    public class LayerTest : MonoBehaviour
    {
        private void Start()
        {

            Dictionary<string, Layer> layers = LayerLoader.GetLayers();
            foreach (KeyValuePair<string, Layer> item in layers)
            {
                //Debug.Log (item.Key);
            }

//		Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(0, 0, 3));
//		Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(0, 0, 4));
//		Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(0, 0, 5));
//		Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(0, 0, 6));
//
//		Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(0, 0, 7));
//

            //Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].BBox2Wmts(new LatLonBoundingBox(54,90,-144,-108)));
            //Debug.Log( layers["VIIRS_SNPP_CorrectedReflectance_TrueColor"].Wmts2Bbox(72, 89, 8));	

            //Debug.Log (layers ["AIRS_CO_Total_Column_Day"].colormap.GetRange(new Color(1,1,1)));
            Debug.Log(layers["AIRS_CO_Total_Column_Day"].Colormap.GetRange(new Color(251f / 255, 209f / 255, 251f / 255))
                .max);
//		var blah = layers ["AIRS_CO_Total_Column_Day"].colormap.colors.Keys;
//		Debug.Log (blah);
//		foreach (Color bleh in blah) {
//			Debug.Log (bleh);
//		}
        }

    }
}
