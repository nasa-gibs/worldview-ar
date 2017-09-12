using System;
using System.Collections;
using System.Collections.Generic;
using EVRTH.Scripts.GlobeNS;
using EVRTH.Scripts.Visualization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EVRTH.Scripts.Utility
{
    public class FeatureWalkthroughManager : MonoBehaviour
    {
        public LayerApplier layerApplier;
        public Globe globe;
        public List<UnityEvent> actions;
        public List<string> stepDescriptionsList;
        public int currAction;
        public float cooldown;
        public Text stepDescriptionDisplay;
        public bool nextStep;
        private float lastAction;
        private bool isReady;

        private IEnumerator Start()
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            lastAction = float.MinValue;
            SetupLayerEvents();
            while (!globe.parsedAvailableLayers || globe.availableLayers == null)
            {
                yield return wait;
            }
            NextAction();
            isReady = true;
        }

        //sets up pre-scripted layer changes
        //done in this way so that it is done in addition to any other things already on the events such as turning on/off gameobjects
        //or triggering methods on other GameObjects
        private void SetupLayerEvents()
        {
            if (actions == null)
            {
                actions = new List<UnityEvent>();
            }
            if(actions.Count == 0)
                actions.Add(new UnityEvent());
            (actions[0] ?? (actions[0] = new UnityEvent())).AddListener(() =>
            {
                UpdateGlobeLayer(0, "BlueMarble_ShadedRelief_Bathymetry");
                stepDescriptionDisplay.text = stepDescriptionsList[0];
            });

            if (actions.Count == 1)
                actions.Add(new UnityEvent());
            (actions[1] ?? (actions[1] = new UnityEvent())).AddListener(() =>
            {
                UpdateGlobeLayer(1, "MODIS_Fires_All");
                stepDescriptionDisplay.text = stepDescriptionsList[1];
            });

            if (actions.Count == 2)
                actions.Add(new UnityEvent());
            (actions[2] ?? (actions[2] = new UnityEvent())).AddListener(() =>
            {
                UpdateGlobeLayer(0, "AIRS_CO_Total_Column_Day",true);
                stepDescriptionDisplay.text = stepDescriptionsList[2];
            });

            if (actions.Count == 3)
                actions.Add(new UnityEvent());
            (actions[3] ?? (actions[3] = new UnityEvent())).AddListener(() =>
            {
                UpdateGlobeLayer(2, "AMSR2_Surface_Rain_Rate_Day");
                stepDescriptionDisplay.text = stepDescriptionsList[3];
            });

            if (actions.Count == 4)
                actions.Add(new UnityEvent());
            (actions[4] ?? (actions[4] = new UnityEvent())).AddListener(() =>
            {
                stepDescriptionDisplay.text = stepDescriptionsList[4];
            });

            if (actions.Count == 5)
                actions.Add(new UnityEvent());
            (actions[5] ?? (actions[5] = new UnityEvent())).AddListener(() =>
            {
                UpdateGlobeLayer(0, "AMSR2_Surface_Rain_Rate_Day", true);
                stepDescriptionDisplay.text = stepDescriptionsList[5];
            });

            if (actions.Count == 6)
                actions.Add(new UnityEvent());
            (actions[6] ?? (actions[6] = new UnityEvent())).AddListener(() =>
            {
                stepDescriptionDisplay.text = stepDescriptionsList[6];
            });
        }

        private void Update()
        {
            if (!isReady) return;
            //if vr controller trigger or enter/return is pressed
            if (nextStep)
            {
                nextStep = false;
                //and enough time has elapsed
                if (Time.realtimeSinceStartup - lastAction > cooldown)
                {
                    //do the next action in the time-line
                    NextAction();
                    lastAction = Time.realtimeSinceStartup;
                }
            }
        }


        public void NextAction()
        {
            if (currAction == actions.Count)
                return;

            //perform the next event in the series as long as its not null
            if (actions[currAction] != null)
                actions[currAction].Invoke();

            //increment the current action
            currAction++;
        }

        private void UpdateGlobeLayer(int layerIndex,string layerName, bool isExtruded = false)
        {
            GlobeLayerInfo globeLayer = globe.layers[layerIndex];

            if (globeLayer.name == null || layerName != globeLayer.name || isExtruded)
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
    }
}
