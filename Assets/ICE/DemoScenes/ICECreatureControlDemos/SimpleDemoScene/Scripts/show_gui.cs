using UnityEngine;
using System.Collections;

namespace ICE.Creatures.Demo
{
	public class show_gui : MonoBehaviour {

		// Use this for initialization
		void Start () {
			transform.FindChild( "Canvas" ).gameObject.SetActive(true);
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
