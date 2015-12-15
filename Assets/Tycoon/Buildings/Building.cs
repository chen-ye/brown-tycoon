using UnityEngine;
using NEEDSIM;
using System.Collections;

public class Building : MonoBehaviour {

    NEEDSIMNode thisNode;
    void Start()
    {
        thisNode = GetComponent<NEEDSIMNode>();
        NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(thisNode, transform.parent.GetComponent<NEEDSIMNode>());
        NEEDSIMNode[] nodes = this.GetComponentsInChildren<NEEDSIMNode>();
        foreach (NEEDSIMNode node in nodes)
        {
            if (node.GetInstanceID() != thisNode.GetInstanceID())
            {
                NEEDSIM.NEEDSIMRoot.Instance.AddNEEDSIMNode(node, thisNode);
            }

        }
    }

	void OnDestroy() 
	{
		thisNode.AffordanceTreeNode.Remove ();
	}
}
