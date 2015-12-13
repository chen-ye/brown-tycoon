using UnityEngine;
using System.Collections;

namespace ICE.Shared
{
	[RequireComponent(typeof(Rigidbody))]
	public class ICEDemoProjectile : MonoBehaviour {

		public float Lifespan = 5;
		private float m_Timer = 0;

		public Color ProjectileColor = Color.red;
		public bool RandomColor = true;

		//public bool EffectlessAfterCollision = false;
		//public string EffectlessTag = "";

		public bool DestroyOnCollision = false;
		public string DestroyOnCollisionTag = "";
		public GameObject CollisionEffect = null;

		void Awake () 
		{
			MeshRenderer _renderer = transform.GetComponent<MeshRenderer>();
			
			if( _renderer )
			{
				if( RandomColor )
					_renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
				else
					_renderer.material.color = ProjectileColor;
			}

			float _size = Random.Range(0.25f,0.75f);
			transform.localScale = new Vector3( _size, _size, _size );
		}

		void Update () {
		
			m_Timer += Time.deltaTime;

			if( m_Timer > Lifespan )
				GameObject.Destroy( gameObject );

		}

		void OnCollisionEnter(Collision collision) 
		{
			string _tag = "";
			/*
			if( EffectlessAfterCollision && EffectlessTag != "" )
			{
				if( transform.CompareTag( EffectlessTag ) )
					return;
				else 
					transform.tag = EffectlessTag;
			}*/

			foreach( ContactPoint _point in collision.contacts )
			{
				Vector3 _pos = _point.point;
				Quaternion _rot = transform.rotation;

			
				if( CollisionEffect != null )
					Instantiate( CollisionEffect, _pos, _rot );

				_tag = _point.otherCollider.tag;
			}

			if( DestroyOnCollision && ( DestroyOnCollisionTag == "" || DestroyOnCollisionTag == _tag ) )
				GameObject.Destroy( gameObject );
			
		}

	}
}