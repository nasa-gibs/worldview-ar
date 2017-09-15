using System.Collections.Generic;
using EVRTH.Scripts.GIBS;
using EVRTH.Scripts.Visualization;
using UnityEngine;

namespace EVRTH.Scripts.Utility
{
    public class LayerPresetLoader : MonoBehaviour
    {
        public LayerApplier layerApplier;
        private Date date;

        public List<Preset> presets;

        private List<string> layerList;


        private void Start()
        {
            LayerLoader.Init();
            layerList = new List<string>(LayerLoader.GetLayers().Keys);
        }

        public void ApplyPreset(int toApply)
        {
            if (presets.Count > toApply && toApply > 0)
            {
                var set = presets[toApply];
                for (int i = 0; i < set.layersInPreset.Count; i++)
                {
                    layerApplier.ApplyLayer(set.layersInPreset[i],date.ToDateTime,LayerApplier.LayerVisualizationStyle.Flat,i);
                }
                for (int i = 0; i < 2; i++)
                {
                    layerApplier.ApplyLayer(set.volumetricLayers[i], date.ToDateTime, LayerApplier.LayerVisualizationStyle.Volumetric, i);
                }
            }
        }
    }
}
