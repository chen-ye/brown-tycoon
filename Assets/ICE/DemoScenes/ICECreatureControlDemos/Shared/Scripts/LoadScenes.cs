using UnityEngine;
using System.Collections;

public class LoadScenes : MonoBehaviour {

	public void LoadEsentials() {
		Application.LoadLevel("BasicDemoScene");
	}

	public void LoadMissions() {
		Application.LoadLevel("SimpleDemoScene");
	}
}
