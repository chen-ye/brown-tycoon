using UnityEngine;
using System.Collections;
using ICE;
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Shared;
using UnityEngine.UI;

namespace ICE.Creatures.Demo
{
	public class gui_status : MonoBehaviour {

		public Text GroupsCount;
		public Text CreaturesCount;
		public Text FPS;

		public Image PlayerHealthProgressBar;

		public Text PlayerHealth;

		// Use this for initialization
		void Start () {
		
		}

		float deltaTime = 0.0f;
		

		public void DestroyGroup()
		{
			CreatureRegister.DestroyGroup( "EthanRedEscortFollower" );
		}
		
		// Update is called once per frame
		void Update () {
			/**/
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			FPS.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

			if( ICEDemoFirstPersonController.Player != null )
			{
				int _health = (int)ICEDemoFirstPersonController.Player.Health;

				if( _health < 0 )
					_health = 0;

				if( _health > 100 )
					_health = 100;

				PlayerHealthProgressBar.fillAmount = _health / 100;
				PlayerHealth.text = _health + "%";
			}

			GroupsCount.text = CreatureRegister.GroupCount.ToString();
			CreaturesCount.text = CreatureRegister.TotalCreatureCount.ToString();
		}
	}
}
