/**
 * This is part of the NEEDSIM Life simulation plug in for Unity, version 1.0.1
 * Copyright 2014 - 2015 Fantasieleben UG (haftungsbeschraenkt)
 *
 * http;//www.fantasieleben.com for further details.
 *
 * For questions please get in touch with Tilman Geishauser: tilman@fantasieleben.com
 */

using UnityEngine;
using System.Collections;

namespace NEEDSIMSampleSceneScripts
{
    /// <summary>
    /// A very simple scrolling camera for NEEDSIM Life simulation example scenes.
    /// </summary>
    public class SampleCameraControl : MonoBehaviour
    {
        [Tooltip("Movement speed of scrolling.")]
        public float speed = 0.11f;
        [Tooltip("clamp camera scrolling to e.g. map size, horizontally.")]
        public Vector2 HorizontalMinMax;
        [Tooltip("clamp camera scrolling to e.g. map size, vertically.")]
        public Vector2 VerticalMinMax;

        void Update()
        {
            float horizontalSpeed = Input.GetAxis("Horizontal") * speed;
            float verticalSpeed = Input.GetAxis("Vertical") * speed;
            // Keep the camera within the horizontal bounds.
            if ((transform.position.x <= HorizontalMinMax.x && horizontalSpeed < 0)
            || (transform.position.x >= HorizontalMinMax.y && horizontalSpeed > 0))
            {
                horizontalSpeed = 0.0f;
            }
            //Keep the camera within the vertical bounds.
            if ((transform.position.z <= VerticalMinMax.x && verticalSpeed < 0)
                || (transform.position.z >= VerticalMinMax.y && verticalSpeed > 0))
            {
                verticalSpeed = 0.0f;
            }

            //Translate according to the coordinates in our 2D sample scenes.
            transform.Translate(horizontalSpeed, verticalSpeed, 0.0f);
        }
    }
}
