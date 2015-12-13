using UnityEngine;
using System.Collections;

namespace ICE.Creatures.Demo
{
	public enum DoorStatus
	{
		STOP,
		CLOSE,
		OPEN

	}

	public class ice_DoorHandler : MonoBehaviour {

		public string KeyTag;
		public GameObject DoorLeft;
		private Vector3 DoorLeftOpen;
		private Vector3 DoorLeftClose;
		public GameObject DoorRight;
		private Vector3 DoorRightOpen;
		private Vector3 DoorRightClose;

		public float Size = 1.2f;
		public float Speed = 1.5f;
		public float StopDistance = 0.05f;

		private bool isOpen = false;
		public bool CloseDoor = false;

		public DoorStatus Status = DoorStatus.STOP;

		private float _door_timer = 0;
		private float _door_close_time = 5;

		// Use this for initialization
		void Start () {

			DoorLeftClose = DoorLeft.transform.localPosition;
			DoorRightClose = DoorRight.transform.localPosition;

			DoorLeftOpen = DoorLeft.transform.localPosition + new Vector3( Size, 0, 0);
			DoorRightOpen = DoorRight.transform.localPosition + new Vector3( -Size, 0, 0);
		}
		
		// Update is called once per frame
		void Update () {

			if( ! isOpen && Status == DoorStatus.STOP )
				GetComponent<NavMeshObstacle>().enabled = true;
			else
				GetComponent<NavMeshObstacle>().enabled = false;

			if( isOpen )
			{
				_door_timer += Time.deltaTime;

				if( _door_timer >= _door_close_time )
					Status = DoorStatus.CLOSE; 
			}
			else
			{
				_door_timer = 0;
			}



			if( Status == DoorStatus.OPEN )
			{
				bool _left_open = false;
				bool _right_open = false;

				float _dist_left = Vector3.Distance( DoorLeft.transform.localPosition, DoorLeftOpen );
				if( _dist_left > StopDistance )
					DoorLeft.transform.localPosition = Vector3.Lerp( DoorLeft.transform.localPosition, DoorLeftOpen, Speed * Time.deltaTime);
				else
					_left_open = true;

				float _dist_right = Vector3.Distance( DoorRight.transform.localPosition, DoorRightOpen );
				if( _dist_right > StopDistance)
					DoorRight.transform.localPosition = Vector3.Lerp( DoorRight.transform.localPosition, DoorRightOpen, Speed * Time.deltaTime);
				else
					_right_open = true;

				if( _left_open && _right_open )
				{
					Status = DoorStatus.STOP;
					isOpen = true;
				}
			}
			else if( Status == DoorStatus.CLOSE )
			{
				//GetComponent<NavMeshObstacle>().enabled = false;

				bool _left_closed = false;
				bool _right_closed = false;

				float _dist_left = Vector3.Distance( DoorLeft.transform.localPosition, DoorLeftClose );
				if( _dist_left > StopDistance )
					DoorLeft.transform.localPosition = Vector3.Lerp( DoorLeft.transform.localPosition, DoorLeftClose, Speed * Time.deltaTime);
				else
					_left_closed = true;

				float _dist_right = Vector3.Distance( DoorRight.transform.localPosition, DoorRightClose );
				if( _dist_right > StopDistance )
					DoorRight.transform.localPosition = Vector3.Lerp( DoorRight.transform.localPosition, DoorRightClose, Speed * Time.deltaTime);
				else
					_right_closed = true;

				if( _left_closed && _right_closed )
				{
					Status = DoorStatus.STOP;
					isOpen = false;
				}
			}
		}

		void OnTriggerEnter( Collider other) 
		{
			if( other.CompareTag( KeyTag ) || other.CompareTag( "Player" ) )
			{
				_door_timer = 0;
				Status = DoorStatus.OPEN;
			}
			
		}

		void OnTriggerStay(Collider other) {
			if( other.CompareTag( KeyTag ) || other.CompareTag( "Player" ) )
			{
				_door_timer = 0;
				Status = DoorStatus.OPEN;
			}     
		}

		void OnTriggerExit(Collider other) {
			if( other.CompareTag( KeyTag ) || other.CompareTag( "Player" ) )
				Status = DoorStatus.CLOSE;  
		}
	}
}
