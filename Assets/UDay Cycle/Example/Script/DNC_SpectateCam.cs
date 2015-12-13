using UnityEngine;
using System.Collections;

public class DNC_SpectateCam : MonoBehaviour {

    public float lookSpeed = 15.0f;
    public float moveSpeed = 15.0f;
    public Transform MidPoint;
    public float m_maxDistance = 200;
    float rotationX = 0.0f;
    float rotationY = 0.0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Screen.lockCursor = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.lockCursor = false;
        }
        float m_dist = Vector3.Distance(this.transform.position, MidPoint.transform.position);
        
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
            if (m_dist <= m_maxDistance)
            {
                transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
                transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, MidPoint.transform.position, Time.deltaTime * 5);
            }
    }
}