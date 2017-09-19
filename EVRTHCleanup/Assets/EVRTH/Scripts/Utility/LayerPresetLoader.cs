using System.Collections;
using System.Collections.Generic;
using EVRTH.Editor;
using EVRTH.Scripts.GIBS;
using EVRTH.Scripts.GlobeNS;
using EVRTH.Scripts.Visualization;
using UnityEngine;

namespace EVRTH.Scripts.Utility
{
    public class LayerPresetLoader : MonoBehaviour
    {
        public InspectorCommentBlock instructions;
        public Globe globe;
        private LayerApplier layerApplier;
        public Date date;
        [Space]
        [Space]
        public int autoApply;

        [HideInInspector]
        public List<Preset> presets = new List<Preset>();
        [HideInInspector]
        public int currentPreset;

        //private List<string> layerList;


        private IEnumerable Start()
        {
            var w = new WaitForEndOfFrame();
            while (!globe.parsedAvailableLayers)
            {
                yield return w;
            }
            layerApplier = globe.GetComponent<LayerApplier>();
            ApplyPreset(autoApply);
            //LayerLoader.Init();
            //layerList = new List<string>(LayerLoader.GetLayers().Keys);
        }

        public void ApplyPreset(int toApply)
        {
            if (presets.Count > toApply && toApply >= 0)
            {
                currentPreset = toApply;
                layerApplier.dataVisualizer0.Reset();
                layerApplier.dataVisualizer1.Reset();
                var set = presets[toApply];
                for (int i = 0; i < set.layersInPreset.Count; i++)
                {
                    layerApplier.ApplyLayer(set.layersInPreset[i],date.ToDateTime,LayerApplier.LayerVisualizationStyle.Flat,i);
                }
                for (int i = 0; i < 2; i++)
                {
                    if(!string.IsNullOrEmpty(set.volumetricLayers[i]))
                        layerApplier.ApplyLayer(set.volumetricLayers[i], date.ToDateTime, LayerApplier.LayerVisualizationStyle.Volumetric, i);
                }
            }
        }
    }
}
