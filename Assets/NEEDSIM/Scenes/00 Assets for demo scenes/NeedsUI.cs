/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NEEDSIMSampleSceneScripts
{
    /// <summary>
    /// This class shows bars for need satisfaction. A full bar equals full satisfaction, an empty bar means the need is not satisfied. If the need is currently being satisfied an outline will be added.
    /// </summary>
    public class NeedsUI : MonoBehaviour
    {
        [Tooltip("The canvas that is the parent of the sliders will be used to rotate this UI to the main cameras rotation.")]
        public Canvas Canvas;
        [Tooltip("These sliders will show how satisfied each need is.")]
        public Slider[] Slider;
        [Tooltip("The names of the needs have to match the needs in the database.")]
        public string[] NeedNames;
     
        private Quaternion targetRotation;

        private NEEDSIM.NEEDSIMNode NEEDSIMNode;

        private Outline[] outlines;

        void Start()
        {
            if (Slider.Length != NeedNames.Length)
            {
                Debug.LogError("NeedsUI is not correctly set up.");
            }

            outlines = new Outline[NeedNames.Length];
            NEEDSIMNode = gameObject.GetComponent<NEEDSIM.NEEDSIMNode>();
            targetRotation = Camera.main.transform.rotation;

            for (int i = 0; i < Slider.Length; i++)
            {
                outlines[i] = Slider[i].fillRect.gameObject.AddComponent<Outline>();
                outlines[i].effectDistance = new Vector2(0.2f, -0.2f);
            }
        }

        void Update()
        {
            //Rotate towards main camera
            Canvas.transform.rotation = targetRotation;

            for (int i = 0; i < NeedNames.Length; i++)
            {
                float needSatisfactionValue
                    = 1 - (NEEDSIMNode.AffordanceTreeNode.SatisfactionLevels.GetValue(NeedNames[i]) / 100);
                //Draw an outline around needs that currently are being satisfied.
                if (Slider[i].value > needSatisfactionValue)
                {
                    outlines[i].enabled = true; 
                }
                else
                {
                    outlines[i].enabled = false;
                }
                Slider[i].value = needSatisfactionValue;
            }
        }
    }
}

