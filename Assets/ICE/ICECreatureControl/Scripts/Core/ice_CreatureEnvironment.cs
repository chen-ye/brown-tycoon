using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{
	//--------------------------------------------------
	// ICECreatureObject.FootstepDataObject
	//--------------------------------------------------
	[System.Serializable]
	public class EnvironmentObject : System.Object
	{
		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			SurfaceHandler.Init( m_Owner );
			CollisionHandler.Init( m_Owner );

		}

		[SerializeField]
		private CollisionObject m_Collision = new CollisionObject();
		public CollisionObject CollisionHandler
		{
			get{ return m_Collision; }
		}

		[SerializeField]
		private SurfaceObject m_Surface = new SurfaceObject();
		public SurfaceObject SurfaceHandler
		{
			get{ return m_Surface; }
		}


	}
}
