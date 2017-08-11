﻿using System;
using System.Collections;
using System.Collections.Generic;
using GlobeNS;
using UnityEngine;
using Visualiation;
using WMS;

namespace Utility
{
    [Serializable]
    public class Tile //: MonoBehaviour
    {
        public LatLonBoundingBox bBox;

        private IEnumerator downloadTask;
    
        public Texture2D scienceDataTexture;

        public AbstractGlobe globe;
        public DownloadManager downloadManager;
        public List<DataVisualizer> deformedSphereMakers;

        public string selectedLayer;
        public DateTime selectedDate;

        public readonly int width = 512;
        public readonly int height = 512;

        public string textureFormatString = "image/jpeg";
        public string stylesString = "";

        public string url;

        public bool loadComplete;

        // Starts data download. When download is complete, sets texture
        public void LoadData(string layer, DateTime date)
        {
            downloadManager.RequestTexture(url, selectedLayer, selectedDate, LoadTexture, false);
            loadComplete = false;
        }

        public void LoadTexture(string layer, DateTime date, Texture myTexture)
        {
            scienceDataTexture = (Texture2D) myTexture;
            loadComplete = true;

            float minVal = ((Globe) globe).availableLayers[layer].Colormap.min;
            float maxVal = ((Globe) globe).availableLayers[layer].Colormap.max;

            //Debug.Log(minVal + ", " + maxVal);

            // Deform associated globe model
            for (int i = 0; i < deformedSphereMakers.Count; i++)
            {
                deformedSphereMakers[i].StartCoroutine(deformedSphereMakers[i].DeformBasedOnNewTile(this, minVal, maxVal));
            }

        }

        public void DetermineAndSetUrl()
        {
            string dateStr = selectedDate.ToString("yyyy-MM-dd");
            WmsRequest request = new WmsRequest(selectedLayer, width, height, bBox, dateStr, format: textureFormatString, styles: stylesString);
            url = request.Url;
        }
    }
}
