using System;
using System.Collections.Generic;
using GlobeNS;
using UnityEngine;
using UnityEngine.UI;
using Visualiation;

namespace GIBS
{
    public class GlobeControls : MonoBehaviour
    {

        private Globe globe;
        private LayerApplier layerApplier;

        public Dropdown flatLayerDropdown0;
        public Dropdown flatLayerDropdown1;
        public Dropdown flatLayerDropdown2;

        public Dropdown extrudedlayerDropdown0;
        public Dropdown extrudedlayerDropdown1;

        private bool itemsLoaded;

        private void Start()
        {
            globe = GetComponent<Globe>();
            layerApplier = GetComponent<LayerApplier>();

            if (flatLayerDropdown0 == null)
            {
                flatLayerDropdown0 = FindObjectOfType<Dropdown>();
            }
        }

        private void Update()
        {
            if (!itemsLoaded && globe.parsedAvailableLayers && globe.availableLayers != null)
            {
                PopulateDropdownItems();
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
            {
                UpdateGlobeLayerByName(0, "BlueMarble_NextGeneration", false, new DateTime(2016, 10, 23));
                //UpdateGlobeLayerByName(0, "AMSR2_Surface_Rain_Rate_Day", false, new DateTime(2016, 9, 15));
                //UpdateGlobeLayerByName(1, "AMSR2_Surface_Rain_Rate_Night", false, new DateTime(2016, 9, 15));

                //UpdateGlobeLayerByName(0, "AMSR2_Surface_Rain_Rate_Day", false, new DateTime(2016, 9, 15));
                UpdateGlobeLayerByName(0, "AMSR2_Surface_Rain_Rate_Day", true, new DateTime(2016, 10, 23));
                UpdateGlobeLayerByName(1, "AMSR2_Surface_Rain_Rate_Night", true, new DateTime(2016, 10, 23));

                // Test for alignment
                //UpdateGlobeLayerByName(0, "ASTER_GDEM_Color_Index", false, new DateTime(2016, 9, 15));
                //UpdateGlobeLayerByName(0, "ASTER_GDEM_Color_Index", true, new DateTime(2016, 9, 15));
            }
#endif
        }

        private void PopulateDropdownItems()
        {
            //Add layers from globe dict
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (string dataset in globe.availableLayers.Keys)
            {
                Dropdown.OptionData newOption = new Dropdown.OptionData(dataset);
                options.Add(newOption);
            }

            flatLayerDropdown0.options = options;
            flatLayerDropdown1.options = options;
            flatLayerDropdown2.options = options;

            if (extrudedlayerDropdown0 != null)
            {
                extrudedlayerDropdown0.options =
                    new List<Dropdown.OptionData> {new Dropdown.OptionData("AMSR2_Surface_Rain_Rate_Day"), new Dropdown.OptionData("AIRS_CO_Total_Column_Day") };
            }

            if (extrudedlayerDropdown1 != null)
            {
                extrudedlayerDropdown1.options =
                    new List<Dropdown.OptionData> {new Dropdown.OptionData("AMSR2_Surface_Rain_Rate_Night")};
            }

            itemsLoaded = true;
        }

        public void OnFlatLayer0DropdownChange()
        {
            UpdateGlobeLayer(0, flatLayerDropdown0, false);
        }

        public void OnFlatLayer1DropdownChange()
        {
            UpdateGlobeLayer(1, flatLayerDropdown1, false);
        }

        public void OnFlatLayer2DropdownChange()
        {
            UpdateGlobeLayer(2, flatLayerDropdown2, false);
        }

        public void OnExtrudedLayer0DropdownChange()
        {
            UpdateGlobeLayer(0, extrudedlayerDropdown0, true);
        }

        public void OnExtrudedLayer1DropdownChange()
        {
            UpdateGlobeLayer(1, extrudedlayerDropdown1, true);
        }

        private void UpdateGlobeLayer(int layerIndex, Dropdown dropdown, bool isExtruded)
        {
            string layerName = dropdown.options[dropdown.value].text;
            GlobeLayerInfo globeLayer = globe.layers[layerIndex];

            if (globeLayer.name == null || layerName != globeLayer.name)
            {
                if (isExtruded)
                {
                    Debug.LogFormat("Setting extruded layer {0} to {1}", layerIndex, layerName);
                    layerApplier.ApplyLayer(layerName, new DateTime(2016, 8, 16), LayerApplier.LayerVisualizationStyle.Volumetric, layerIndex);
                }
                else
                {
                    Debug.LogFormat("Setting flat layer {0} to {1}", layerIndex, layerName);
                    layerApplier.ApplyLayer(layerName, new DateTime(2016, 8, 16), LayerApplier.LayerVisualizationStyle.Flat, layerIndex);
                }
            }
        }

        private void UpdateGlobeLayerByName(int layerIndex, string layerName, bool isExtruded, DateTime dateTime)
        {
            if (isExtruded)
            {
                Debug.LogFormat("Setting extruded layer {0} to {1}", layerIndex, layerName);
                layerApplier.ApplyLayer(layerName, dateTime, LayerApplier.LayerVisualizationStyle.Volumetric, layerIndex);
            }
            else
            {
                Debug.LogFormat("Setting flat layer {0} to {1}", layerIndex, layerName);
                layerApplier.ApplyLayer(layerName, dateTime, LayerApplier.LayerVisualizationStyle.Flat, layerIndex);
            }
        }
    }
}
