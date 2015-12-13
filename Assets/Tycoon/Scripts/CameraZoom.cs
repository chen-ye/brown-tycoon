using UnityEngine;
using System.Collections;

[System.Serializable]
public class CameraZoom : MonoBehaviour {

    public float multiplier = 10f;

    void LateUpdate()
    {
        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.fieldOfView += 4 * multiplier * scroll;
            Camera.main.orthographicSize += multiplier * scroll;
        }
        //-------Code to switch camera between Perspective and Orthographic--------
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (Camera.main.orthographic == true)
                Camera.main.orthographic = false;
            else
                Camera.main.orthographic = true;
        }
    }
}
