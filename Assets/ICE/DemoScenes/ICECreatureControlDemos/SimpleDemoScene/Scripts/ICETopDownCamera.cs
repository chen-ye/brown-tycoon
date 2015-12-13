using UnityEngine;
using System.Collections;

namespace ICE.Shared
{
	public class ICETopDownCamera : MonoBehaviour {

		public GameObject Target;
		public float damping = 10;
		Vector3 offset;

		// Use this for initialization
		void Start () {

			Vector3 new_pos = Target.transform.position;

			new_pos.x += 0;
			new_pos.z -= 10;
			new_pos.y += 5;

			transform.position = new_pos;
			offset = Target.transform.position - transform.position;
		}
		
		// Update is called once per frame
		void Update () {
			//transform.LookAt( Target.transform );

			//transform.rotation = Quaternion.Slerp ( m_Owner.transform.rotation, tmp_destination_rotation, behaviour.MoveAngularVelocity.y * Time.deltaTime );
			//transform.position += Target.transform.TransformDirection( - Vector3.forward ) * 10 * Time.deltaTime;

			if (Input.GetKey(KeyCode.Q))
			{


			}
			else if (Input.GetKey(KeyCode.A))
			{
				if( offset.z > -50 )
					offset.z -= 1;
			}

			if(Input.GetAxis("Mouse ScrollWheel") <0)
			{
				if( offset.z > 5 )
					offset.z -= 1;

				if( offset.y < -5 )
					offset.y += 2;
				
			} 
			else if (Input.GetAxis("Mouse ScrollWheel") >0)
			{
				if( offset.z < 50 )
					offset.z += 1;

				if( offset.y > -50 )
					offset.y -= 2;
				
			}

		}

		void LateUpdate() {

			Debug.Log ( offset.ToString() );

			float currentAngle = transform.eulerAngles.y;
			float desiredAngle = Target.transform.eulerAngles.y;
			float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
			
			Quaternion rotation = Quaternion.Euler(0, angle, 0);
			transform.position = Target.transform.position - (rotation * offset);
			
			transform.LookAt(Target.transform);
		}
	}
}
