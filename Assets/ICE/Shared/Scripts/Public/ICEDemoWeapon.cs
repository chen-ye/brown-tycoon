using UnityEngine;
using System.Collections;
using ICE;
using ICE.Creatures;

namespace ICE.Shared
{
	public class ICEDemoWeapon: MonoBehaviour
	{
		private float m_Timer = 10;
		public int Interval = 15;
		
		public Rigidbody Projectile;
		public int ProjectileSpeed = 10;
		public Transform Muzzle;
		public GameObject MuzzleEffect = null;
		public AudioClip ShootingSound;	
		private AudioSource m_AudioSource;

		private bool _active = false;

		private ICECreatureControl m_Controller = null;
		private ICECreatureControl Controller
		{
			get{
				if( m_Controller == null )
					m_Controller = (ICECreatureControl)transform.GetComponentInParent<ICECreatureControl>();

				return m_Controller;
			}
		}

		void Start()
		{
			if( ShootingSound != null )
			{
				m_AudioSource = gameObject.AddComponent<AudioSource>();
				m_AudioSource.rolloffMode = AudioRolloffMode.Linear;
				m_AudioSource.volume = 0.5f;
				m_AudioSource.minDistance = 1;
				m_AudioSource.maxDistance = 100;
				m_AudioSource.spatialBlend = 1f;
			}
			
		}

		void OnEnable() {
			_active = true;
		}

		void OnDisable() {
			_active = false;
		}
		
		
		void Update()
		{
			if( ! _active )
				return;

			m_Timer += Time.deltaTime;
			
			if( m_Timer >= Interval )
			{
				m_Timer = 0;

				Quaternion _rotation = transform.rotation;

				if( Controller )
				{
					if( Controller.Creature.ActiveTarget.IsValid )
					{
						Vector3 _heading = Controller.Creature.ActiveTarget.TargetGameObject.transform.position - Controller.transform.position;
						_rotation = Quaternion.LookRotation(_heading );
					}
					else
						_rotation = Controller.transform.rotation;
				}

				Rigidbody _projectile = Instantiate( Projectile, transform.position, _rotation ) as Rigidbody;
				_projectile.velocity = transform.TransformDirection(Vector3.forward * ProjectileSpeed );
				//            Physics.IgnoreCollision ( projectilePrefab.collider, transform.root.collider );
				
				if( m_AudioSource != null && ShootingSound != null )
					m_AudioSource.PlayOneShot( ShootingSound );
				
				if( MuzzleEffect != null )
					Instantiate( MuzzleEffect, Muzzle.position, Muzzle.rotation );
			}

		}
		
		void OnGUI()
		{

		}
		

	}
}