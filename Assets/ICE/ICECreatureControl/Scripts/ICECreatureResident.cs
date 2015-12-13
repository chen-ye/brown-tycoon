using UnityEngine;
using System.Collections;
using ICE;
using ICE.Creatures.Objects;

namespace ICE.Creatures{

	public class ICECreatureResident : MonoBehaviour {
		
		void Start () {
			CreatureRegister.Register( gameObject );
		}

		void OnDestroy() {
			CreatureRegister.Deregister( gameObject );
		}
	}
}
