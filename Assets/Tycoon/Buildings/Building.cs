using UnityEngine;
using NEEDSIM;
using System.Collections;

public class Building : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        NEEDSIMNode[] nodes = this.GetComponentsInChildren<NEEDSIMNode>();
        foreach (NEEDSIMNode node in nodes)
        {
            NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(node);
        }
        
    }
}
