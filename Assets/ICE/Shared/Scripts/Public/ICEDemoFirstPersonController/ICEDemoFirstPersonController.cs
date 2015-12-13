using UnityEngine;
using System.Collections;
using ICE.Shared.Objects;

namespace ICE.Shared
{
	[RequireComponent(typeof(CharacterController))]
	public class ICEDemoFirstPersonController : MonoBehaviour
	{
		private static ICEDemoFirstPersonController m_Player;

		public static ICEDemoFirstPersonController Player
		{
			get
			{
				//If m_Register hasn't been set yet, we grab it from the scene!
				//This will only happen the first time this reference is used.
				if( m_Player == null )
				{
					m_Player = GameObject.FindObjectOfType<ICEDemoFirstPersonController>();
				}
				
				return m_Player;
			}
		}

	    public float WalkSpeed = 6;
		public float RunSpeed = 10;
		public float JumpSpeed = 5f;

	    public float MouseSensitivity = 2.0f;
	    public float PitchLimit = 60.0f;

		public Camera m_camera;
	    

	    float _pitch = 0;
		float _vertical_forces = 0;

	    private CharacterController m_character_controller;

		public Transform MuzzleHeading = null;
		public Transform MuzzlePitch = null;

		public Rigidbody Projectile;
		public int ProjectileSpeed = 10;
		public Transform Muzzle;
		public GameObject MuzzleEffect = null;
		public AudioClip ShootingSound;	
		private AudioSource m_AudioSource;

		private bool _aiming = false;

		public Texture CrossHair = null;
		
		private Rect _crosshair_rect;
		
		public float Health = 100;
		public float Damage = 0.1f;

		public bool UseRespawn = true;
		public Transform RespawnPosition;

		public string[] CollissionTags;


	    void Start()
	    {
			Cursor.visible = false;

			if( m_camera == null )
				m_camera = GameObject.Find("FPCamera").GetComponent<Camera>();

			if( ShootingSound != null )
			{
				m_AudioSource = gameObject.AddComponent<AudioSource>();
				m_AudioSource.rolloffMode = AudioRolloffMode.Linear;
				m_AudioSource.volume = 0.5f;
				m_AudioSource.minDistance = 1;
				m_AudioSource.maxDistance = 100;
				m_AudioSource.spatialBlend = 1f;
			}

	        m_character_controller = GetComponent<CharacterController>();

			_crosshair_rect = new Rect((Screen.width - CrossHair.width) / 2, (Screen.height - CrossHair.height) /2, CrossHair.width, CrossHair.height);
	   }

		void OnDisable() {
			_aiming = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}


	    void Update()
	    {
			if( transform.position.y < -1 )
				Health -= Damage * 10 * Time.deltaTime;

			if( Health <= 0 )
			{
				if( RespawnPosition != null )
					transform.position = RespawnPosition.position;

				Health = 100;
			}
				

		    //Rotation
		    float _heading = Input.GetAxis("Mouse X") * MouseSensitivity;
		    transform.Rotate(0, _heading, 0);

		    _pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;
		    _pitch = Mathf.Clamp( _pitch, -PitchLimit, PitchLimit );
		    m_camera.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);

		    //Movement
		    float _forward_speed = Input.GetAxis("Vertical") * WalkSpeed;
		    float _side_speed = Input.GetAxis("Horizontal") * WalkSpeed;

		    _vertical_forces += Physics.gravity.y * Time.deltaTime;

		    if (Input.GetButtonDown("Jump") && m_character_controller.isGrounded)
		 		_vertical_forces = JumpSpeed;
			else if( Input.GetKey( KeyCode.LeftShift ) )
				_forward_speed = Input.GetAxis("Vertical") * RunSpeed;

		    Vector3 _speed = new Vector3( _side_speed, _vertical_forces, _forward_speed );
		    _speed = transform.rotation * _speed;

		    m_character_controller.Move( _speed * Time.deltaTime );

			if( Input.GetButtonDown("Fire1") && _aiming )
			{
				Quaternion _rotation = transform.rotation;
				_rotation.x = MuzzlePitch.rotation.x;

				Rigidbody _projectile = Instantiate( Projectile, Muzzle.position, _rotation) as Rigidbody;
				_projectile.velocity = transform.TransformDirection(Vector3.forward * ProjectileSpeed );
				//            Physics.IgnoreCollision ( projectilePrefab.collider, transform.root.collider );
				
				if( m_AudioSource != null && ShootingSound != null )
					m_AudioSource.PlayOneShot( ShootingSound );
				
				if( MuzzleEffect != null )
					Instantiate( MuzzleEffect, Muzzle.position, Muzzle.rotation );
			}
			else if (Input.GetKeyDown( KeyCode.Q ))
			{
				ToogleAiming();
			}
		}

		void OnGUI()
		{
			if( _aiming )
			{
				GUI.DrawTexture( _crosshair_rect, CrossHair);

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else 
			{
				Cursor.lockState = CursorLockMode.None; 
				Cursor.visible = true;
			}
		}

		private void ToogleAiming()
		{
			_aiming = ! _aiming;

			if( _aiming )
				Cursor.lockState = CursorLockMode.Locked; 
			else
				Cursor.lockState = CursorLockMode.None; 
		}


	    bool lockMovement()
	    {

	            return false;
	    }

		void OnCollisionEnter( Collision collision ) 
		{

			foreach( ContactPoint _point in collision.contacts )
			{
				foreach( string _tag in CollissionTags )
				{
					if( _point.otherCollider.CompareTag( _tag ) )
					{
						Health -= Damage;
					}
				}

			}

			
		}
	}
}
