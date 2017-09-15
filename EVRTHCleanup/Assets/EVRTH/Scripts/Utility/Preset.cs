using System;
using System.Collections.Generic;

namespace EVRTH.Scripts.Utility
{
    [Serializable]
    public class Preset
    {
        internal string presetName;
        internal List<string> layersInPreset;
        internal List<string> volumetricLayers;

        public Preset()
        {
            layersInPreset = new List<string> {"BlueMarble_ShadedRelief"};
            volumetricLayers = new List<string>(2);
        }

        public bool AddOverlayLayer(string newLayer)
        {
            if (layersInPreset.Count < 5)
            {
                layersInPreset.Add(newLayer);
                return true;
            }
            return false;
        }

        public void SetVolumetricLayer(int index, string layer)
        {
            if (index != 0 && index != 1)
            {
                return;
            }

            volumetricLayers[index] = layer;
        }

        public void SetBaseLayer(string newLayer)
        {
            layersInPreset[0] = newLayer;
        }

    }
}