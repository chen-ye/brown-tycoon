using UnityEngine;
using System.Collections;

namespace Tycoon
{
    [System.Serializable]
    public class AgentManager : MonoBehaviour
    {

        public int numAgents = 10;
        public Object agentType;
        public Transform startPosition;
        public float startBox = 10;

        public GameObject[] agentList;

        // Use this for initialization
        void Start()
        {
            this.agentList = new GameObject[this.numAgents];
            if (this.agentType != null)
            {
                for(int i=0;i<this.numAgents;i++)
                {
                    Vector3 position = this.startPosition.position;
                    position.x += Random.Range(-startBox, startBox);
                    position.z += Random.Range(-startBox, startBox);
                    GameObject agent = Instantiate(this.agentType, position, Quaternion.AngleAxis(Random.Range(0f,360f),Vector3.up)) as GameObject;
                    agent.transform.parent = this.transform;
                    //agent.GetComponent<NEEDSIM.NEEDSIMNode>().
                    agentList[i] = agent;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

